using UnityEditor;
using UnityEngine;

namespace E3D
{
    [DisallowMultipleComponent]
    public class HudBaseView : MonoBehaviour
    {
        [HideInInspector]
        public RectTransform m_RectTransform;
        [HideInInspector]
        public CanvasGroup m_CanvasGroup;

        [Header("HUD View Base")]
        public bool m_StartClosed;

        public Hud Owner;


        protected virtual void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual void Reset()
        {
            m_StartClosed = false;
        }

        protected virtual void Start()
        {
            //SetVisible(!m_StartClosed);
            //SetInteractable(!m_StartClosed);
        }

        public virtual void Open()
        {
            SetVisible(true);
            SetInteractable(true);
        }

        public virtual void Close()
        {
            SetInteractable(false);
            SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            m_CanvasGroup.alpha = visible ? 1.0f : 0.0f;
        }

        public void SetInteractable(bool value)
        {
            m_CanvasGroup.interactable = value;
            m_CanvasGroup.blocksRaycasts = value;
        }

        protected virtual void OnValidate()
        {
            m_RectTransform = gameObject.GetOrAddComponent<RectTransform>();
            m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            SetVisible(!m_StartClosed);
            SetInteractable(!m_StartClosed);
        }
    }
}
