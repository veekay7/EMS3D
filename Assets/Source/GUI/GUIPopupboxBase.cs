using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class GUIPopupboxBase : GUIPopupBase
{
    public const int MAX_CHAR = 260;

    public TMPro.TMP_InputField m_TxtMsg;

    protected bool m_allowInput;
    protected UnityAction<int> responseFunc;


    protected override void Awake()
    {
        base.Awake();
        m_allowInput = false;
    }

    protected override void Update()
    {
        if (m_allowInput)
        {
            HandleKbInput();
        }
    }

    protected virtual void HandleKbInput() { return; }

    protected void SetMessage(string message)
    {
        StringBuilder sb = new StringBuilder(message, MAX_CHAR);
        m_TxtMsg.text = sb.ToString();
    }
}
