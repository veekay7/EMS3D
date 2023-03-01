using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace E3D
{
    public class GUITrainingScreen : GUIScreen
    {
        [Header("Training Screen")]
        public TMP_Dropdown m_MapDrawer;
        public Slider m_NumVictimsSlider;
        public Slider m_P3VictimsSlider;
        public Slider m_P2VictimsSlider;
        public Slider m_P1VictimsSlider;
        public Slider m_P0VictimsSlider;

        [SerializeField, ReadOnlyVar]
        private MapListEntry[] m_mapList;
        [SerializeField, ReadOnlyVar]
        private int m_selectedMapIdx;

        private bool m_mapListPopulated;


        protected override void Awake()
        {
            base.Awake();
            m_selectedMapIdx = -1;
        }

        public override void Open(UnityAction onFinishAnim = null)
        {
            m_selectedMapIdx = -1;

            if (!m_mapListPopulated)
            {
                m_mapList = Resources.LoadAll<MapListEntry>("MapList");

                PopulateMapDrawer();

                m_mapListPopulated = true;
            }

            if (m_MapDrawer.options.Count > 0)
                m_MapDrawer.onValueChanged.Invoke(0);

            m_NumVictimsSlider.value = Globals.m_NumVictims;
            m_P3VictimsSlider.value = Globals.m_Probability_P3;
            m_P2VictimsSlider.value = Globals.m_Probability_P2;
            m_P1VictimsSlider.value = Globals.m_Probability_P1;
            m_P0VictimsSlider.value = Globals.m_Probability_P0;

            base.Open(onFinishAnim);
        }

        public void SelectMap(int index)
        {
            m_selectedMapIdx = index;
        }

        public void StartGame()
        {
            SetInteractable(false);

            // set up with game systems
            if (GameSystem.Instance == null)
            {
                Debug.Log("Cannot start the game, GameSystem.Instance is null");
            }
            else
            {
                if (m_selectedMapIdx != -1)
                {
                    MapListEntry map = m_mapList[m_selectedMapIdx];
                    if (string.IsNullOrEmpty(map.m_SceneFileName))
                    {
                        Debug.Log("Cannot load scene, scene file name is empty in " + map.name);
                        return;
                    }

                    Globals.m_CurrentMap = map;
                    Globals.m_NumVictims = (int)m_NumVictimsSlider.value;
                    Globals.m_Probability_P3 = (int)m_P3VictimsSlider.value;
                    Globals.m_Probability_P2 = (int)m_P2VictimsSlider.value;
                    Globals.m_Probability_P1 = (int)m_P1VictimsSlider.value;
                    Globals.m_Probability_P0 = (int)m_P0VictimsSlider.value;
                    
                    ScreenWiper.Instance.DoFade(ScreenWiper.FillMode.Fill, 1.0f, 0.0f, () =>
                    {
                        SceneLoader.Instance.LoadScene(map.m_SceneFileName);
                    });
                }
            }
        }

        private void PopulateMapDrawer()
        {
            m_MapDrawer.ClearOptions();

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            
            for (int i = 0; i < m_mapList.Length; i++)
            {
                string label = m_mapList[i].m_DisplayName;
                options.Add(new TMP_Dropdown.OptionData(label));
            }

            m_MapDrawer.AddOptions(options);
            if (m_MapDrawer.options.Count > 0)
            {
                m_MapDrawer.value = 0;
            }
        }
    }
}
