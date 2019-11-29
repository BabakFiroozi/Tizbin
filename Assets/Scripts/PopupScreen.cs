using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;



public class PopupScreen : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] float _animShowTime = 2;
    [SerializeField] float _animMoveTime = .5f;
    [SerializeField] Vector2 _animMoveDirection = new Vector2(0, 1);
    [Space(10)]
    


    [SerializeField] bool _permenant = false;
    [SerializeField] float _transparency = .7f;
    [SerializeField] RectTransform _frameRectTr = null;
    [SerializeField] Button _closeButton = null;

    [SerializeField] Transform _overSbilingIndex = null;

    [SerializeField] bool _logBusy = true;


    Vector2 _animOffset;

    Image _backgImage;

    Vector2 _initPos;

    public System.Action ShowEvent { get; set; }

    public System.Action ShowedEvent { get; set; }

    public System.Action ShowFinishedEvent { get; set; }

    bool _hide = false;

    Vector2 _framehidePos;

    public bool IsBusy
    {
        get; private set;
    }


    // Use this for initialization
    void Start()
    {

        var rect = GetComponent<RectTransform>().rect;
        _animOffset = _animMoveDirection.normalized * new Vector2(rect.width, rect.height);

        _backgImage = GetComponent<Image>();
        _frameRectTr.gameObject.SetActive(false);
        _initPos = _frameRectTr.anchoredPosition;

        var closeBackg = _backgImage.gameObject.GetComponent<Button>();

        if (closeBackg != null)
            closeBackg.onClick.AddListener(ClosePopup);

        if (_closeButton != null)
            _closeButton.onClick.AddListener(ClosePopup);


        //Hide popup first
        _backgImage.enabled = false;
        _framehidePos = _initPos + _animOffset;
        _frameRectTr.anchoredPosition = _framehidePos;

    }


    void ClosePopup()
    {
        if (!_permenant)
            return;

        HidePopup();
    }


    public void ShowPopup(bool withAnim = true)
    {
        if (_overSbilingIndex == null)
            transform.SetAsLastSibling();
        else
            transform.SetSiblingIndex(_overSbilingIndex.GetSiblingIndex() - 1);

        if (_backgImage.enabled == false)
            _backgImage.enabled = true;

        if (IsBusy)
        {
            if (_logBusy)
                Debug.LogWarning(gameObject.name + " PopupScreen is busy.");
            return;
        }       

        _hide = false;
        _frameRectTr.gameObject.SetActive(true);
        StartCoroutine(Show(withAnim));
    }

    public void HidePopup()
    {
        _hide = true;
    }


    IEnumerator Show(bool anim)
    {
        IsBusy = true;

        float animTime = _animMoveTime;        

        _backgImage.raycastTarget = true;

        ShowEvent?.Invoke();

        //show
        if (anim)
        {
            _frameRectTr.anchoredPosition = _framehidePos;
            _frameRectTr.DOAnchorPos(_initPos, animTime);
            _backgImage.DOFade(0, 0);
            _backgImage.DOFade(_transparency, animTime).onComplete = () => { ShowedEvent?.Invoke(); };
        }
        else
        {
            var col = _backgImage.color;
            _backgImage.color = new Color(col.r, col.g, col.b, _transparency);
            _frameRectTr.anchoredPosition = _initPos;
        }

        //SoundManager.Instance.PlaySound(SoundNames.PopupShow);


        if (_permenant)
            yield return new WaitUntil(() => _hide);
        else
            yield return new WaitForSeconds(_animShowTime + animTime);


        //hide
        if (anim)
        {
            _frameRectTr.DOAnchorPos(_framehidePos, animTime);
            _backgImage.DOFade(0, animTime);
        }
        else
        {
            var col = _backgImage.color;
            _backgImage.color = new Color(col.r, col.g, col.b, 0);
            _frameRectTr.anchoredPosition = _framehidePos;
        }

        //SoundManager.Instance.PlaySound(SoundNames.PopupHide);

        yield return new WaitForSeconds(animTime);

        _frameRectTr.gameObject.SetActive(false);

        _backgImage.raycastTarget = false;

        ShowFinishedEvent?.Invoke();

        IsBusy = false;
    }

}
