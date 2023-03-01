using UnityEngine;

namespace E3D
{
    public class AEmtFirstAidDoc : AEmtBase
    {
        public bool m_TreatmentWasApplied;

        private AFirstAidPoint m_curfap;
        [SerializeField, ReadOnlyVar]
        private AVictim m_curVictim;
        private float m_startTime;

        public AFirstAidPoint CurrentFirstAidPoint { get => m_curfap; }

        public AVictim CurrentVictim { get => m_curVictim; }


        protected override void Awake()
        {
            base.Awake();

            m_curVictim = null;
            m_startTime = 0.0f;
        }

        public override void EnterArea(AVictimPlaceableArea area)
        {
            base.EnterArea(area);

            if (m_curArea != null)
                m_curfap = (AFirstAidPoint)m_curArea;
        }

        public void SetVictim(AVictim newVictim)
        {
            if (m_curVictim != null)
            {
                m_curVictim.Use(null);
                m_curVictim = null;

                m_startTime = 0.0f;
                m_TreatmentWasApplied = false;
            }

            if (newVictim != null)
            {
                newVictim.Use(m_player);

                m_curVictim = newVictim;

                m_startTime = Time.time;
                m_TreatmentWasApplied = false;
            }
        }

        public void UseItemOnVictim(int itemIdx)
        {
            if (m_curVictim == null)
                return;

            string outputString;
            int result = m_curfap.UseItem(itemIdx, m_curVictim, out outputString);
            
            switch (result)
            {
                case 0: // success
                    {
                        m_TreatmentWasApplied = true;

                        if (m_curVictim.RequiresTreatment())
                            outputString += "\nThe victim seems to be more stable but more needs to be done.";
                        else
                            outputString += "\nThe victim is now stable.";

                        m_player.m_State.m_TotalTreatmentNum++;
                        m_player.m_State.m_CorrectTreatmentNum++;
                    }
                    break;

                //case 1: // insufficient quantities
                //    {
                //        m_player.m_State.m_TotalTreatmentNum++;
                //    }
                //    break;
            }

            SendResponse("doc_use_item", outputString);
        }

        public void SendVictimToMorgue()
        {
            if (m_curVictim == null)
                return;

            if (!m_curVictim.IsAlive)
            {
                m_player.m_State.m_TotalTreatmentNum++;

                if (m_curVictim.m_StartHealth <= float.Epsilon)
                {
                    m_player.m_State.m_CorrectTreatmentNum++;
                }

                // mark the selected victim as treated
                m_curVictim.m_State.m_IsTreated = true;

                // deactivate the victim
                m_curVictim.m_IsActive = false;

                // do scoring shit here!!
                float endTime = Time.time - m_startTime;
                GameMode.Current.ProcessMorgueScoring(m_player, m_curVictim, endTime);
                m_startTime = 0.0f;

                // remove victim from the first aid point
                m_curfap.RemoveVictim(m_curVictim);

                m_player.m_State.m_TotalVictimsAttendedNum++;

                SendResponse("send_morgue_success", "Sent " + m_curVictim.m_GivenName + " to the the morgue.");

                // unset the victim
                SetVictim(null);
            }
            else
            {
                SendResponse("not_dead", "The victim is still alive. You cannot send the victim to the morgue.");
            }
        }

        public void SendVictimToEvac()
        {
            if (m_curVictim == null)
                return;

            if (!m_curVictim.RequiresTreatment())
            {
                // this one checks to see if the victim has a "no action" tag and has not yet applied any treatment to,
                // they will get a score of m_TreatmentCount++ and m_CorrectTreament++.
                if (m_curVictim.HasInjury && m_curVictim.ContainsTreatmentTag("no action"))
                {
                    m_player.m_State.m_TotalTreatmentNum++;
                    m_player.m_State.m_CorrectTreatmentNum++;
                }

                // mark the selected victim as treated
                m_curVictim.m_State.m_IsTreated = true;

                // deactivate the victim
                m_curVictim.m_IsActive = false;

                // do scoring shit here!!
                float endTime = Time.time - m_startTime;
                GameMode.Current.ProcessTreatmentScoring(m_player, m_curVictim, endTime);
                m_startTime = 0.0f;

                m_player.m_State.m_TotalVictimsAttendedNum++;

                // notify listeners
                SendResponse("success", "Sent " + m_curVictim.m_GivenName + " to the evacuation area.");

                // remove victim from the first aid point and unset the victim from the 
                m_curfap.RemoveVictim(m_curVictim);

                // unset the victim
                SetVictim(null);
            }
            else
            {
                // the patient requirestreatment still
                SendResponse("require_treatment", "The victim still requires treatment.");
            }
        }
    }
}
