using UnityEngine;
using TMPro;
using System;

namespace E3D
{
    public class GUIGameStateDisplay : MonoBehaviour
    {
        public TMP_Text m_TxtIncidentScene;
        public TMP_Text m_TxtCurrLocation;
        public TMP_Text m_TxtTime;
        public TMP_Text m_TxtVictimCount;

        public E3DPlayer Player { get; set; }

        private void LateUpdate()
        {
            if (GameMode.Current != null)
            {
                // update game time
                var timeSpan = TimeSpan.FromSeconds(GameMode.Current.ElapsedGameTime);
                m_TxtTime.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);

                // update incident scene text
                if (Globals.m_CurrentMap != null)
                {
                    m_TxtIncidentScene.text = Globals.m_CurrentMap.m_DisplayName;
                }

                // update victims left
                int victimsLeft = GameMode.Current.m_State.m_Victims.Count;

                if (Player != null && Player.PossessedEmt != null)
                {
                    if (Player.PossessedEmt is AEmtTriageOffr)
                        victimsLeft -= GameMode.Current.m_State.m_TriagedCount;

                    if (Player.PossessedEmt is AEmtFirstAidDoc)
                        victimsLeft -= GameMode.Current.m_State.m_TreatedCount;

                    if (Player.PossessedEmt is AEmtEvacOffr)
                        victimsLeft -= GameMode.Current.m_State.m_EvacuatedCount;

                    if (Player.PossessedEmt.CurrentArea != null)
                        m_TxtCurrLocation.text = Player.PossessedEmt.CurrentArea.m_PrintName;
                    else
                        m_TxtCurrLocation.text = "Loc: None";
                }

                m_TxtVictimCount.text = "Victims: " + victimsLeft.ToString();
            }
        }
    }
}
