﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using SightGame.Server;

public class SightBoard : MonoBehaviour
{
    public static SightBoard Instance { get; private set; } = null;

    public int SucceedCount { get; private set; } = 0;
    public int HelpsCount { get; private set; } = 0;
    public int HintsCount { get; private set; } = 0;

    [SerializeField] GameObject _circleObj = null;

    [SerializeField] GameObject _perventTouch = null;

    [SerializeField] Button _hintButton = null;
    [SerializeField] Button _helpButton = null;

    [SerializeField] Image _timerBar = null;

    [SerializeField] GameObject[] _albumeObjects = null;

    RectTransform _topAlbume = null;
    RectTransform _bottAlbume = null;

    List<GameObject> _commonPics = new List<GameObject>();

    [SerializeField] Image _succeedSign = null;
    [SerializeField] RectTransform _failSignTr = null;

    [SerializeField] GameObject _failPanelObj = null;
    [SerializeField] GameObject _failedAlert = null;

    List<GameObject> _guideCirclesList = new List<GameObject>();

    [SerializeField] float[] _hitDuratinaArr = null;
    float _hitDuratin = 0;
    float _hitTimer = 0;

    [SerializeField] GameObject _tutorialPageObj = null;
    int _tutorialCounter = 0;
    bool _tutorialFinished = false;
    bool _firstShowTutorialPage = false;

    Coroutine _hintRoutine = null;

    int _difficulty = 0;



    IEnumerator ShowHint()
    {
        HintsCount++;

        _hintButton.interactable = false;

        int eliminate = 2;
        if (_difficulty == 1)
            eliminate = 3;
        if (_difficulty == 2)
            eliminate = 4;

        List<Image> topList = new List<Image>();
        foreach (var o in _topAlbume)
        {
            var tr = o as Transform;
            if (_commonPics.Contains(tr.gameObject))
                continue;
            topList.Add(tr.GetComponent<Image>());
        }
        do
        {
            int index = Rand(0, topList.Count);
            topList.RemoveAt(index);
        } while (topList.Count > eliminate);

        List<Image> bottList = new List<Image>();
        foreach (var o in _bottAlbume)
        {
            var tr = o as Transform;
            if (_commonPics.Contains(tr.gameObject))
                continue;
            bottList.Add(tr.GetComponent<Image>());
        }
        do
        {
            int index = Rand(0, bottList.Count);
            bottList.RemoveAt(index);
        } while (bottList.Count > eliminate);

        foreach (var image in topList)
            image.DOFade(0, .5f);

        foreach (var image in bottList)
            image.DOFade(0, .5f);

        yield return new WaitForSeconds(5);

        foreach (var image in topList)
            image.DOFade(1, .5f);

        foreach (var image in bottList)
            image.DOFade(1, .5f);

        yield return new WaitForSeconds(.5f);

        _hintButton.interactable = true;
    }

    void Awake()
    {
        Instance = this;

        _tutorialFinished = GameSaveData.IsTutorialDoneSight();
    }

    // Use this for initialization
    void Start()
    {
        if (DataHelper.Instance.GameMode == GameModes.Easy)
            _difficulty = 0;
        if (DataHelper.Instance.GameMode == GameModes.Normal)
            _difficulty = 1;
        if (DataHelper.Instance.GameMode == GameModes.Hard)
            _difficulty = 2;

        _hitDuratin = _hitDuratinaArr[_difficulty];

        _succeedSign.DOFade(0, 0);

        _hintButton.onClick.AddListener(() =>
        {
            _hintRoutine = StartCoroutine(ShowHint());
        });

        _helpButton.onClick.AddListener(() =>
        {
            HelpsCount++;
            _hitTimer += _hitDuratin / 2;
            if (_hitTimer > _hitDuratin)
                _hitDuratin = _hitTimer;
        });

        Server.Instance.Init();
        Server.Instance.SetAllPictureIds();

        GenerateAlbume();

        var pic = _topAlbume.GetChild(0);
        _circleObj.GetComponent<RectTransform>().sizeDelta =
            new Vector2(pic.GetComponent<RectTransform>().rect.width, pic.GetComponent<RectTransform>().rect.height);
    }


