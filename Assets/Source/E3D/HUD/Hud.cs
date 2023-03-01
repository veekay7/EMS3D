using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace E3D
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
    public class Hud : MonoBehaviour
    {
        [HideInInspector]
        public RectTransform m_RectTransform;
        [HideInInspector]
        public Canvas m_Canvas;

        [Header("HUD")]
        public GUIGameStateDisplay m_GameState;
        public List<HudBaseView> m_CachedViews = new List<HudBaseView>();
        public HudBaseView m_StartView;

        protected HudBaseView m_curView;
        

        public E3DPlayer Player { get; private set; }

        protected virtual void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_Canvas = GetComponent<Canvas>();

            m_curView = null;
        }

        protected virtual void Reset() { return; }

        protected virtual void Start()
        {
            for (int i = 0; i < m_CachedViews.Count; i++)
            {
                m_CachedViews[i].Owner = this;
            }

            if (m_StartView != null)
                OpenView(m_StartView);
        }

        public virtual void SetPlayer(E3DPlayer newPlayer)
        {
            Player = newPlayer;
            if (m_GameState != null)
                m_GameState.Player = Player;
        }

        public virtual void UnsetPlayer()
        {
            Player = null;
        }

        public void SetVisible(bool value)
        {
            m_Canvas.enabled = value;
        }

        public void OpenView(HudBaseView newView)
        {
            if (m_curView == newView)
                return;

            newView.gameObject.SetActive(true);

            newView.transform.SetAsLastSibling();

            // close the old panel
            CloseCurrentView();

            m_curView = newView;

            // open the new panel
            m_curView.Open();
        }

        public void CloseCurrentView()
        {
            if (m_curView == null)
                return;

            m_curView.Close();

            m_curView.gameObject.SetActive(false);

            m_curView = null;
        }

        public void PauseGame()
        {
            if (Player == null)
                return;

            Player.Pause();
        }

        public void OpenBriefingScreen()
        {
            if (Player == null)
                return;

            Player.OpenBriefingScreen();
        }

        public void ShowTextBoxPrompt(string outputString, UnityAction dlgBoxResponseFunc = null)
        {
            if (Player != null)
            {
                Player.IsWaitingForPrompt = true;

                var box = PopupboxFactory.Instance.Create<GUIDialogueBox01>(m_RectTransform);
                
                box.m_RectTransform.SetAsLastSibling();

                box.Open(outputString, () => {

                    if (dlgBoxResponseFunc != null)
                        dlgBoxResponseFunc.Invoke();

                    if (Player != null)
                        Player.IsWaitingForPrompt = false;
                });
            }
        }

        protected virtual void OnDestroy()
        {
        }

        // editor only
        protected virtual void OnValidate()
        {
            m_RectTransform = gameObject.GetOrAddComponent<RectTransform>();
            m_Canvas = gameObject.GetOrAddComponent<Canvas>();
        }
    }
}
