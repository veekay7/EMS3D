using UnityEngine;

namespace E3D
{
    // the action id for checking vitals by the triage officer
    public enum ECheckVitalAct { None, CanWalk, HeartRate, Respiration, BloodPressure, SpO2, CapRefillTime, GCS };

    public class AEmtTriageOffr : AEmtBase
    {
        private AVictim m_curVictim;
        private float m_startTime;

        public AVictim CurrentVictim { get => m_curVictim; }


        protected override void Awake()
        {
            base.Awake();

            m_curVictim = null;
            m_startTime = 0.0f;
        }

        public void SetVictim(AVictim newVictim)
        {
            if (m_curVictim != null)
            {
                m_curVictim.Use(null);
                m_curVictim = null;
            }

            if (newVictim != null)
            {
                newVictim.Use(m_player);

                m_curVictim = newVictim;
                m_startTime = Time.time;
            }
        }

        public void CheckVictimVital(ECheckVitalAct action)
        {
            if (m_curVictim == null)
                return;

            switch (action)
            {
                case ECheckVitalAct.CanWalk:
                    m_curVictim.m_State.m_CheckedCanWalk = true;
                    break;

                case ECheckVitalAct.HeartRate:
                    m_curVictim.m_State.m_CheckedHeartRate = true;
                    break;

                case ECheckVitalAct.Respiration:
                    m_curVictim.m_State.m_CheckedRespiration = true;
                    break;

                case ECheckVitalAct.BloodPressure:
                    m_curVictim.m_State.m_CheckedBloodPressure = true;
                    break;

                case ECheckVitalAct.SpO2:
                    m_curVictim.m_State.m_CheckedSpO2 = true;
                    break;

                case ECheckVitalAct.GCS:
                    m_curVictim.m_State.m_CheckedGCS = true;
                    break;
            }

            SendResponse("check_vital", string.Empty, action);
        }

        public void SetPACSTag(EPACS tag)
        {
            if (m_curVictim == null)
                return;

            m_curVictim.m_State.m_GivenPACS = tag;

            SendResponse("set_pacs", string.Empty, tag);
        }

        public void FinishTriage()
        {
            if (m_curVictim != null && m_curVictim.m_State.m_GivenPACS != EPACS.None)
            {
                var gameMode = GameMode.Current;

                // if the selected victim has not been given a tag, cannot finish triage bish!!
                if (!gameMode.CanSendVictimToFirstAid(m_curVictim))
                    return;

                bool isMarkedDeadWrongly = gameMode.CheckVictimWronglyTriagedDead(m_curVictim);
                if (isMarkedDeadWrongly)
                {
                    SendResponse("triage_wrong", "Wrongly triaged " + m_curVictim.m_GivenName + " as dead.");
                }
                else
                {
                    // mark the selected victim as triaged and deactivate them
                    m_curVictim.m_State.m_IsTriaged = true;
                    m_curVictim.m_IsActive = false;        // NOTE: this should not be set to false in multiplayer

                    // inform client about correct triage
                    SendResponse("triage_complete", m_curVictim.m_GivenName + " has been successfully triaged.");

                    // calculate scoring
                    float triageTime = Time.time - m_startTime;
                    gameMode.ProcessTriageScoring(m_player, m_curVictim, triageTime);
                    m_startTime = 0.0f;

                    // remove the victim from the area
                    m_curArea.RemoveVictim(m_curVictim);

                    // send the victim to the first aid point
                    gameMode.SendVictimToRandFirstAidPoint(m_curVictim);

                    // unset the victim
                    m_curVictim = null;
                }
            }
        }

        public void StopTriage()
        {
            if (m_curVictim != null)
            {
                m_curVictim.m_State.m_CheckedBloodPressure = false;
                m_curVictim.m_State.m_CheckedCanWalk = false;
                m_curVictim.m_State.m_CheckedGCS = false;
                m_curVictim.m_State.m_CheckedHeartRate = false;
                m_curVictim.m_State.m_CheckedRespiration = false;
                m_curVictim.m_State.m_CheckedSpO2 = false;
                m_curVictim.m_State.m_GivenPACS = EPACS.None;
                SetVictim(null);

                SendResponse("triage_cancel", string.Empty);
            }
        }
    }
}

