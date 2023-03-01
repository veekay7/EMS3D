using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UGUI = UnityEngine.UI;

public class GUIDialogueBox01 : GUIPopupBase
{
    public const int MAX_CHAR = 255;

    public TMPro.TMP_Text m_TxtMsg;
    public UGUI.Button m_BtnContinue;

    private bool m_allowInput;
    private bool m_isPrinting;
    private int m_curPage;
    private int m_numMsgs;
    private int m_curPrintIdx;
    private float m_nextCharPrintTime;
    private string m_printmsg;
    private List<string> m_messages = new List<string>();
    private UnityAction onFinishFunc;


    protected override void Awake()
    {
        base.Awake();

        m_allowInput = true;
        m_numMsgs = 0;
        m_curPage = -1;
        m_curPrintIdx = 0;
        m_nextCharPrintTime = 0.0f;
        onFinishFunc = null;

        m_TxtMsg.text = string.Empty;
        m_BtnContinue.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (m_allowInput)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                if (m_isPrinting)
                {
                    // skip print and show everything
                    m_TxtMsg.text = m_printmsg;
                    m_isPrinting = false;
                    m_BtnContinue.gameObject.SetActive(true);
                }
                else
                {
                    // go to the next page
                    m_curPage++;

                    // if reached the final page and we try to go on, then we can get outta here and close!
                    if (m_curPage > m_numMsgs - 1)
                    {
                        m_curPage = m_numMsgs - 1;

                        if (onFinishFunc != null)
                            onFinishFunc.Invoke();

                        SetVisible(false);

                        return;
                    }

                    m_printmsg = m_messages[m_curPage];
                    m_TxtMsg.text = string.Empty;
                    m_curPrintIdx = 0;
                    m_isPrinting = true;
                }
            }
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        // print the text
        if (m_isPrinting)
        {
            if (Time.time > m_nextCharPrintTime)
            {
                char nextch = m_printmsg[m_curPrintIdx];
                m_TxtMsg.text += nextch;
                m_curPrintIdx++;

                float nextTime = 1.0f / Globals.m_UserGameConfig.m_PrintSpd;
                m_nextCharPrintTime = Time.time + nextTime;

                if (m_curPrintIdx == m_printmsg.Length)
                {
                    m_BtnContinue.gameObject.SetActive(true);
                    m_curPrintIdx = 0;
                    m_isPrinting = false;
                }
            }
        }
    }

    /// <summary>
    /// Opens the dialogue box and immediately prints the fucking message.
    /// </summary>
    /// <param name="msg">Put your entire fucking message here and the dialogue box will paginate for you</param>
    public void Open(string msg, UnityAction onFinishCallback)
    {
        // get total number of characters in msg
        int msglen = msg.Length;
        m_numMsgs = Mathf.CeilToInt(msglen / (float)MAX_CHAR);

        for (int i = 0; i < m_numMsgs; i++)
        {
            int startidx = i * (MAX_CHAR - 1);
            int len = Mathf.Min(startidx + (MAX_CHAR), msglen) - startidx;
            string substr = msg.Substring(startidx, len);

            m_messages.Add(substr);
        }

        m_curPage = 0;
        m_printmsg = m_messages[m_curPage];
        m_isPrinting = true;
        m_TxtMsg.text = string.Empty;
        onFinishFunc = onFinishCallback;

        SetVisible(true);
    }
}
