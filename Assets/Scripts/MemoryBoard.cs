﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MemoryBoard : MonoBehaviour {

	static MemoryBoard _instance = null;
	public static MemoryBoard Instance
	{
		get{return _instance;}
	}

	[SerializeField] Image _rightSel = null;
	[SerializeField] Image _wrongSel = null;

	[SerializeField] Sprite[] _allSprites = null;

	[SerializeField] RectTransform _gridContent = null;

	[SerializeField] GameObject _cellObj = null;

	[SerializeField] Image _timerBar = null;

	[SerializeField] Button _hintButton = null;
	[SerializeField] Button _helpButton = null;
	[SerializeField] Button _backButton = null;
	[SerializeField] Button _replyButton = null;


	[SerializeField] Text _gameModeText = null;

	[SerializeField] GameObject _succeedPage = null;

	[SerializeField] GameObject _preventTouchObj = null;

	[SerializeField] GameObject _tutorialPageObj = null;
	int _tutorialCounter = 0;
	bool _tutorialFinished = false;
	bool _firstShowTutorialPage = false;

	float _glimpTime = 5;

	[SerializeField] float _summonDuration = 1.0f;

	int _lastCellNumber = -1;
	GameObject _lastCellObj = null;
	GameObject _firstCellObj = null;

	float _playDuration = -1;
	int _wrongCount = 0;
	int _hintCount = 0;
	int _helpCount = 0;


	Dictionary<GameObject, int> _cellsNumsDic = new Dictionary<GameObject, int>();

	bool _gameFinished;
	public bool GameFinished
	{
		get{return _gameFinished;}
	}

	public int WrongCount
	{
		get{return _wrongCount;}
	}

	public int HintCount
	{
		get{return _hintCount;}
	}

	public float PlayDuration
	{
		get{return _playDuration;}
	}

	void Awake()
	{
		_instance = this;
	}

	public int HelpCount
	{
		get{return _helpCount;}
	}


	// Use this for initialization
	void Start () {

		_wrongSel.DOFade (0, 0);
		_rightSel.DOFade (0, 0);

		_tutorialFinished = GamePlayerPrefs.Instance.IsTutorialDoneMemory ();

		if (DataCarrier.Instance.GameMode == GameModes.Normal)
			_summonDuration *= 2;
		if (DataCarrier.Instance.GameMode == GameModes.Hard)
			_summonDuration *= 3;

		_gameModeText.text = (DataCarrier.Instance.SelectedStage + 1) + " " + DataCarrier.Instance.GetString (DataCarrier.Instance.GameMode.ToString ());
		
		var textAsset = Resources.Load<TextAsset> ("Levels/" + DataCarrier.Instance.GameMode.ToString ());
		var jsonObj = JSONObject.Create (textAsset.text);
		var stage = jsonObj.GetField ("stages").list [DataCarrier.Instance.SelectedStage];
		var cellsList = stage.GetField ("cells").list;
		var spritesList = stage.GetField ("sprites").list;

		_glimpTime = stage.GetField ("glimpTime").f;


		for(int i = 0; i < cellsList.Count; ++i)
			Instantiate(_cellObj, _gridContent);

		int iter = 0;
		for(int i = 0; i < cellsList.Count; ++i)
		{
			int cellIndex = (int)cellsList[i].i;
			var cellObj = _gridContent.GetChild (cellIndex).gameObject;
			int spriteIndex = (int)spritesList [iter].i;
			cellObj.transform.Find ("pic").GetComponent<Image> ().sprite = _allSprites [spriteIndex];

			cellObj.GetComponent<Button>().onClick.AddListener(()=>CellClick(cellObj));
			_cellsNumsDic.Add(cellObj, iter);

			iter += i % 2;
		}

		if(!_tutorialFinished && !_firstShowTutorialPage)
		{
			_firstShowTutorialPage = true;
			_tutorialPageObj.SetActive (true);
			_tutorialPageObj.transform.Find ("Backg/button").GetComponent<Button> ().onClick.AddListener (() => {
				_tutorialPageObj.SetActive(false);
				if(_playDuration == -1)
					StartCoroutine (ShowFirstGlimp ());
			});
		}

		if (_tutorialFinished)
			StartCoroutine (ShowFirstGlimp ());

		_hintButton.onClick.AddListener (() => {
			ShowHintCells();
		});
		_helpButton.onClick.AddListener (() => {
			StartCoroutine (ShowHelpPics ());
		});
		_backButton.onClick.AddListener (() => {
			SceneTransitor.Instance.TransitScene (DataCarrier.SCENE_STAGE_MENU);
		});
		_replyButton.onClick.AddListener (() => {
			SceneTransitor.Instance.TransitScene (DataCarrier.SCENE_GAME_MEMORY);
		});

		UpdateUiTexts ();
	}

	IEnumerator ShowFirstGlimp()
	{
		_timerBar.DOFillAmount (0, _glimpTime).SetEase(Ease.Linear);

		foreach(var obj in _gridContent)
		{
			var tr = obj as Transform;
			tr.Find ("pic").gameObject.SetActive (true);
		}

		yield return new WaitForSeconds (_glimpTime);

		foreach(var obj in _gridContent)
		{
			var tr = obj as Transform;
			var image = tr.transform.Find ("pic").GetComponent<Image> ();
			image.DOFade (0, .5f).OnComplete (() => {
				image.color = Color.white;
				image.gameObject.SetActive(false);
			});
		}

		yield return new WaitForSeconds (.5f);

		_preventTouchObj.SetActive (false);

		_timerBar.transform.parent.gameObject.SetActive (false);

		_helpButton.interactable = true;
		_hintButton.interactable = false;

		_playDuration = 0;


		if(!_tutorialFinished)
		{			
			ShowPairedCellsForTutorial (0);

			_tutorialPageObj.SetActive (true);
			_tutorialPageObj.transform.Find ("Backg/text1").gameObject.SetActive (false);
			_tutorialPageObj.transform.Find ("Backg/text2").gameObject.SetActive (true);
			_tutorialPageObj.transform.Find ("Backg/button/text1").gameObject.SetActive (false);
			_tutorialPageObj.transform.Find ("Backg/button/text2").gameObject.SetActive (true);

		}
	}

	List<GameObject> _tutorialCellObjsList = new List<GameObject>();
	void ShowPairedCellsForTutorial(int num)
	{
		int cellNum = num;
		foreach(var pair in _cellsNumsDic)
		{
			if(pair.Value == cellNum)
			{
				_tutorialCellObjsList.Add (pair.Key);
			}
		}

		foreach (var cellObj in _tutorialCellObjsList)
		{
			var tickObj = Instantiate (transform.Find ("tick").gameObject, cellObj.transform);
			tickObj.name = "tick";
			tickObj.transform.localPosition = Vector3.zero;
		}
	}


	void ShowHintCells()
	{
		if (_firstCellObj == null)
			return;

		if (_cellsNumsDic.Count < 4)
			return;

		_hintCount++;

		//GamePlayerPrefs.Instance.SetHint (GamePlayerPrefs.Instance.GetHint() - 1);

		UpdateUiTexts ();

		List<GameObject> helpsList = new List<GameObject> ();

		List<GameObject> randomsList = new List<GameObject> ();
		foreach (var pair in _cellsNumsDic)
		{
			if (pair.Key == _firstCellObj)
				continue;
			if (pair.Value != _lastCellNumber)
				randomsList.Add (pair.Key);
			else
				helpsList.Add (pair.Key);
		}

		do {
			int index = UnityEngine.Random.Range(0, randomsList.Count);
			var cell = randomsList[index];
			randomsList.RemoveAt(index);
			helpsList.Add(cell);
		} while(helpsList.Count < 3);

		foreach (var obj in helpsList)
			obj.transform.Find ("ques").gameObject.SetActive (true);

	}

	IEnumerator ShowHelpPics()
	{
		if (_firstCellObj != null)
			yield break;

		_helpCount++;

		//GamePlayerPrefs.Instance.SetHelp (GamePlayerPrefs.Instance.GetHelp() - 1);

		UpdateUiTexts ();
		
		_preventTouchObj.SetActive (true);

		float fadeTime = .3f;
		foreach(var pair in _cellsNumsDic)
		{
			var image = pair.Key.transform.Find ("pic").GetComponent<Image> ();
			image.gameObject.SetActive (true);
			image.color = new Color (1, 1, 1, 0);
			image.DOFade (.5f, fadeTime);
		}

		yield return new WaitForSeconds (_summonDuration);

		foreach(var pair in _cellsNumsDic)
		{
			var image = pair.Key.transform.Find ("pic").GetComponent<Image> ();
			image.DOFade (0, fadeTime);
		}

		yield return new WaitForSeconds (fadeTime);

		foreach(var pair in _cellsNumsDic)
		{
			var image = pair.Key.transform.Find ("pic").GetComponent<Image> ();
			image.color = Color.white;
			image.gameObject.SetActive (false);
		}
		
		_preventTouchObj.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		if (_gameFinished)
			return;

		if (_playDuration != -1)
			_playDuration += Time.deltaTime;
	}

	void UpdateUiTexts()
	{
		_hintButton.transform.Find ("hintText").GetComponent<Text> ().text = GamePlayerPrefs.Instance.GetHint ().ToString ();
		_helpButton.transform.Find ("helpText").GetComponent<Text> ().text = GamePlayerPrefs.Instance.GetHelp ().ToString ();
	}

	void CellClick(GameObject cellObj)
	{
		bool isTutorialFinished = _tutorialFinished;

		if(!_tutorialFinished)
		{
			var tickObj = cellObj.transform.Find ("tick");
			if(tickObj == null)
			{
				return;
			}
			else
			{
				Destroy (tickObj.gameObject);
				_tutorialCounter++;

				if (_tutorialCounter == 2 || _tutorialCounter == 4)
				{
					_tutorialCellObjsList.Clear ();
					ShowPairedCellsForTutorial (_tutorialCounter / 2);
				}
				if (_tutorialCounter == 6)
				{
					_tutorialFinished = true;
					GamePlayerPrefs.Instance.DoneTutorialMemory ();	
				}
			}
		}

		if (!_cellsNumsDic.ContainsKey (cellObj))
			return;
		int cellNum = _cellsNumsDic [cellObj];

		if (!_cellsNumsDic.ContainsKey (cellObj))
			return;

		foreach (var ent in _cellsNumsDic)
			ent.Key.transform.Find ("ques").gameObject.SetActive (false);
		
		if(_lastCellNumber == -1)
		{
			cellObj.transform.Find ("pic").gameObject.SetActive (true);
			_firstCellObj = cellObj;
			_lastCellNumber = cellNum;
			_helpButton.interactable = false;
			_hintButton.interactable = true;
		}
		else
		{
			_helpButton.interactable = true;
			_hintButton.interactable = false;
			if(cellNum == _lastCellNumber)
			{
				cellObj.transform.Find ("pic").gameObject.SetActive (true);
				_lastCellNumber = -1;

				_lastCellObj = cellObj;

				_cellsNumsDic.Remove (_firstCellObj);
				_cellsNumsDic.Remove (_lastCellObj);

				_firstCellObj = null;
				_lastCellObj = null;

				if (_cellsNumsDic.Count == 0)
					StartCoroutine (FinishGame ());

				if(isTutorialFinished)
				{
					_rightSel.DOKill ();
					_rightSel.DOFade (1, 0);
					_rightSel.DOFade (0, .3f).SetDelay (.6f);
				}

				if (_tutorialCounter == 6)
				{
					_tutorialFinished = true;
					GamePlayerPrefs.Instance.DoneTutorialMemory ();	
				}

				//right sound
			}
			else
			{
				_lastCellObj = cellObj;
				StartCoroutine (HideWrongCells ());
				_wrongCount++;
				_wrongSel.DOKill ();
				_wrongSel.DOFade (1, 0);
				_wrongSel.DOFade (0, .3f).SetDelay (.4f);
				//wrog sound
				//hide sound with delay
			}
		}
	}

	IEnumerator HideWrongCells()
	{
		_preventTouchObj.SetActive (true);

		_lastCellObj.transform.Find ("pic").gameObject.SetActive (true);

		yield return new WaitForSeconds (.5f);

		_lastCellObj.transform.Find ("pic").gameObject.SetActive (false);
		_firstCellObj.transform.Find ("pic").gameObject.SetActive (false);
		_firstCellObj = null;
		_lastCellObj = null;
		_lastCellNumber = -1;

		_preventTouchObj.SetActive (false);
	}

	IEnumerator FinishGame()
	{
		_preventTouchObj.SetActive (true);

		yield return new WaitForSeconds (1);

		GamePlayerPrefs.Instance.SetPlayTime (DataCarrier.Instance.GameMode, DataCarrier.Instance.SelectedStage, (int)_playDuration);
		GamePlayerPrefs.Instance.SetPlayRecord (DataCarrier.Instance.GameMode, DataCarrier.Instance.SelectedStage, _wrongCount);

		GamePlayerPrefs.Instance.UnlockStage ( DataCarrier.Instance.GameMode, DataCarrier.Instance.SelectedStage + 1);

		_succeedPage.SetActive (true);
		_gameFinished = true;

		yield return new WaitForSeconds (1);

		_preventTouchObj.SetActive (false);
	}
}