    void ShowGuidCircles(bool isTut)
    {
        if (isTut)
        {
            _tutorialCounter++;
            if (_tutorialCounter > 3)
            {
                _tutorialFinished = true;
                GameSaveData.DoneTutorialSight();
                return;
            }
        }

        foreach (var obj in _guideCirclesList)
            Destroy(obj);
        _guideCirclesList.Clear();

        foreach (var obj in _commonPics)
        {
            var circle = Instantiate(_circleObj, obj.transform.position, _circleObj.transform.rotation, obj.transform);
            var rectTr = circle.GetComponent<RectTransform>();
            rectTr.Rotate(0, 0, Rand(1, 360));
            circle.GetComponent<Image>().color = isTut ? Color.green : Color.black;
            rectTr.gameObject.SetActive(true);
            var seq = DOTween.Sequence();
            seq.Append(rectTr.DOScale(1.1f, .2f));
            seq.Append(rectTr.DOScale(1.0f, .2f));
            seq.SetLoops(isTut ? -1 : 3);
            _guideCirclesList.Add(circle);
        }

    }


    void GenerateAlbume()
    {
        if (_topAlbume != null)
            Destroy(_topAlbume.gameObject);
        if (_bottAlbume != null)
            Destroy(_bottAlbume.gameObject);

        var allbumeObj = _albumeObjects[_difficulty];

        _topAlbume = Instantiate(allbumeObj, transform).GetComponent<RectTransform>();
        _topAlbume.anchoredPosition = new Vector2(700, 310);

        _bottAlbume = Instantiate(allbumeObj, transform).GetComponent<RectTransform>();
        _bottAlbume.anchoredPosition = new Vector2(-700, -175);

        _commonPics.Clear();
        
        
        Server.Instance.RequestAlbume(_difficulty + 1);

        int commonIndex = Server.Instance.ServerData.albume.commonPicId;

        var obj = _topAlbume.GetChild(Rand(0, _topAlbume.childCount)).gameObject;
        obj.GetComponent<Image>().sprite = GameAsset.Instance.AllSprites[commonIndex];
        obj.GetComponent<Button>().onClick.AddListener(() => ButtonClick(obj));
        _commonPics.Add(obj);
        obj.transform.SetSiblingIndex(_topAlbume.childCount - 1);

        obj = _bottAlbume.GetChild(Rand(0, _bottAlbume.childCount)).gameObject;
        obj.GetComponent<Image>().sprite = GameAsset.Instance.AllSprites[commonIndex];
        obj.GetComponent<Button>().onClick.AddListener(() => ButtonClick(obj));
        _commonPics.Add(obj);
        obj.transform.SetSiblingIndex(_bottAlbume.childCount - 1);

        var topPage =  Server.Instance.ServerData.albume.topPage;
        for (int i = 0; i < topPage.picIds.Count; ++i)
        {
            var tr = _topAlbume.GetChild(i);
            tr.rotation = Quaternion.identity;
            tr.Rotate(0, 0, Rand(0, 360));

            int picIndex = topPage.picIds[i];

            tr.GetComponent<Image>().sprite = GameAsset.Instance.AllSprites[picIndex];

            var buttonObj = tr.gameObject;
            tr.GetComponent<Button>().onClick.AddListener(() => ButtonClick(buttonObj));
        }

        var bottomPage =  Server.Instance.ServerData.albume.bottomPage;
        for (int i = 0; i < bottomPage.picIds.Count; ++i)
        {
            var tr = _bottAlbume.GetChild(i);
            tr.rotation = Quaternion.identity;
            tr.Rotate(0, 0, Rand(0, 360));

            int picIndex = bottomPage.picIds[i];
            
            tr.GetComponent<Image>().sprite = GameAsset.Instance.AllSprites[picIndex];
            
            var buttonObj = tr.gameObject;
            tr.GetComponent<Button>().onClick.AddListener(() => ButtonClick(buttonObj));
        }

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        _topAlbume.rotation = Quaternion.identity;
        _bottAlbume.rotation = Quaternion.identity;

        float animTime = .5f;
        _topAlbume.DOAnchorPosX(0, animTime);
        _topAlbume.DORotate(new Vector3(0, 0, Rand(0, 120) / 10f * 30), animTime);

        _bottAlbume.DOAnchorPosX(0, animTime);
        _bottAlbume.DORotate(new Vector3(0, 0, -Rand(0, 120) / 10f * 30), animTime);

        yield return new WaitForSeconds(animTime);


        _hitTimer = _hitDuratin;
        _perventTouch.SetActive(false);

        _hitDuratin -= .5f;
        if (_hitDuratin < _hitDuratinaArr[_difficulty] / 3)
            _hitDuratin = _hitDuratinaArr[_difficulty] / 3;

        if (!_tutorialFinished)
            ShowGuidCircles(true);

        if (!_tutorialFinished && !_firstShowTutorialPage)
        {
            _firstShowTutorialPage = true;
            _tutorialPageObj.SetActive(true);
            _tutorialPageObj.transform.Find("Backg/button").GetComponent<Button>().onClick.AddListener(() =>
            {
                _tutorialPageObj.SetActive(false);
            });
        }
    }

