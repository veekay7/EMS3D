using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class GUIScreen : MonoBehaviour
{
    [HideInInspector]
    public RectTransform m_RectTransform;
    [HideInInspector]
    public Canvas m_Canvas;

    [Header("Screen Base")]
    public string m_Classname;
    public CanvasGroup m_BackgroundGroup;
    public CanvasGroup m_WindowGroup;
    public float m_FadeInDuration;
    public float m_FadeOutDuration;
    public bool m_StartClosed;


    protected virtual void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Canvas = GetComponent<Canvas>();
    }

    protected virtual void Reset()
    {
        m_Classname = "Generic";
        m_FadeInDuration = 0.5f;
        m_FadeOutDuration = 0.35f;
        m_StartClosed = false;
    }

    protected virtual void Start()
    {
    }

    public virtual void Open(UnityAction onFinishAnim = null)
    {
        //gameObject.SetActive(true);
        m_Canvas.enabled = true;

        if (m_BackgroundGroup != null)
        {
            TweenGroupAlpha(m_BackgroundGroup, 1.0f, m_FadeInDuration);
        }

        if (m_WindowGroup != null)
        {
            TweenGroupAlpha(m_WindowGroup, 1.0f, m_FadeInDuration, () => 
            {
                SetInteractable(true);

                OnOpenAnimFinished();

                if (onFinishAnim != null)
                    onFinishAnim();
           });
        }
        else
        {
            SetInteractable(true);

            OnOpenAnimFinished();

            if (onFinishAnim != null)
                onFinishAnim();
        }
    }

    public virtual void Close(UnityAction onFinishAnim = null)
    {
        SetInteractable(false);

        if (m_BackgroundGroup != null)
        {
            TweenGroupAlpha(m_BackgroundGroup, 0.0f, m_FadeOutDuration);
        }

        if (m_WindowGroup != null)
        {
            TweenGroupAlpha(m_WindowGroup, 0.0f, m_FadeOutDuration, () => 
            {
                OnCloseAnimFinished();

                if (onFinishAnim != null)
                    onFinishAnim.Invoke();

                m_Canvas.enabled = false;
                //gameObject.SetActive(false);
            });
        }
        else
        {
            OnCloseAnimFinished();

            m_Canvas.enabled = false;
            //gameObject.SetActive(false);
        }
    }

    protected virtual void OnOpenAnimFinished() { }

    protected virtual void OnCloseAnimFinished() { }

    private void TweenGroupAlpha(CanvasGroup group, float alpha, float fadeDuration, TweenCallback onFinish = null)
    {
        var tween = DOTween.To(() => group.alpha, (a) => group.alpha = a, alpha, fadeDuration);
        tween.SetId("fade");
        
        if (onFinish != null)
            tween.OnComplete(onFinish);
    }

    private void SetGroupVisibility(CanvasGroup group, bool visible)
    {
        group.alpha = visible ? 1.0f : 0.0f;
    }

    public void SetInteractable(bool value)
    {
        if (m_WindowGroup == null)
            return;

        m_WindowGroup.interactable = value;
        m_WindowGroup.blocksRaycasts = value;
    }

    protected virtual void OnDestroy()
    {
    }

    protected virtual void OnValidate()
    {
        m_RectTransform = gameObject.GetOrAddComponent<RectTransform>();
        m_Canvas = gameObject.GetOrAddComponent<Canvas>();

        // visibility settings
        if (m_BackgroundGroup != null)
            SetGroupVisibility(m_BackgroundGroup, !m_StartClosed);

        if (m_WindowGroup != null)
            SetGroupVisibility(m_WindowGroup, !m_StartClosed);

        SetInteractable(!m_StartClosed);

        m_Canvas.enabled = !m_StartClosed;
    }
}
