using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;


public class ConfirmScreen : MonoBehaviour
{
    public enum ConfirmTypes
    {
        Ok, Cancel, Close
    }


    [SerializeField] Button _closeButton = null;
    [SerializeField] Button _okButton = null;
    [SerializeField] Button _cancelButton = null;

    [SerializeField] RectTransform _framRectTr = null;

    [SerializeField] bool _closeWithBackg = true;


    Image _backgImage = null;


    public Action<ConfirmTypes> ConfirmedEvent { get; set; }

    public void OffOkButton(bool off)
    {
        _okButton.interactable = !off;
    }

    // Use this for initialization
    void Start()
    {
        _backgImage = GetComponent<Image>();

        var closeBackg = _backgImage.gameObject.GetComponent<Button>();
        closeBackg.onClick.AddListener(CloseButtonClick);
        _closeButton.onClick.AddListener(CloseButtonClick);
        _okButton.onClick.AddListener(OkButtonClick);
        _cancelButton.onClick.AddListener(CancelButtonClick);

        closeBackg.interactable = _closeWithBackg;

        Close();
    }


    public void Close()
    {
        _backgImage.enabled = false;
        _backgImage.raycastTarget = false;
        _framRectTr.gameObject.SetActive(false);
    }

    void CancelButtonClick()
    {
        ConfirmedEvent?.Invoke(ConfirmTypes.Cancel);
        Close();
    }

    void CloseButtonClick()
    {
        ConfirmedEvent?.Invoke(ConfirmTypes.Close);
        Close();
    }

    void OkButtonClick()
    {
        _okButton.interactable = false;
        ConfirmedEvent?.Invoke(ConfirmTypes.Ok);        
    }


    public void ShowConfirm(bool withAnim = true)
    {
        _okButton.interactable = true;
        _backgImage.enabled = true;
        _backgImage.raycastTarget = true;
        _framRectTr.gameObject.SetActive(true);
        transform.SetAsLastSibling();

        if (withAnim)
        {
            float animTime = .3f;
            _backgImage.DOFade(0, 0);
            _backgImage.DOFade(.7f, animTime);
            _framRectTr.localScale = Vector3.one * .3f;
            _framRectTr.DOScale(Vector3.one, animTime).SetEase(Ease.OutBounce);
        }
        else
        {
            _framRectTr.localScale = Vector3.one;
        }

        //SoundManager.Instance.PlaySound(SoundNames.PopupConfirm);
    }
}