    int Rand(int min, int max)
    {
        return Random.Range(min, max);
    }

    // Update is called once per frame
    void Update()
    {
        CheckTime();
    }

    void CheckTime()
    {
        if (!_tutorialFinished)
            return;

        if (_hitTimer != 0)
        {
            float coef = _hitTimer / _hitDuratin;
            _timerBar.fillAmount = coef;
            _hitTimer -= Time.deltaTime;
            if (_hitTimer <= 0)
            {
                _timerBar.fillAmount = 0;
                FinishGame(2);
                return;
            }
        }
    }

    void ButtonClick(GameObject obj)
    {
        if (_tutorialFinished == false)
        {
            if (_commonPics.Contains(obj))
                FinishGame(0);
            return;
        }

        FinishGame(_commonPics.Contains(obj) ? 0 : 1, obj.GetComponent<RectTransform>());
    }

    /// <summary>
    /// cond == 0 is succeed, 1 is wrong select, 2 is time out
    /// </summary>
    /// <param name="cond">Cond.</param>
    void FinishGame(int cond, RectTransform cellTr = null)
    {
        _hitTimer = 0;

        if (cond == 0)//right
        {
            _perventTouch.SetActive(true);
            if (_hintRoutine != null)
                StopCoroutine(_hintRoutine);
            _hintButton.interactable = true;
            StartCoroutine(GoNextGeneration());
            if (_tutorialFinished)
                SucceedCount++;

            //Right selection sound
        }
        else
        {
            var circleObj = transform.Find("circle").gameObject;
            _failSignTr.sizeDelta = new Vector2(circleObj.GetComponent<RectTransform>().rect.width, circleObj.GetComponent<RectTransform>().rect.height);
            Debug.Log(_failSignTr.sizeDelta.ToString());
            _failSignTr.SetParent(cellTr);
            _failSignTr.anchoredPosition = Vector2.zero;
            _perventTouch.SetActive(true);
            StartCoroutine(ShowFailedPanel(cond == 2));

            //Wrong selection sound
        }
    }

    IEnumerator GoNextGeneration()
    {
        _succeedSign.DOFade(1, .5f);
        _succeedSign.DOFade(0, .5f).SetDelay(.5f);

        float animTime = .5f;
        _topAlbume.DOAnchorPosX(700, animTime);
        _topAlbume.DORotate(new Vector3(0, 0, _topAlbume.rotation.eulerAngles.z - 180), animTime);

        _bottAlbume.DOAnchorPosX(-700, animTime);
        _bottAlbume.DORotate(new Vector3(0, 0, _topAlbume.rotation.eulerAngles.z + 180), animTime);

        yield return new WaitForSeconds(animTime);

        GenerateAlbume();
    }

    IEnumerator ShowFailedPanel(bool timeOut)
    {
        ShowGuidCircles(false);

        yield return new WaitForSeconds(.75f);

        _failedAlert.SetActive(true);
        _failedAlert.transform.Find("timeOut").gameObject.SetActive(timeOut);
        _failedAlert.transform.Find("wrongSelect").gameObject.SetActive(!timeOut);

        yield return new WaitForSeconds(1.25f);

        _failedAlert.SetActive(false);
        _failPanelObj.SetActive(true);
        _perventTouch.SetActive(false);
    }
}