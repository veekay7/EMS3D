using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace E3D
{
    [RequireComponent(typeof(GameConfigManager))]
    public class GUIConfigScreen : GUIScreen
    {
        [HideInInspector]
        public GameConfigManager m_GameConfigManager;

        [Header("Config Screen")]
        public TMP_Dropdown m_ScreenResoDrawer;
        public Toggle m_FullscreenChkBox;
        public Slider m_MasterVolumeSlider;
        public Slider m_SfxVolumeSlider;
        public Slider m_MusicVolumeSlider;
        public Slider m_PrintSpdSlider;

        private bool m_initialised;


        protected override void Awake()
        {
            base.Awake();

            m_GameConfigManager = GetComponent<GameConfigManager>();
            m_initialised = false;
        }

        public override void Open(UnityAction onFinishAnim = null)
        {
            if (!m_initialised)
            {
                PopulateScreenResolutionDrawer();
                m_initialised = true;
            }

            // setup the screen on open
            Resolution curRes = new Resolution();
            curRes.width = Screen.currentResolution.width;
            curRes.height = Screen.currentResolution.height;

            int index = Globals.m_SupportedResolutions.IndexOf(curRes);
            if (index != -1)
                m_ScreenResoDrawer.value = index;
            else
                m_ScreenResoDrawer.value = 0;

            m_FullscreenChkBox.isOn = Globals.m_UserGameConfig.m_FullScreen;
            m_MasterVolumeSlider.value = Globals.m_UserGameConfig.m_MasterVolume;
            m_SfxVolumeSlider.value = Globals.m_UserGameConfig.m_SfxVolume;
            m_MusicVolumeSlider.value = Globals.m_UserGameConfig.m_BgmVolume;
            m_PrintSpdSlider.value = Globals.m_UserGameConfig.m_PrintSpd;

            base.Open(onFinishAnim);
        }

        private void PopulateScreenResolutionDrawer()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            m_ScreenResoDrawer.ClearOptions();

            for (int i = 0; i < Globals.m_SupportedResolutions.Count; i++)
            {
                var res = Globals.m_SupportedResolutions[i];
                string label = res.width.ToString() + " x " + res.height.ToString();

                options.Add(new TMP_Dropdown.OptionData(label));
            }

            m_ScreenResoDrawer.AddOptions(options);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            m_GameConfigManager = GetComponent<GameConfigManager>();
        }
    }
}
