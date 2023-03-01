using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace E3D
{
    public class GUIKrankenakte : GUIBase
    {
        [Header("Krankenakte/Karte")]
        public TMP_Text m_TxtName;
        public Image m_ImgPortrait;
        public Image m_ImgSex;
        
        public Slider m_Healthbar;

        public TMP_Text m_TxtAge;
        public TMP_Text m_TxtPregnant;
        public Image m_ImgPAC;
        public TMP_Text m_TxtPAC;

        public Text m_TxtHR;
        public Text m_TxtSystolic;
        public Text m_TxtDiastolic;
        public Text m_TxtWalk;
        public Text m_TxtSpO2;
        public Text m_TxtResp;
        public Text m_TxtGCS;

        public TMP_Text m_TxtCondition;

        public Sprite m_NullMalePortraitSprite;
        public Sprite m_NullFemalePortraitSprite;
        public Sprite m_ImgMaleSexIcon;
        public Sprite m_ImgFemaleSexIcon;


        public AVictim Victim { get; private set; }

        public void SetVictim(AVictim newVictim)
        {
            Victim = newVictim;

            ClearDisplay();
            UpdateProfileDisplay();
        }

        private void UpdateProfileDisplay()
        {
            if (Victim == null)
                return;

            m_TxtName.text = Victim.m_GivenName;

            // put profile pic
            if (m_ImgPortrait != null)
            {
                if (Victim.m_PortraitSprite != null)
                {
                    m_ImgPortrait.sprite = Victim.m_PortraitSprite;
                }
                else
                {
                    if (Victim.m_Sex == ESex.Male)
                        m_ImgPortrait.sprite = m_NullMalePortraitSprite;
                    else
                        m_ImgPortrait.sprite = m_NullFemalePortraitSprite;
                }
            }

            // display sex symbol (yea boi!!)
            if (m_ImgSex != null)
            {
                if (Victim.m_Sex == ESex.Male)
                    m_ImgSex.sprite = m_ImgMaleSexIcon;
                else
                    m_ImgSex.sprite = m_ImgFemaleSexIcon;
            }

            // print profile
            string ageGroupString = Victim.m_AgeLo.ToString() + " ~ " + Victim.m_AgeHi.ToString();
            string isPregnantString = "No";

            // if the victim can be pregnant, then only we check whether this person is pregnant or not
            if (Victim.m_CanBePregnant)
            {
                isPregnantString = Victim.m_IsPregnant ? "Yes" : "No";
            }

            m_TxtAge.text = ageGroupString;
            m_TxtPregnant.text = isPregnantString;

            m_TxtPAC.text = Victim.m_State.m_GivenPACS.ToString();

            // print condition
            Injury injury = InjuryList.FindByIndex(Victim.m_Injury);
            m_TxtCondition.text = injury.m_PrintName + "\n" + injury.m_Description;
        }

        private void LateUpdate()
        {
            if (Victim == null)
                return;

            string pacString;
            string walkString = "?";
            string systolicString = "???/";
            string diastolicString = "??";
            string hrString = "??";
            string respString = "??";
            string spo2String = "??";
            string gcsString = "??";

            // update PAC
            EPACS pacs = Victim.m_State.m_GivenPACS;
            pacString = pacs.ToString();
            m_TxtPAC.text = pacString;

            switch(pacs)
            {
                case EPACS.None:
                    m_ImgPAC.color = Consts.COLOR_PAC_OK2;
                    break;

                case EPACS.P0:
                    m_ImgPAC.color = Consts.COLOR_P0;
                    break;

                case EPACS.P1:
                    m_ImgPAC.color = Consts.COLOR_P1;
                    break;

                case EPACS.P2:
                    m_ImgPAC.color = Consts.COLOR_P2;
                    break;

                case EPACS.P3:
                    m_ImgPAC.color = Consts.COLOR_P3;
                    break;
            }

            // health bar
            m_Healthbar.value = Victim.m_CurHealth / Consts.MAX_VICTIM_HEALTH;

            var victimState = Victim.m_State;

            // heart rate
            if (victimState.m_CheckedHeartRate)
                hrString = Victim.m_HeartRate.ToString();

            m_TxtHR.text = hrString;

            // blood pressure
            if (victimState.m_CheckedBloodPressure)
            {
                systolicString = Mathf.CeilToInt(Victim.m_BloodPressure.Systolic).ToString() + "/";
                diastolicString = Mathf.CeilToInt(Victim.m_BloodPressure.Diastolic).ToString();
            }

            m_TxtSystolic.text = systolicString;
            m_TxtDiastolic.text = diastolicString;

            // spo2
            if (victimState.m_CheckedSpO2)
                spo2String = Victim.m_SpO2.ToString();

            m_TxtSpO2.text = spo2String;

            // ambulant
            if (victimState.m_CheckedCanWalk)
                walkString = Victim.m_CanWalk ? "Yes" : "No";

            m_TxtWalk.text = walkString;

            // respiration
            if (victimState.m_CheckedRespiration)
                respString = Victim.m_Respiration.ToString();

            m_TxtResp.text = respString;

            // gcs
            if (victimState.m_CheckedGCS)
                gcsString = Victim.m_GCS.ToString();

            m_TxtGCS.text = gcsString;
        }

        public void ClearDisplay()
        {
            m_TxtName.text = "No victim";

            if (m_ImgPortrait != null)
                m_ImgPortrait.sprite = m_NullMalePortraitSprite;

            if (m_ImgSex != null)
                m_ImgSex.sprite = null;

            m_TxtAge.text = "?";
            m_TxtPregnant.text = "?";
            m_Healthbar.value = 0.0f;

            m_TxtHR.text = "00";
            m_TxtSystolic.text = "000/";
            m_TxtDiastolic.text = "00";
            m_TxtWalk.text = "?";
            m_TxtSpO2.text = "00";
            m_TxtResp.text = "00";
            m_TxtGCS.text = "00";

            m_TxtCondition.text = "No data";
        }
    }
}
