using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UGUI = UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(UGUI.Image))]
public class ScreenWiper : SingletonBehaviour<ScreenWiper>
{
    public enum FillMode
    {
        None,
        Fill,     // The blocking image will fill the screen.
        Clear     // The blocking image will clear and reveal everything underneath.
    }

    public Canvas m_Canvas;
    public UGUI.Image m_Img;
    public CanvasGroup m_CanvasGroup;
    public bool m_ClearOnAwake;

    private bool m_isBusy;
    private FillMode m_currentTweenMode;
    private static Color s_baseColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    private static Color s_transparentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    public bool IsFilled { get; private set; }

    public FillMode CurrentTweenMode { get => m_currentTweenMode; }


    protected override void AfterAwake()
    {
        m_Canvas = GetComponent<Canvas>();
        m_Img = GetComponent<UGUI.Image>();
        m_CanvasGroup = GetComponent<CanvasGroup>();

        m_currentTweenMode = FillMode.None;
        m_isBusy = false;
        IsFilled = false;

        if (m_ClearOnAwake)
        {
            SetFilled(false);
        }
    }

    // editor only
    private void Reset()
    {
        if (m_Img != null)
        {
            m_Img.color = s_baseColor;
        }
    }

    /// <summary>
    /// Do a simple screen black fade wipe.
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="time"></param>
    /// <param name="delay"></param>
    /// <param name="onComplete"></param>
    public void DoFade(FillMode mode, float time = 1.0f, float delay = 0.0f, UnityAction onComplete = null)
    {
        if (m_isBusy)
            return;

        m_isBusy = true;
        m_Img.type = UGUI.Image.Type.Simple;
        Color targetColor = s_transparentColor;

        // Set up tween parameters.
        switch (mode)
        {
            case FillMode.Fill:
                targetColor = s_baseColor;
                SetFilled(false);
                break;

            case FillMode.Clear:
                targetColor = s_transparentColor;
                SetFilled(true);
                break;
        }

        // Do tween.
        var m_tween = DOTween.To(
                        () => m_Img.color,
                        (color) => m_Img.color = color,
                        targetColor,
                        time);
        m_tween.SetId("screenwiper");
        m_tween.SetEase(Ease.Linear);
        m_tween.SetDelay(delay);
        m_currentTweenMode = FillMode.Fill;
        m_tween.OnComplete(() => OnCompleteTween(onComplete));
    }

    /// <summary>
    /// Forces a screen wipe to be killed.
    /// </summary>
    public void ForceKillFade()
    {
        int tweensCompleted = DOTween.Complete("screenwiper");
        Debug.Log("Tweens completed: " + tweensCompleted);
    }

    /// <summary>
    /// Is the screen wiper doing a wipe right now?
    /// </summary>
    /// <returns></returns>
    public bool IsTweening()
    {
        return m_isBusy;
    }

    /// <summary>
    /// Set the screen wiper to fill/clear the screen.
    /// </summary>
    /// <param name="filled"></param>
    public void SetFilled(bool filled)
    {
        m_Img.color = filled ? s_baseColor : s_transparentColor;
        IsFilled = filled;
    }

    private void OnCompleteTween(UnityAction onComplete)
    {
        m_isBusy = false;
        if (onComplete != null)
            onComplete.Invoke();
    }

    private void OnValidate()
    {
        m_Canvas = gameObject.GetOrAddComponent<Canvas>();
        m_Img = gameObject.GetOrAddComponent<UGUI.Image>();
        m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

        if (m_Img != null)
            SetFilled(!m_ClearOnAwake);
    }
}