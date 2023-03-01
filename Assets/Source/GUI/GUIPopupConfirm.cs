using UnityEngine;
using UnityEngine.Events;
using UGUI = UnityEngine.UI;

[DisallowMultipleComponent]
public class GUIPopupConfirm : GUIPopupboxBase
{
    public UGUI.Button m_BtnOk;
    public UGUI.Button m_BtnCancel;


    protected override void Awake()
    {
        base.Awake();
        m_BtnOk.onClick.AddListener(OkButtonPressedCallback);
        m_BtnCancel.onClick.AddListener(CancelButtonPressedCallback);
    }

    public void Open(string messageString, string okBtnString = null, string cancelBtnString = null, UnityAction<int> callback = null)
    {
        SetMessage(messageString);

        if (!string.IsNullOrEmpty(okBtnString))
            m_BtnOk.GetComponent<RectTransform>().GetChild(0).GetComponent<TMPro.TMP_Text>().text = okBtnString;

        if (!string.IsNullOrEmpty(cancelBtnString))
            m_BtnCancel.GetComponent<RectTransform>().GetChild(0).GetComponent<TMPro.TMP_Text>().text = cancelBtnString;

        responseFunc = callback;
    }

    protected override void HandleKbInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            OkButtonPressedCallback();
        else if (Input.GetKeyDown(KeyCode.Escape))
            CancelButtonPressedCallback();
    }

    private void OkButtonPressedCallback()
    {
        if (responseFunc != null)
            responseFunc.Invoke(0);
        SetVisible(false);
    }

    private void CancelButtonPressedCallback()
    {
        if (responseFunc != null)
            responseFunc.Invoke(1);
        SetVisible(false);
    }
}
