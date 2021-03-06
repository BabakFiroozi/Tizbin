﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MemoryBoard : MonoBehaviour
{
    public static MemoryBoard Instance { get; private set; } = null;

    [SerializeField] Image _rightSelectedSign = null;
    [SerializeField] Image _wrongSelectedSign = null;

    [SerializeField] RectTransform _gridContent = null;

    [SerializeField] GameObject _cellObj = null;

    [SerializeField] Image _timerBar = null;

    [SerializeField] Button _hintButton = null;
    [SerializeField] Button _helpButton = null;
    [SerializeField] Button _replyButton = null;


    [SerializeField] Text _gameModeText = null;

    [SerializeField] GameObject _succeedPage = null;

    [SerializeField] GameObject _preventTouchObj = null;

    [SerializeField] GameObject _tutorialPageObj = null;

    [SerializeField] float _glimpDuration = 1.0f;

    [SerializeField] GameObject _tutorialTick = null;


    int _tutorialCounter = 0;
    bool _tutorialFinished = false;
    bool _firstShowTutorialPage = false;

    float _glimpTime = 5;
    int _maxSpritesCount = 20;
    int _cellsCount = 12;


    int _lastCellIdSelected = -1;
    GameObject _lastCellObj = null;
    GameObject _firstCellObj = null;
    Dictionary<GameObject, int> _cellsNumsDic = new Dictionary<GameObject, int>();

    public bool GameFinished { get; private set; }
    public int WrongCount { get; private set; } = 0;
    public int HintCount { get; private set; } = 0;
    public float PlayDuration { get; private set; } = -1;
    public int HelpCount { get; private set; } = 0;

    List<GameObject> _tutorialCellObjsList = new List<GameObject>();


    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        _wrongSelectedSign.DOFade(0, 0);
        _rightSelectedSign.DOFade(0, 0);

        _tutorialFinished = GameSaveData.IsTutorialDoneMemory();

        if (DataHelper.Instance.GameMode == GameModes.Easy)
        {
            _cellsCount = 12;
            _glimpDuration *= 1;
            _glimpTime = 5;
        }
        if (DataHelper.Instance.GameMode == GameModes.Normal)
        {
            _cellsCount = 16;
            _glimpDuration *= 2;
            _glimpTime = 7;
        }
        if (DataHelper.Instance.GameMode == GameModes.Hard)
        {
            _cellsCount = 20;
            _glimpDuration *= 3;
            _glimpTime = 9;
        }

        _gameModeText.text = (DataHelper.Instance.SelectedStage + 1) + " " + Translator.GetString(DataHelper.Instance.GameMode.ToString());


        List<int> cellsList = new List<int>();
        for (int c = 0; c < _cellsCount; ++c)
            cellsList.Add(c);

        var memCellsList = new List<int>();

        do
        {
            int index = UnityEngine.Random.Range(0, cellsList.Count);
            int cellIndex = cellsList[index];
            memCellsList.Add(cellIndex);
            cellsList.RemoveAt(index);
        } while (cellsList.Count > 0);

        List<int> spritesList = new List<int>();
        for (int c = 0; c < _maxSpritesCount; ++c)
            spritesList.Add(c);

        do
        {
            int index = UnityEngine.Random.Range(0, spritesList.Count);
            spritesList.RemoveAt(index);
        } while (spritesList.Count != _cellsCount / 2);


        for (int i = 0; i < memCellsList.Count; ++i)
            Instantiate(_cellObj, _gridContent);

        int iterId = 0;
        for (int i = 0; i < memCellsList.Count; ++i)
        {
            int cellIndex = memCellsList[i];
            var cellObj = _gridContent.GetChild(cellIndex).gameObject;
            int spriteIndex = spritesList[iterId];
            cellObj.transform.Find("pic").GetComponent<Image>().sprite = GameAsset.Instance.AllSprites[spriteIndex];

            cellObj.GetComponent<Button>().onClick.AddListener(() => CellClick(cellObj));
            _cellsNumsDic.Add(cellObj, iterId);

            iterId += i % 2;
        }

        if (!_tutorialFinished && !_firstShowTutorialPage)
        {
            _firstShowTutorialPage = true;
            _tutorialPageObj.SetActive(true);
            _tutorialPageObj.transform.Find("Backg/button").GetComponent<Button>().onClick.AddListener(() =>
            {
                _tutorialPageObj.SetActive(false);
                if (PlayDuration == -1)
                    StartCoroutine(ShowFirstGlimp());
            });
        }

        if (_tutorialFinished)
            StartCoroutine(ShowFirstGlimp());

        _hintButton.onClick.AddListener(() =>
        {
            ShowHintCells();
        });
        _helpButton.onClick.AddListener(() =>
        {
            StartCoroutine(ShowHelpPics());
        });
        _replyButton.onClick.AddListener(() =>
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME_MEMORY);
        });

        UpdateUiTexts();
    }

    IEnumerator ShowFirstGlimp()
    {
        _timerBar.DOFillAmount(0, _glimpTime).SetEase(Ease.Linear);

        foreach (var obj in _gridContent)
        {
            var tr = obj as Transform;
            tr.Find("pic").gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(_glimpTime);

        foreach (var obj in _gridContent)
        {
            var tr = obj as Transform;
            var image = tr.transform.Find("pic").GetComponent<Image>();
            image.DOFade(0, .5f).OnComplete(() =>
            {
                image.color = Color.white;
                image.gameObject.SetActive(false);
            });
        }

        yield return new WaitForSeconds(.5f);

        _preventTouchObj.SetActive(false);

        _timerBar.transform.parent.gameObject.SetActive(false);

        _helpButton.interactable = true;
        _hintButton.interactable = false;

        PlayDuration = 0;


        if (!_tutorialFinished)
        {
            ShowPairedCellsForTutorial(0);

            _tutorialPageObj.SetActive(true);
            _tutorialPageObj.transform.Find("Backg/text1").gameObject.SetActive(false);
            _tutorialPageObj.transform.Find("Backg/text2").gameObject.SetActive(true);
            _tutorialPageObj.transform.Find("Backg/button/text1").gameObject.SetActive(false);
            _tutorialPageObj.transform.Find("Backg/button/text2").gameObject.SetActive(true);

        }

        _rightSelectedSign.GetComponent<RectTransform>().anchoredPosition = _gridContent.GetComponent<RectTransform>().anchoredPosition +
        new Vector2(0, _gridContent.GetComponent<RectTransform>().sizeDelta.y / 2 + _rightSelectedSign.GetComponent<RectTransform>().sizeDelta.y / 2);
        _wrongSelectedSign.GetComponent<RectTransform>().anchoredPosition = _rightSelectedSign.GetComponent<RectTransform>().anchoredPosition;
    }

    void ShowPairedCellsForTutorial(int num)
    {
        int cellNum = num;
        foreach (var pair in _cellsNumsDic)
        {
            if (pair.Value == cellNum)
            {
                _tutorialCellObjsList.Add(pair.Key);
            }
        }

        foreach (var cellObj in _tutorialCellObjsList)
        {
            var tickObj = Instantiate(_tutorialTick, cellObj.transform);
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

        HintCount++;

        //GamePlayerPrefs.Instance.SetHint (GamePlayerPrefs.Instance.GetHint() - 1);

        UpdateUiTexts();

        List<GameObject> helpsList = new List<GameObject>();

        List<GameObject> randomsList = new List<GameObject>();
        foreach (var pair in _cellsNumsDic)
        {
            if (pair.Key == _firstCellObj)
                continue;
            if (pair.Value != _lastCellIdSelected)
                randomsList.Add(pair.Key);
            else
                helpsList.Add(pair.Key);
        }

        do
        {
            int index = UnityEngine.Random.Range(0, randomsList.Count);
            var cell = randomsList[index];
            randomsList.RemoveAt(index);
            helpsList.Add(cell);
        } while (helpsList.Count < 3);

        foreach (var obj in helpsList)
            obj.transform.Find("ques").gameObject.SetActive(true);

    }

    IEnumerator ShowHelpPics()
    {
        if (_firstCellObj != null)
            yield break;

        HelpCount++;

        //GamePlayerPrefs.Instance.SetHelp (GamePlayerPrefs.Instance.GetHelp() - 1);

        UpdateUiTexts();

        _preventTouchObj.SetActive(true);

        float fadeTime = .3f;
        foreach (var pair in _cellsNumsDic)
        {
            var image = pair.Key.transform.Find("pic").GetComponent<Image>();
            image.gameObject.SetActive(true);
            image.color = new Color(1, 1, 1, 0);
            image.DOFade(.5f, fadeTime);
        }

        yield return new WaitForSeconds(_glimpDuration);

        foreach (var pair in _cellsNumsDic)
        {
            var image = pair.Key.transform.Find("pic").GetComponent<Image>();
            image.DOFade(0, fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);

        foreach (var pair in _cellsNumsDic)
        {
            var image = pair.Key.transform.Find("pic").GetComponent<Image>();
            image.color = Color.white;
            image.gameObject.SetActive(false);
        }

        _preventTouchObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (GameFinished)
            return;

        if (PlayDuration != -1)
            PlayDuration += Time.deltaTime;
    }

    void UpdateUiTexts()
    {
        _hintButton.transform.Find("hintText").GetComponent<Text>().text = GameSaveData.GetHint().ToString();
        _helpButton.transform.Find("helpText").GetComponent<Text>().text = GameSaveData.GetHelp().ToString();
    }

    void CellClick(GameObject cellObj)
    {
        bool isTutorialFinished = _tutorialFinished;

        if (!_tutorialFinished)
        {
            var tickObj = cellObj.transform.Find("tick");
            if (tickObj == null)
            {
                return;
            }
            else
            {
                Destroy(tickObj.gameObject);
                _tutorialCounter++;

                if (_tutorialCounter == 2 || _tutorialCounter == 4)
                {
                    _tutorialCellObjsList.Clear();
                    ShowPairedCellsForTutorial(_tutorialCounter / 2);
                }
                if (_tutorialCounter == 6)
                {
                    _tutorialFinished = true;
                    GameSaveData.DoneTutorialMemory();
                }
            }
        }

        if (!_cellsNumsDic.ContainsKey(cellObj))
            return;

        int cellNum = _cellsNumsDic[cellObj];

        foreach (var ent in _cellsNumsDic)
            ent.Key.transform.Find("ques").gameObject.SetActive(false);

        if (_lastCellIdSelected == -1)
        {
            cellObj.transform.Find("pic").gameObject.SetActive(true);
            _firstCellObj = cellObj;
            _lastCellIdSelected = cellNum;
            _helpButton.interactable = false;
            _hintButton.interactable = true;
        }
        else
        {
            _helpButton.interactable = true;
            _hintButton.interactable = false;
            if (cellNum == _lastCellIdSelected)
            {
                cellObj.transform.Find("pic").gameObject.SetActive(true);
                _lastCellIdSelected = -1;

                _lastCellObj = cellObj;

                _cellsNumsDic.Remove(_firstCellObj);
                _cellsNumsDic.Remove(_lastCellObj);

                _firstCellObj = null;
                _lastCellObj = null;

                if (_cellsNumsDic.Count == 0)
                    StartCoroutine(FinishGame());

                if (isTutorialFinished)
                {
                    _rightSelectedSign.DOKill();
                    _rightSelectedSign.DOFade(1, 0);
                    _rightSelectedSign.DOFade(0, .3f).SetDelay(.6f);
                }

                if (_tutorialCounter == 6)
                {
                    _tutorialFinished = true;
                    GameSaveData.DoneTutorialMemory();
                }

                //right sound
            }
            else
            {
                _lastCellObj = cellObj;
                StartCoroutine(HideWrongCells());
                WrongCount++;
                _wrongSelectedSign.DOKill();
                _wrongSelectedSign.DOFade(1, 0);
                _wrongSelectedSign.DOFade(0, .3f).SetDelay(.4f);
                //wrog sound
                //hide sound with delay
            }
        }
    }

    IEnumerator HideWrongCells()
    {
        _preventTouchObj.SetActive(true);

        _lastCellObj.transform.Find("pic").gameObject.SetActive(true);

        yield return new WaitForSeconds(.5f);

        _lastCellObj.transform.Find("pic").gameObject.SetActive(false);
        _firstCellObj.transform.Find("pic").gameObject.SetActive(false);
        _firstCellObj = null;
        _lastCellObj = null;
        _lastCellIdSelected = -1;

        _preventTouchObj.SetActive(false);
    }

    IEnumerator FinishGame()
    {
        _preventTouchObj.SetActive(true);

        yield return new WaitForSeconds(1);

        _succeedPage.SetActive(true);
        GameFinished = true;

        yield return new WaitForSeconds(1);

        _preventTouchObj.SetActive(false);
    }
}
