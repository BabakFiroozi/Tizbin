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

	int _passedCount = 0;
	public int PassedCount
	{
		get{return _passedCount;}
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

	[SerializeField] Text _succeedText = null;

	[SerializeField] GameObject _failPanelObj = null;

	[SerializeField] float[] _hitDuratinaArr = null;
	float _hitDuratin = 5;
	float _hitTimer = 0;

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

	}

	// Use this for initialization
	void Start ()
	{
		if (DataCarrier.Instance.GameMode == GameModes.Normal)
			_gameMode = 1;
		if (DataCarrier.Instance.GameMode == GameModes.Hard)
			_gameMode = 2;

		_hitDuratin = _hitDuratinaArr [_gameMode];
		
		_succeedText.DOFade (0, 0);

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

		obj = _bottAlbume.GetChild (Rand (0, _bottAlbume.childCount)).gameObject;
		obj.GetComponent<Image> ().sprite = _allSprites [randomIndex];
		obj.GetComponent<Button> ().onClick.AddListener (() => ButtonClick (obj));
		_commonPics.Add (obj);

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
	}

	int Rand(int min, int max)
	{
		return UnityEngine.Random.Range (min, max);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(_hitTimer != 0)
		{
			float coef = _hitTimer / _hitDuratin;
			_timerBar.fillAmount = coef;
			_hitTimer -= Time.deltaTime;
			if(_hitTimer <= 0)
			{
				_timerBar.fillAmount = 0;
				FinishGame (false);
				return;
			}
		}
	}

	void ButtonClick(GameObject obj)
	{
		FinishGame (_commonPics.Contains (obj));
	}

	void FinishGame(bool succeed)
	{
		_hitTimer = 0;

		if(succeed)
		{
			_perventTouch.SetActive (true);
			if (_hintRoutine != null)
				StopCoroutine (_hintRoutine);
			_hintButton.interactable = true;
			StartCoroutine (GoNextGeneration ());
			_passedCount++;
		}
		else
		{
			_perventTouch.SetActive (true);
			StartCoroutine (ShowFailedPanel ());
		}
	}

	IEnumerator GoNextGeneration()
	{
		_succeedText.DOFade (1, .5f);
		_succeedText.DOFade (0, .5f).SetDelay(.5f);

		float animTime = .5f;
		_topAlbume.DOAnchorPosX (700, animTime);
		_topAlbume.DORotate (new Vector3 (0, 0, _topAlbume.rotation.eulerAngles.z - 180), animTime);

		_bottAlbume.DOAnchorPosX (-700, animTime);
		_bottAlbume.DORotate (new Vector3 (0, 0, _topAlbume.rotation.eulerAngles.z + 180), animTime);

		yield return new WaitForSeconds (animTime);

		GenerateAlbume ();
	}

	IEnumerator ShowFailedPanel()
	{
		var circleObj = transform.Find ("circle").gameObject;
		circleObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (_commonPics [0].GetComponent<RectTransform> ().rect.width, _commonPics [0].GetComponent<RectTransform> ().rect.height);

		foreach (var obj in _commonPics)
		{
			var circle = Instantiate (circleObj, obj.transform.position, obj.transform.rotation, circleObj.transform.parent);
			var rectTr = circle.GetComponent<RectTransform> ();
			rectTr.gameObject.SetActive (true);
			var seq = DOTween.Sequence ();
			seq.Append (rectTr.DOScale (1.1f, .2f));
			seq.Append (rectTr.DOScale (1.0f, .2f));
			seq.SetLoops (3);
		}

		yield return new WaitForSeconds (2);

		_failPanelObj.SetActive (true);
		_perventTouch.SetActive (false);
	}
}