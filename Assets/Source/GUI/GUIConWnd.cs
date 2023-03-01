using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(Canvas))]
public class GUIConWnd : MonoBehaviour
{
    [HideInInspector]
    public Canvas m_Canvas;

    public CanvasGroup m_ConWndPanelGroup;
    public TMP_Text m_TxtCmdBuffer;
    public TMP_InputField m_TxtBoxCmd;
    public Button m_BtnSubmit;
    public Button m_BtnHide;

    public int m_FontSize;
    public float m_VisChangeDuration;
    public Color m_TxtFieldColor;
    public Color m_BufferColor;

    private bool m_visibilityChanging;
    private DevCmdSys m_cmdSysCtrl;

    public bool IsVisible { get; private set; }


    private void Awake()
    {
        m_Canvas = GetComponent<Canvas>();

        m_visibilityChanging = false;
        m_cmdSysCtrl = null;

        IsVisible = false;
    }

    private void Reset()
    {
        m_FontSize = 16;
        m_VisChangeDuration = 0.25f;
        m_TxtFieldColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        m_BufferColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void Start()
    {
        SetVisibility(false);

        m_TxtBoxCmd.onEndEdit.AddListener(TxtBoxCmd_EndEdit);
        m_BtnSubmit.onClick.AddListener(BtnSubmit_Click);
        m_BtnHide.onClick.AddListener(BtnHide_Click);
    }

    public void SetCmdSystem(DevCmdSys cmdsysCtrl)
    {
        if (m_cmdSysCtrl != null)
        {
            m_TxtBoxCmd.onValueChanged.RemoveListener(m_cmdSysCtrl.UpdateInputText);
        }

        m_cmdSysCtrl = cmdsysCtrl;
        if (m_cmdSysCtrl != null)
        {
            m_TxtBoxCmd.onValueChanged.AddListener(m_cmdSysCtrl.UpdateInputText);
        }
    }

    public void SetVisibility(bool value)
    {
        if (m_ConWndPanelGroup != null)
        {
            m_ConWndPanelGroup.alpha = value ? 1.0f : 0.0f;
            m_ConWndPanelGroup.interactable = value;
            m_ConWndPanelGroup.blocksRaycasts = value;
        }

        IsVisible = value;
        gameObject.SetActive(value);
    }

    public void Show()
    {
        if (m_visibilityChanging || IsVisible)
            return;

        gameObject.SetActive(true);

        var tween = DOTween.To(() => m_ConWndPanelGroup.alpha, (a) => m_ConWndPanelGroup.alpha = a, 1.0f, m_VisChangeDuration);
        tween.OnComplete(() =>
        {
            m_ConWndPanelGroup.interactable = false;
            m_ConWndPanelGroup.blocksRaycasts = false;
            IsVisible = true;
        });
    }

    public void Hide()
    {
        if (m_visibilityChanging || !IsVisible)
            return;

        var tween = DOTween.To(() => m_ConWndPanelGroup.alpha, (a) => m_ConWndPanelGroup.alpha = a, 0.0f, m_VisChangeDuration);
        tween.OnComplete(() =>
        {
            m_ConWndPanelGroup.interactable = false;
            m_ConWndPanelGroup.blocksRaycasts = false;
            IsVisible = false;

            gameObject.SetActive(false);
        });
    }

    public void ClearInputText()
    {
        m_TxtBoxCmd.text = string.Empty;
    }

    private void BtnHide_Click()
    {
        Hide();
    }

    private void BtnSubmit_Click()
    {
        if (m_cmdSysCtrl == null)
            return;
    }

    private void TxtBoxCmd_EndEdit(string arg0)
    {
        Debug.Log("Entered cmd: " + arg0);
    }

    private void OnValidate()
    {
        m_Canvas = gameObject.GetOrAddComponent<Canvas>();
    }
}
