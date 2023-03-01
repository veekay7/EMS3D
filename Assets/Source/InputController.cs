using System.Collections.Generic;
using UnityEngine;

public interface IInputCtrlEventListener
{
    void Input_OnMouseButtonClick(MouseInfo mouseInfo);
    void Input_OnMouseButtonHold(MouseInfo mouseInfo);
    void Input_OnMouseButtonDown(MouseInfo mouseInfo);
    void Input_OnMouseButtonUp(MouseInfo mouseInfo);
    void Input_OnMouseMove(Vector3 newMousePos, Vector3 oldMousePos, Vector3 deltaMousePos);
    void Input_OnMouseBeginDrag(MouseInfo mouseInfo);
    void Input_OnMouseDragged(MouseInfo mouseInfo);
    void Input_OnMouseEndDrag(MouseInfo mouseInfo);
}

/// <summary>
/// MouseInfo
/// </summary>
public class MouseInfo
{
    public int m_ButtonId;
    public Vector3 m_NewPosition;
    public Vector3 m_OldPosition;
    public Vector3 m_Delta;
    public Vector3 m_StartPosition;
    public float m_OnClickTime;
    public bool m_IsOverUI;
    public bool m_IsHolding;
    public bool m_IsDrag;


    /// <summary>
    /// Resets the state of the mouse info.
    /// </summary>
    public void Reset()
    {
        m_ButtonId = -1;
        m_NewPosition = Vector3.zero;
        m_OldPosition = Vector3.zero;
        m_Delta = Vector3.zero;
        m_OnClickTime = 0.0f;
        m_IsOverUI = false;
        m_IsHolding = false;
        m_IsDrag = false;
    }
}


/// <summary>
/// Input controller
/// </summary>
public class InputController : SingletonBehaviour<InputController>
{
    private Vector3 m_newMousePos;
    private Vector3 m_oldMousePos;
    private Vector3 m_mousePosDelta;
    private MouseInfo[] m_mouseInfo;
    private List<IInputCtrlEventListener> m_listeners = new List<IInputCtrlEventListener>();

    public bool EnableInput { get; set; }

    public IInputCtrlEventListener[] Listeners { get => m_listeners.ToArray(); }


    protected override void AfterAwake()
    {
        base.AfterAwake();

        m_newMousePos = Vector2.zero;
        m_oldMousePos = Vector2.zero;
        m_mousePosDelta = Vector2.zero;
        m_mouseInfo = new MouseInfo[2];
        for (int i = 0; i < m_mouseInfo.Length; i++)
        {
            m_mouseInfo[i] = new MouseInfo();
        }

        EnableInput = true;
    }

    private void Update()
    {
        if (EnableInput)
        {
            Update_MouseInput();
        }
    }

    public void AddListener(IInputCtrlEventListener newListener)
    {
        if (!m_listeners.Contains(newListener))
            m_listeners.Add(newListener);
    }

    public void RemoveListener(IInputCtrlEventListener listener)
    {
        if (m_listeners.Contains(listener))
            m_listeners.Remove(listener);
    }

