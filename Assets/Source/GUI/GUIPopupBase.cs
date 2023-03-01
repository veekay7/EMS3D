using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public abstract class GUIPopupBase : MonoBehaviour
{
    [HideInInspector]
    public RectTransform m_RectTransform;
    [HideInInspector]
    public Canvas m_Canvas;
    [HideInInspector]
    public CanvasGroup m_CanvasGroup;

    public bool IsVisible { get; protected set; }


    protected virtual void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Canvas = GetComponent<Canvas>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
        
        IsVisible = false;
    }

    public void SetVisible(bool show)
    {
        m_CanvasGroup.alpha = show ? 1.0f : 0.0f;
        m_CanvasGroup.interactable = show;
        m_CanvasGroup.blocksRaycasts = show;
        IsVisible = show;

        if (!show)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected virtual void Update() { return; }

    protected virtual void LateUpdate() { return; }

    protected virtual void OnDestroy() { return; }

    protected virtual void OnValidate()
    {
        if (m_RectTransform == null)
        {
            m_RectTransform = GetComponent<RectTransform>();
            if (m_RectTransform == null)
                m_RectTransform = gameObject.AddComponent<RectTransform>();
        }

        if (m_Canvas == null)
        {
            m_Canvas = GetComponent<Canvas>();
            if (m_Canvas == null)
                m_Canvas = gameObject.AddComponent<Canvas>();
        }

        if (m_CanvasGroup == null)
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            if (m_CanvasGroup == null)
                m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
}
