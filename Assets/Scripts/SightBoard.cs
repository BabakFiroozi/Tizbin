using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class SightBoard : MonoBehaviour
{
	static SightBoard _instance = null;
	public static SightBoard Instance
	{
		get{return _instance;}
	}

	int _succeedCount = 0;
	public int SucceedCount
	{
		get{return _succeedCount;}
	}

	int _helpsCount = 0;
	public int HelpsCount
	{
		get{return _helpsCount;}
	}

	int _hintsCount = 0;
	public int HintsCount
	{
		get{return _hintsCount;}
	}

	[SerializeField] GameObject _perventTouch = null;

	[SerializeField] Button _hintButton = null;
	[SerializeField] Button _helpButton = null;
	[SerializeField] Button _backButton = null;

	[SerializeField] Image _timerBar = null;

	[SerializeField] Sprite[] _allSprites = null;

	[SerializeField] GameObject[] _albumeObjects = null;

	RectTransform _topAlbume = null;
	RectTransform _bottAlbume = null;

	List<GameObject> _commonPics = new List<GameObject>();

	[SerializeField] Image _succeedSign = null;

	[SerializeField] GameObject _failPanelObj = null;
	[SerializeField] GameObject _failedAlert = null;

	List<GameObject> _guideCirclesist = new List<GameObject> ();

	[SerializeField] float[] _hitDuratinaArr = null;
	float _hitDuratin = 0;
	float _hitTimer = 0;

	[SerializeField] GameObject _tutorialPageObj = null;
	int _tutorialCounter = 0;
	bool _tutorialFinished = false;
	bool _firstShowTutorialPage = false;


	IEnumerator ShowHint()
	{
		_hintsCount++;

		_hintButton.interactable = false;

		int eliminate = 2;
		if (_gameMode == 1)
			eliminate = 3;
		if (_gameMode == 2)
			eliminate = 4;

		List<Image> topList = new List<Image>();
		foreach(var o in _topAlbume)
		{
			var tr = o as Transform;
			if(_commonPics.Contains(tr.gameObject))
				continue;
			topList.Add(tr.GetComponent<Image>());
		}
		do
		{
			int index = Rand(0, topList.Count);
			topList.RemoveAt(index);
		}while(topList.Count > eliminate);

		List<Image> bottList = new List<Image>();
		foreach(var o in _bottAlbume)
		{
			var tr = o as Transform;
			if(_commonPics.Contains(tr.gameObject))
				continue;
			bottList.Add(tr.GetComponent<Image>());
		}
		do
		{
			int index = Rand(0, bottList.Count);
			bottList.RemoveAt(index);
		}while(bottList.Count > eliminate);

		foreach(var image in topList)
			image.DOFade (0, .5f);
		
		foreach(var image in bottList)
			image.DOFade (0, .5f);

		yield return new WaitForSeconds (5);

		foreach(var image in topList)
			image.DOFade (1, .5f);

		foreach(var image in bottList)
			image.DOFade (1, .5f);

		yield return new WaitForSeconds (.5f);

		_hintButton.interactable = true;
	}

	Coroutine _hintRoutine = null;

	void Awake()
	{
		_instance = this;

		_tutorialFinished = GamePlayerPrefs.Instance.IsTutorialDoneSight ();
	}

	// Use this for initialization
	void Start ()
	{
		if (DataCarrier.Instance.GameMode == GameModes.Normal)
			_gameMode = 1;
		if (DataCarrier.Instance.GameMode == GameModes.Hard)
			_gameMode = 2;	

		_hitDuratin = _hitDuratinaArr [_gameMode];
		
		_succeedSign.DOFade (0, 0);

		_hintButton.onClick.AddListener (() => {
			_hintRoutine = StartCoroutine (ShowHint ());
		});

		_helpButton.onClick.AddListener (() => {
			_helpsCount++;
			_hitTimer += _hitDuratin / 2;
			if(_hitTimer > _hitDuratin)
				_hitDuratin = _hitTimer;
		});

		_backButton.onClick.AddListener (() => {
			UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_MAIN_MENU);
		});

		GenerateAlbume ();
	}


	void ShowGuidCircles(bool isTut)
	{
		if (isTut)
		{
			_tutorialCounter++;
			if (_tutorialCounter > 3)
			{
				_tutorialFinished = true;
				return;
			}
		}

		foreach (var obj in _guideCirclesist)
			Destroy (obj);
		_guideCirclesist.Clear ();

		var circleObj = transform.Find ("circle").gameObject;
		circleObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (_commonPics [0].GetComponent<RectTransform> ().rect.width, _commonPics [0].GetComponent<RectTransform> ().rect.height);
		foreach (var obj in _commonPics)
		{
			var circle = Instantiate (circleObj, obj.transform.position, circleObj.transform.rotation, obj.transform);
			var rectTr = circle.GetComponent<RectTransform> ();
			rectTr.Rotate (0, 0, Rand (1, 360));
			circle.GetComponent<Image> ().color = isTut ? Color.green : Color.black;
			rectTr.gameObject.SetActive (true);
			var seq = DOTween.Sequence ();
			seq.Append (rectTr.DOScale (1.1f, .2f));
			seq.Append (rectTr.DOScale (1.0f, .2f));
			seq.SetLoops (isTut ? -1 : 3);
			_guideCirclesist.Add (circle);
		}

	}

	int _gameMode = 0;


	void GenerateAlbume()
	{
		if (_topAlbume != null)
			Destroy (_topAlbume.gameObject);
		if (_bottAlbume != null)
			Destroy (_bottAlbume.gameObject);

		var allbumeObj = _albumeObjects [_gameMode];

		_topAlbume = Instantiate (allbumeObj, transform).GetComponent<RectTransform>();
		_topAlbume.anchoredPosition = new Vector2 (700, 310);

		_bottAlbume = Instantiate (allbumeObj, transform).GetComponent<RectTransform>();
		_bottAlbume.anchoredPosition = new Vector2 (-700, -175);

		_commonPics.Clear ();

		List<int> spriteIndicesList = new List<int> ();
		for(int i = 0; i < _allSprites.Length; ++i)
			spriteIndicesList.Add (i);

		int randomIndex = Rand (0, spriteIndicesList.Count);
		int spriteIndex = spriteIndicesList [randomIndex];
		spriteIndicesList.RemoveAt (randomIndex);

		var obj = _topAlbume.GetChild (Rand (0, _topAlbume.childCount)).gameObject;
		obj.GetComponent<Image> ().sprite = _allSprites [randomIndex];
		obj.GetComponent<Button> ().onClick.AddListener (() => ButtonClick (obj));
		_commonPics.Add (obj);
		obj.transform.SetSiblingIndex (_topAlbume.childCount - 1);

		obj = _bottAlbume.GetChild (Rand (0, _bottAlbume.childCount)).gameObject;
		obj.GetComponent<Image> ().sprite = _allSprites [randomIndex];
		obj.GetComponent<Button> ().onClick.AddListener (() => ButtonClick (obj));
		_commonPics.Add (obj);
		obj.transform.SetSiblingIndex (_bottAlbume.childCount - 1);

		foreach(var o in _topAlbume)
		{
			var tr = o as Transform;
			tr.rotation = Quaternion.identity;
			tr.Rotate (0, 0, Rand (0, 360));

			if (_commonPics.Contains (tr.gameObject))
				continue;
			randomIndex = Rand (0, spriteIndicesList.Count);
			spriteIndex = spriteIndicesList[randomIndex];
			spriteIndicesList.RemoveAt (randomIndex);
			tr.GetComponent<Image> ().sprite = _allSprites [spriteIndex];

			var buttonObj = tr.gameObject;
			tr.GetComponent<Button> ().onClick.AddListener (() => ButtonClick (buttonObj));
		}

		foreach(var o in _bottAlbume)
		{
			var tr = o as Transform;
			tr.rotation = Quaternion.identity;
			tr.Rotate (0, 0, Rand (0, 360));

			if (_commonPics.Contains (tr.gameObject))
				continue;
			randomIndex = Rand (0, spriteIndicesList.Count);
			spriteIndex = spriteIndicesList[randomIndex];
			spriteIndicesList.RemoveAt (randomIndex);
			tr.GetComponent<Image> ().sprite = _allSprites [spriteIndex];

			var buttonObj = tr.gameObject;
			tr.GetComponent<Button> ().onClick.AddListener (() => ButtonClick (buttonObj));
		}

		StartCoroutine (StartGame ());
	}

	IEnumerator StartGame()
	{
		_topAlbume.rotation = Quaternion.identity;
		_bottAlbume.rotation = Quaternion.identity;

		float animTime = .5f;
		_topAlbume.DOAnchorPosX (0, animTime);
		_topAlbume.DORotate (new Vector3 (0, 0, Rand (0, 120) / 10f * 30), animTime);

		_bottAlbume.DOAnchorPosX (0, animTime);
		_bottAlbume.DORotate (new Vector3 (0, 0, -Rand (0, 120) / 10f * 30), animTime);

		yield return new WaitForSeconds (animTime);


		_hitTimer = _hitDuratin;
		_perventTouch.SetActive (false);

		_hitDuratin -= .5f;
		if (_hitDuratin < _hitDuratinaArr [_gameMode] / 3)
			_hitDuratin = _hitDuratinaArr [_gameMode] / 3;

		if (!_tutorialFinished)
			ShowGuidCircles (true);

		if(!_tutorialFinished && !_firstShowTutorialPage)
		{
			_firstShowTutorialPage = true;
			_tutorialPageObj.SetActive (true);
			_tutorialPageObj.transform.Find ("Backg/button").GetComponent<Button> ().onClick.AddListener (() => {
				_tutorialPageObj.SetActive(false);
			});
		}
	}

	int Rand(int min, int max)
	{
		return UnityEngine.Random.Range (min, max);
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckTime ();
	}

	void CheckTime()
	{
		if (!_tutorialFinished)
			return;
		
		if(_hitTimer != 0)
		{
			float coef = _hitTimer / _hitDuratin;
			_timerBar.fillAmount = coef;
			_hitTimer -= Time.deltaTime;
			if(_hitTimer <= 0)
			{
				_timerBar.fillAmount = 0;
				FinishGame (2);
				return;
			}
		}
	}

	void ButtonClick(GameObject obj)
	{
		if(_tutorialFinished == false)
		{
			if (_commonPics.Contains (obj))
				FinishGame (0);
			return;
		}

		FinishGame (_commonPics.Contains (obj) ? 0 : 1);
	}

	/// <summary>
	/// cond == 0 is succeed, 1 is wrong select, 2 is time out
	/// </summary>
	/// <param name="cond">Cond.</param>
	void FinishGame(int cond)
	{
		_hitTimer = 0;

		if(cond == 0)
		{
			_perventTouch.SetActive (true);
			if (_hintRoutine != null)
				StopCoroutine (_hintRoutine);
			_hintButton.interactable = true;
			StartCoroutine (GoNextGeneration ());
			if (_tutorialFinished)
				_succeedCount++;

			if (_tutorialFinished)
				GamePlayerPrefs.Instance.DoneTutorialSight ();
		}
		else
		{
			_perventTouch.SetActive (true);
			StartCoroutine (ShowFailedPanel (cond == 2));
		}
	}

	IEnumerator GoNextGeneration()
	{
		_succeedSign.DOFade (1, .5f);
		_succeedSign.DOFade (0, .5f).SetDelay(.5f);

		float animTime = .5f;
		_topAlbume.DOAnchorPosX (700, animTime);
		_topAlbume.DORotate (new Vector3 (0, 0, _topAlbume.rotation.eulerAngles.z - 180), animTime);

		_bottAlbume.DOAnchorPosX (-700, animTime);
		_bottAlbume.DORotate (new Vector3 (0, 0, _topAlbume.rotation.eulerAngles.z + 180), animTime);

		yield return new WaitForSeconds (animTime);

		GenerateAlbume ();
	}

	IEnumerator ShowFailedPanel(bool timeOut)
	{
		ShowGuidCircles (false);

		yield return new WaitForSeconds (.75f);

		_failedAlert.SetActive (true);
		_failedAlert.transform.Find ("timeOut").gameObject.SetActive (timeOut);
		_failedAlert.transform.Find ("wrongSelect").gameObject.SetActive (!timeOut);

		yield return new WaitForSeconds (1.25f);

		_failedAlert.SetActive (false);
		_failPanelObj.SetActive (true);
		_perventTouch.SetActive (false);
	}
}