    private void Update_MouseInput()
    {
        m_oldMousePos = m_newMousePos;
        m_newMousePos = Input.mousePosition;
        m_mousePosDelta = m_newMousePos - m_oldMousePos;

        /* Mouse moved */
        if (m_mousePosDelta.sqrMagnitude > Mathf.Epsilon)
            Input_OnMouseMove(m_newMousePos, m_oldMousePos, m_mousePosDelta);

        /* Mouse buttons */
        for (int i = 0; i < 2; i++)
        {
            MouseInfo mouseInfo = m_mouseInfo[i];
            mouseInfo.m_ButtonId = i;
            mouseInfo.m_OldPosition = m_oldMousePos;
            mouseInfo.m_NewPosition = m_newMousePos;
            mouseInfo.m_Delta = m_mousePosDelta;

            if (Input.GetMouseButtonDown(i))
            {
                // When left mouse button is down, we store the time and the initial click position
                mouseInfo.m_StartPosition = Input.mousePosition;
                mouseInfo.m_OnClickTime = Time.realtimeSinceStartup;
                mouseInfo.m_IsOverUI = Utils.Input_IsPointerOnGUI();

                // Reset some mouse info shit.
                mouseInfo.m_IsHolding = false;
                mouseInfo.m_IsDrag = false;
                mouseInfo.m_IsHolding = false;

                Input_OnMouseButtonDown(mouseInfo);
            }
            else if (Input.GetMouseButton(i))
            {
                // I'm holding my mouse button down.
                if (Time.realtimeSinceStartup - mouseInfo.m_OnClickTime >= Consts.MOUSEBTN_HOLD_TIME)
                {
                    mouseInfo.m_IsHolding = true;
                    Input_OnMouseButtonHold(mouseInfo);
                }

                // If the mouse button was held down and we have moved, then we are dragging.
                if (mouseInfo.m_IsHolding && mouseInfo.m_Delta.magnitude > Consts.KINDA_SMALL_NUMBER)
                {
                    // If I was not dragging before, I have just begun to drag!
                    bool wasDragged = mouseInfo.m_IsDrag;
                    mouseInfo.m_IsDrag = true;
                    if (!wasDragged)
                        Input_OnMouseBeginDrag(mouseInfo);

                    // Otherwise, we are in the middle of draggin' balls.
                    Input_OnMouseDragged(mouseInfo);
                }
            }

            if (Input.GetMouseButtonUp(i))
            {
                if (Time.realtimeSinceStartup - mouseInfo.m_OnClickTime <= Consts.MOUSEBTN_CLICK_TIME)
                    Input_OnMouseButtonClick(mouseInfo);

                if (mouseInfo.m_IsDrag)
                {
                    mouseInfo.m_IsHolding = false;
                    mouseInfo.m_IsDrag = false;
                    Input_OnMouseEndDrag(mouseInfo);
                }

                Input_OnMouseButtonUp(mouseInfo);
            }
        }
    }

    private void Input_OnMouseButtonClick(MouseInfo mouseInfo)
    {
        for (int i = 0; i < Listeners.Length; i++)
        {
            Listeners[i].Input_OnMouseButtonClick(mouseInfo);
        }
    }

    private void Input_OnMouseButtonHold(MouseInfo mouseInfo)
    {
        for (int i = 0; i < Listeners.Length; i++)
        {
            Listeners[i].Input_OnMouseButtonHold(mouseInfo);
        }
    }

    private void Input_OnMouseButtonDown(MouseInfo mouseInfo)
    {
        for (int i = 0; i < Listeners.Length; i++)
        {
            Listeners[i].Input_OnMouseButtonDown(mouseInfo);
        }
    }

    private void Input_OnMouseButtonUp(MouseInfo mouseInfo)
    {
        for (int i = 0; i < Listeners.Length; i++)
        {
            Listeners[i].Input_OnMouseButtonUp(mouseInfo);
        }
    }

    private void Input_OnMouseMove(Vector3 newMousePos, Vector3 oldMousePos, Vector3 deltaMousePos)
    {
        for (int i = 0; i < Listeners.Length; i++)
        {
            Listeners[i].Input_OnMouseMove(newMousePos, oldMousePos, deltaMousePos);
        }
    }

    private void Input_OnMouseBeginDrag(MouseInfo mouseInfo)
    {
        for (int i = 0; i < Listeners.Length; i++)
        {
            Listeners[i].Input_OnMouseBeginDrag(mouseInfo);
        }
    }

    private void Input_OnMouseDragged(MouseInfo mouseInfo)
    {
        for (int i = 0; i < Listeners.Length; i++)
        {
            Listeners[i].Input_OnMouseDragged(mouseInfo);
        }
    }

    private void Input_OnMouseEndDrag(MouseInfo mouseInfo)
    {
        for (int i = 0; i < Listeners.Length; i++)
        {
            Listeners[i].Input_OnMouseEndDrag(mouseInfo);
        }
    }
}
