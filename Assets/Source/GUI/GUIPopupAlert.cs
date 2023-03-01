using UnityEngine;
using UnityEngine.Events;
using UGUI = UnityEngine.UI;

[DisallowMultipleComponent]
public class GUIPopupAlert : GUIPopupboxBase
{
    public UGUI.Button m_BtnOk;


    protected override void Awake()
    {
        base.Awake();
        m_BtnOk.onClick.AddListener(OkButtonPressedCallback);
    }

    protected override void HandleKbInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            OkButtonPressedCallback();
    }

    public void Open(string messageString, string buttonString = null, UnityAction<int> onOkCallback = null)
    {
        SetMessage(messageString);
        if (!string.IsNullOrEmpty(buttonString))
            m_BtnOk.GetComponent<RectTransform>().GetChild(0).GetComponent<TMPro.TMP_Text>().text = buttonString;
        responseFunc = onOkCallback;
    }

    private void OkButtonPressedCallback()
    {
        if (responseFunc != null)
            responseFunc.Invoke(0);
        SetVisible(false);
    }
}
