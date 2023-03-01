using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.Events;

namespace E3D
{
    public class GUIBriefingScreen : GUIScreen
    {
        [Header("Briefing Screen")]
        public VideoPlayer m_VideoPlayer;
        public VideoClip m_NullVideoClip;
        public RawImage m_VideoRawImg;
        public Button m_BtnPlay;
        public Button m_BtnStop;
        public Button m_BtnPause;
        public TMP_Text m_TxtObjectives;
        

        protected override void Awake()
        {
            base.Awake();

            m_Classname = "Briefing";
            m_VideoPlayer.clip = m_NullVideoClip;
            m_VideoPlayer.isLooping = true;
            m_BtnPlay.interactable = false;
            m_BtnStop.interactable = false;
            m_BtnPause.interactable = false;
            m_TxtObjectives.text = "No objectives specified.";
        }

        protected override void Reset()
        {
            base.Reset();

            m_Classname = "Briefing";
        }

        protected override void Start()
        {
            base.Start();
            PlayVid();
        }

        public override void Open(UnityAction onFinishAnim = null)
        {
            if (Globals.m_CurrentMap != null)
            {
                m_VideoPlayer.clip = Globals.m_CurrentMap.m_SceneVideoClip;
                m_VideoPlayer.Stop();
                m_VideoPlayer.Play();

                if (Globals.m_CurrentMap.m_ObjectivesDesc != null)
                    m_TxtObjectives.text = Globals.m_CurrentMap.m_ObjectivesDesc.text;
            }

            base.Open(onFinishAnim);
        }

        public void ReadyAndOrClose()
        {
            if (GameState.Current != null && E3DPlayer.Local != null && GUIController.Instance != null)
            {
                if (GameState.Current.m_CurrentMatchState == EMatchState.Enter)
                    E3DPlayer.Local.SetReady();

                if (GUIController.Instance.CurrentActiveScreen == this)
                    GUIController.Instance.CloseCurrentScreen();
            }
        }

        public void PauseVid()
        {
            m_VideoPlayer.Pause();
            m_BtnPlay.interactable = true;
            m_BtnStop.interactable = true;
            m_BtnPause.interactable = false;
        }

        public void StopVid()
        {
            m_VideoPlayer.Stop();
            m_BtnPlay.interactable = true;
            m_BtnStop.interactable = false;
            m_BtnPause.interactable = false;
        }

        public void PlayVid()
        {
            m_VideoPlayer.Play();
            m_BtnPlay.interactable = false;
            m_BtnStop.interactable = true;
            m_BtnPause.interactable = true;
        }

        protected override void OnDestroy()
        {
            if (m_VideoRawImg.texture != null)
            {
                RenderTexture renderTex = (RenderTexture)m_VideoRawImg.texture;
                renderTex.Release();
            }
        }
    }
}
