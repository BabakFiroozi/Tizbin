using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


public class MathBoard : MonoBehaviour
{
    public static MathBoard Instance { get; private set; } = null;

    [SerializeField] Button _nextButton = null;
    [SerializeField] Button _backButton = null;
    [SerializeField] Button _replyButton = null;
    [SerializeField] Button _hintButton = null;

    [SerializeField] GameAsset _gameAsset = null;

    [SerializeField] GameObject _gridContentObj = null;

    [SerializeField] GameObject _cellObj = null;

    [SerializeField] Image _glimpTimerBar = null;

    [SerializeField] float _glimpDuration = 2;

    [SerializeField] Image _succeedBadge = null;

    int _selectsCount = 0;

    List<int> _selectionsList = new List<int>();

    bool _allowSelect = false;

    int _correctCount = 0;

    RectTransform _gridContent = null;

    int _cellsCount = 0;



    // Use this for initialization
    void Start()
    {
        if (DataHelper.Instance.GameMode == GameModes.Easy)
        {
            _cellsCount = 9;
            _glimpDuration = 2;
            _selectsCount = 2;
        }
        if (DataHelper.Instance.GameMode == GameModes.Normal)
        {
            _cellsCount = 16;
            _glimpDuration = 3;
            _selectsCount = 3;
        }
        if (DataHelper.Instance.GameMode == GameModes.Hard)
        {
            _cellsCount = 25;
            _glimpDuration = 4;
            _selectsCount = 4;
        }

        _succeedBadge.color = new Color(1, 1, 1, 0);

        _gridContentObj.SetActive(false);
        _cellObj.SetActive(false);

        StartCoroutine(NextGame(true, false));

        _nextButton.onClick.AddListener(() => 
        {
            if (_allowSelect)
                StartCoroutine(NextGame(false, false));
        });

        _backButton.onClick.AddListener(() =>
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_MAIN_MENU);
        });
        _replyButton.onClick.AddListener(() =>
        {
            SceneTransitor.Instance.TransitScene(SceneTransitor.SCENE_GAME_MATH);
        });
    }


    IEnumerator NextGame(bool first, bool done)
    {
        const float animOffset = 700;
        const float animTime = .3f;

        _allowSelect = false;

        if (!first && done)
        {
            _succeedBadge.DOFade(1, .5f);
            _succeedBadge.DOFade(0, .5f).SetDelay(.5f);
            yield return new WaitForSeconds(.5f);
        }

        if (_gridContent != null)
        {
            _gridContent.DOAnchorPosX(_gridContent.anchoredPosition.x - animOffset, animTime);
            yield return new WaitForSeconds(animTime);
            Destroy(_gridContent.gameObject);
        }

        _correctCount = 0;

        var gridObj = Instantiate(_gridContentObj, transform);
        gridObj.SetActive(true);
        _gridContent = gridObj.GetComponent<RectTransform>();

        int rowSize = (int)Mathf.Sqrt(_cellsCount);

        var gridLayoutGroup = gridObj.GetComponent<GridLayoutGroup>();
        float cellSize = _gridContent.rect.size.x / rowSize;
        cellSize -= (gridLayoutGroup.padding.left * 2) / rowSize;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

        _cellObj.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);

        List<int> cellsList = new List<int>();
        for (int c = 0; c < _cellsCount; ++c)
            cellsList.Add(c);

        List<int> selList = new List<int>();
        foreach (var n in cellsList)
            selList.Add(n);

        do
        {
            int index = Rand(0, selList.Count);
            selList.RemoveAt(index);

        } while (selList.Count > _selectsCount);

        _selectionsList = selList;

        for (int i = 0; i < _cellsCount; ++i)
        {
            var obj = Instantiate(_cellObj, _gridContent);
            obj.SetActive(true);
            var iconObj = obj.transform.Find("pic").gameObject;
            iconObj.SetActive(_selectionsList.Contains(i));
            iconObj.transform.Rotate(0, 0, Rand(0, 360));
            int c = i;
            obj.GetComponent<Button>().onClick.AddListener(() => CellClick(c));

            obj.transform.Find("false").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            obj.transform.Find("true").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        if(!first)
        {
            Vector2 pos = _gridContent.anchoredPosition;
            _gridContent.anchoredPosition += new Vector2(animOffset, 0);
            _gridContent.DOAnchorPosX(pos.x, animTime);
        }

        yield return new WaitForSeconds(animTime);

        StartCoroutine(TwistTable());
    }

    IEnumerator TwistTable()
    {
        _glimpTimerBar.fillAmount = 1;

        _glimpTimerBar.DOFillAmount(0, _glimpDuration).SetEase(Ease.Linear);

        yield return new WaitForSeconds(_glimpDuration);

        foreach (var s in _selectionsList)
        {
            var icon = _gridContent.GetChild(s);
            icon.Find("pic").GetComponent<Image>().DOFade(0, .5f);
        }

        yield return new WaitForSeconds(.5f);

        var seq = DOTween.Sequence();
        seq.Append(_gridContent.DOLocalRotate(new Vector3(0, 0, 90), .5f));
        seq.AppendInterval(.2f);
        seq.Append(_gridContent.DOLocalRotate(new Vector3(0, 0, 0), .5f));
        seq.AppendInterval(.2f);
        seq.Append(_gridContent.DOLocalRotate(new Vector3(0, 0, -90), .5f));

        seq.onComplete = () =>
        {
            _allowSelect = true;
        };
    }


    // Update is called once per frame
    void Update()
    {
    }


    void CellClick(int i)
    {
        if (!_allowSelect)
            return;

        var cellObj = _gridContent.GetChild(i);

        if(_selectionsList.Contains(i))
        {
            var pic = cellObj.Find("pic").GetComponent<Image>();
            pic.DOFade(1, .5f);

            //sound play correct

            _correctCount++;

            if (_selectionsList.Count == _correctCount)
            {
                StartCoroutine(NextGame(false, true));
            }
        }
        else
        {
            var pic = cellObj.Find("false").GetComponent<Image>();
            pic.gameObject.SetActive(true);
            pic.DOFade(1, .3f).onComplete = ()=> { pic.gameObject.SetActive(false); };

            //sound play wrong
        }
    }


    int Rand(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

}
