using System;
using UnityEngine;

namespace E3D
{
    // patient sex (if you dare change this i'll fucking kill you!
    public enum ESex { Female = 0, Male = 1 }

    // capiliary refill time
    public enum ECapRefillTimeType { NoResponse = 0, LessThan2 = 1, MoreThan2 = 2 }

    /// <summary>
    /// Victim actor
    /// </summary>
    [RequireComponent(typeof(VictimState))]
    public class AVictim : MonoBehaviour
    {
        [HideInInspector]
        public VictimState m_State;

        public int m_Id;
        public Sprite m_PortraitSprite;
        public string m_GivenName;
        public bool m_IsActive;
        public bool m_CanBePregnant;
        public bool m_IsPregnant;
        public ESex m_Sex;
        public EPACS m_PACS;

        public int m_Age;
        public int m_AgeLo;
        public int m_AgeHi;

        public float m_CurHealth;          // current health
        public float m_HealthSinceGameStart;
        public float m_StartHealth;

        public bool m_CanWalk;

        public float m_StartHeartRate;
        public int m_HeartRate;

        public float m_StartResp;
        public int m_Respiration;

        public BloodPressure m_StartBp;
        public BloodPressure m_BloodPressure;
        
        public float m_StartSpO2;
        public float m_SpO2;

        public float m_StartGCS;
        public float m_GCS;
        //public ECapRefillTimeType     m_CapRefillTimeType;
        public int m_Injury;
        public string m_TreatmentTags;
        public float m_CurStableTime;

        public const float m_Resitance = 1.075f;
        public const float m_Elasticity = 0.597f;
        public const float m_StrokeVolume = 72.0f;

        private float m_s_Resp;
        private float m_s_HR;
        private float m_s_GCS;
        private float m_s_Total;
        private float m_stableTime;
        private E3DPlayer m_player;

        public bool IsAlive { get => m_CurHealth > 0; }

        public bool IsPlayerUsing { get => m_player != null; }

        public E3DPlayer UsingPlayer { get => m_player; }

        public int InjuryIdx { get => m_Injury; }

        public string TreatmentTags { get => m_TreatmentTags; }

        public bool HasInjury { get => (m_Injury == -1 && string.IsNullOrEmpty(m_TreatmentTags)); }


        protected void Awake()
        {
            m_State = GetComponent<VictimState>();

            m_Id = -1;
            m_GivenName = "unnamed";
            m_IsActive = true;
            m_CanBePregnant = false;
            m_IsPregnant = false;
            m_Sex = ESex.Male;
            m_PACS = EPACS.None;
            m_Age = 0;
            m_AgeLo = 0;
            m_AgeHi = 1;

            m_CurHealth = 100.0f;

            m_CanWalk = true;
            m_HeartRate = 0;
            m_Respiration = 0;
            m_BloodPressure = BloodPressure.Zero;
            m_SpO2 = 0.0f;
            m_GCS = 0.0f;
            //m_CapRefillTimeType = ECapRefillTimeType.NoResponse;
            
            m_Injury = -1;
            m_TreatmentTags = string.Empty;

            m_HealthSinceGameStart = 0.0f;
            m_StartHealth = 0.0f;
            m_StartSpO2 = 0.0f;
            m_StartResp = 0.0f;
            m_StartGCS = 0.0f;
            m_StartHeartRate = 0.0f;
            m_StartBp = BloodPressure.Zero;

            m_s_Resp = 0.0f;
            m_s_HR = 0.0f;
            m_s_GCS = 0.0f;
            m_s_Total = 0.0f;
            m_stableTime = 0.0f;
        }

        protected void Reset()
        {
        }

        protected void Start()
        {
            if (GameState.Current != null)
            {
                GameState.Current.AddVictim(this);
            }
        }

        public void Init(int id, ESex sex, int age, int age_lo, int age_hi, Sprite portraitSprite, EPACS scale)
        {
            m_Id = id;
            m_Sex = sex;
            m_Age = age;
            m_AgeLo = age_lo;
            m_AgeHi = age_hi;
            m_PortraitSprite = portraitSprite;
            m_PACS = scale;

            // give the victim a name
            string idString = m_Id.ToString();
            m_GivenName = "Victim #" + idString;

            // apply the initial vitals
            if (m_PACS == EPACS.P3)
            {
                SetVitalsByPAC(EPACS.P3);
            }
            else if (m_PACS == EPACS.P2)
            {
                SetVitalsByPAC(EPACS.P2);
            }
            else if (m_PACS == EPACS.P1)
            {
                SetVitalsByPAC(EPACS.P1);
            }
            else if (m_PACS == EPACS.P0)
            {
                SetVitalsByPAC(EPACS.P0);
            }
            else
            {
                SetVitalsByPAC(EPACS.None);
            }

            // set initial vitals here
            m_StartSpO2 = m_SpO2;
            m_StartResp = m_Respiration;
            m_StartGCS = m_GCS;
            m_StartHeartRate = m_HeartRate;
            m_StartBp = m_BloodPressure;

            // do vital scoring shit
            if (m_StartResp <= 20)
            {
                m_s_Resp = 5.15529f / (1 + Mathf.Exp(-m_StartResp / 3.74014f + 1.39551f)) - 1.01516f;
            }
            else
            {
                m_s_Resp = 4.29751f / (1 + Mathf.Exp(m_StartResp / 5.68283f - 6.17549f));
            }

            m_s_HR = Mathf.Pow(200 - m_StartHeartRate, 5.0f) * Mathf.Pow(14.0f + m_StartHeartRate, 5.0f) * 2.0335f * Mathf.Pow(10.0f, -20.0f);
            m_s_GCS = 2.43555f * Mathf.Log(m_StartGCS) - 2.63742f;
            m_s_Total = m_s_Resp + m_s_HR + m_s_GCS;

            // set initial health
            m_HealthSinceGameStart = CalcNewHealth(0.0f);   // because we start at 0 yo!
            m_CurHealth = m_HealthSinceGameStart;
            m_stableTime = CalcStableTime(0.0f, 0.0f, 0.0f);
        }

        public void Use(E3DPlayer player)
        {
            m_player = player;
        }

        public void AddHealth(float amount)
        {
            m_CurHealth += amount;
            if (m_CurHealth > Consts.MAX_VICTIM_HEALTH)
                m_CurHealth = Consts.MAX_VICTIM_HEALTH;
        }

        public void RemoveHealth(float amount)
        {
            m_CurHealth -= amount;
            if (m_CurHealth <= 0)
            {
                m_CurHealth = 0.0f;
                m_PACS = EPACS.P0;
                SetVitalsByPAC(EPACS.P0);

                //var victimManager = E3D_VictimManager.Current;
                //if (victimManager != null)
                //    victimManager.m_DeadCount += 1;
            }
        }

        public float CalcNewHealth(float time)
        {
            if (m_s_Total <= float.Epsilon)
                return 0.0f;

            float result = (m_s_Total * 100.0f / 12.0f) / (Mathf.Pow(((time / 30) /
                Mathf.Pow(2.11876f, (0.50176f * m_s_Total - 2.26823f))), (2.46462f / (1 + Mathf.Exp(0.58548f * m_s_Total - 7.24525f)))) + 1);
            return result;
        }

        public void UpdateHealthAndVitals(float time)
        {
            m_CurHealth = CalcNewHealth(time);
            float deltaHealth = m_HealthSinceGameStart - m_CurHealth;

            /* adjust vitals */
            m_HeartRate = (int)AdjustHeartRate(deltaHealth * m_s_HR / m_s_Total / 25.0f);
            m_Respiration = (int)AdjustRespiration(deltaHealth * m_s_Resp / m_s_Total / 25.0f);
            m_BloodPressure.Systolic = Mathf.RoundToInt(((3 * m_Resitance * m_HeartRate * m_StrokeVolume / 60) + (2 * m_StrokeVolume * m_Elasticity)) / 3);
            m_BloodPressure.Diastolic = Mathf.RoundToInt(((3 * m_Resitance * m_HeartRate * m_StrokeVolume / 60) - (2 * m_StrokeVolume * m_Elasticity)) / 3);
            //m_Vitals.SpO2 = spO2;
            m_GCS = AdjustGCS(deltaHealth * m_s_GCS / m_s_Total / 25);

            if (m_CurHealth <= Consts.VERY_SMOL_HEALTH)
            {
                m_CurHealth = 0;
                m_PACS = EPACS.P0;
                SetVitalsByPAC(m_PACS);
            }
        }

        public void SetVitalsByPAC(EPACS scale)
        {
            switch (scale)
            {
                case EPACS.P3:
                    SetVitals(true, 80, 20, 120, 80, 99, 15.0f);
                    break;

                case EPACS.P2:
                    SetVitals(false, 90, 20, 110, 70, 95, 9.0f);
                    break;

                case EPACS.P1:
                    SetVitals(false, 140, 40, 90, 60, 90, 4.0f);
                    break;

                case EPACS.P0:
                    SetVitals(false, 0, 0, 0, 0, 0, 0.0f);
                    break;

                case EPACS.None:
                    SetVitals(true, 60, 14, 120, 80, 100, 15.0f);
                    break;
            }
        }

        public void SetVitals(bool canWalk, int heartRate, int respiration, int bp_systolic, int bp_diastolic, float spO2, float gcs)
        {
            m_CanWalk = canWalk;
            m_HeartRate = heartRate;
            m_Respiration = respiration;
            m_BloodPressure.Systolic = bp_systolic;
            m_BloodPressure.Diastolic = bp_diastolic;
            m_SpO2 = spO2;
            m_GCS = gcs;
            //m_CapRefillTimeType = capRefillTimeType;
        }

        public void SetInjury(Injury injury)
        {
            m_Injury = InjuryList.IndexOf(injury);
            m_TreatmentTags = injury.m_TreatmentTags;

            //RemoveHealth(injury.m_Damage);
        }

        public string FindTreatmentTag(string tag)
        {
            if (m_TreatmentTags.Contains(tag))
            {
                string[] tags = m_TreatmentTags.Split(',');
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i].Equals(tag))
                        return tags[i];
                }
            }

            return null;
        }

        public bool RemoveTreatmentTag(string tag)
        {
            if (m_TreatmentTags.Contains(tag))
            {
                string[] tags = m_TreatmentTags.Split(',');
                m_TreatmentTags = string.Empty;

                // reconstruct the treatment tags string
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i].Equals(tag))
                        continue;
                    if (i < tags.Length - 1)
                        m_TreatmentTags = string.Concat(m_TreatmentTags, tags[i], ",");
                    else
                        m_TreatmentTags = string.Concat(m_TreatmentTags, tags[i]);
                }

                return true;
            }

            return false;
        }

        public bool ContainsTreatmentTag(string tag)
        {
            return m_TreatmentTags.Contains(tag);
        }

        public bool RequiresTreatment()
        {
            return !string.IsNullOrEmpty(m_TreatmentTags);
        }

        // JOSHUA: 
        public float CalcStableTime(float hr, float resp, float gcs)
        {
            return 10.0f;
        }

        private float AdjustHeartRate(float scaledHealth)
        {
            int result = 0;
            float a = 200.0f;
            float b = 14.0f;
            float c = 5.0f;
            float d = 2.0335f * Mathf.Pow(10, -20);
            float s_initHeartRate = Mathf.Pow(a - m_StartHeartRate, c) * Mathf.Pow(b + m_StartHeartRate, c) * d;
            float HR1 = (float)(((a - b) / 2.0f) - 0.5 * Mathf.Pow(d, (-1.0f / (2.0f * c))) * Mathf.Sqrt(a * a * Mathf.Pow(d, 1.0f / c) + 2.0f * a * b * Mathf.Pow(d, 1.0f / c) + b * b * Mathf.Pow(d, 1.0f / c) - 4.0f * Mathf.Pow(s_initHeartRate - scaledHealth, 1.0f / c)));
            float HR2 = (float)(((a - b) / 2.0f) + 0.5 * Mathf.Pow(d, (-1.0f / (2.0f * c))) * Mathf.Sqrt(a * a * Mathf.Pow(d, 1.0f / c) + 2.0f * a * b * Mathf.Pow(d, 1.0f / c) + b * b * Mathf.Pow(d, 1.0f / c) - 4.0f * Mathf.Pow(s_initHeartRate - scaledHealth, 1.0f / c)));

            if ((m_StartHeartRate - HR1) > (m_StartHeartRate - HR2))
            {
                result = Mathf.RoundToInt(HR2);
            }
            else
            {
                result = Mathf.RoundToInt(HR1);
            }

            if (result > 0 && result <= 200)
            {
                return result;
            }

            return 0;
        }

        private float AdjustRespiration(float scaledHealth)
        {
            float s_initRR;
            int result;
            if (m_StartResp <= 20)
            {
                s_initRR = 5.15529f / (1 + Mathf.Exp(-m_StartResp / 3.74014f + 1.39551f)) - 1.01516f;
                result = Mathf.RoundToInt((float)(-3.74014f * (Math.Log(5.15529f / ((s_initRR - scaledHealth) + 1.01516f) - 1) + 1.39551)));
            }
            else
            {
                s_initRR = 4.29751f / (1 + Mathf.Exp(m_StartResp / 5.68283f - 6.17549f));
                result = Mathf.RoundToInt((float)(5.68283f * (Mathf.Log(4.29751f / (s_initRR - scaledHealth) - 1) + 6.17549)));
            }

            if (result > 0 && result <= 60)
            {
                return result;
            }

            return 0;
        }

        private float AdjustGCS(float scaledHealth)
        {
            float s_initGCS = 2.43555f * Mathf.Log(m_StartGCS) - 2.63742f;
            float result = Mathf.RoundToInt(Mathf.Exp(((s_initGCS - scaledHealth) + 2.63742f) / 2.43555f));
            if (result <= 15 && result >= 3)
            {
                return result;
            }

            return 0;
        }

        private void OnDestroy()
        {
            if (GameState.Current != null)
            {
                GameState.Current.RemoveVictim(this);
            }
        }

        private void OnValidate()
        {
            if (m_State == null)
            {
                m_State = GetComponent<VictimState>();
                if (m_State == null)
                    m_State = gameObject.AddComponent<VictimState>();
            }
        }
    }


    /// <summary>
    /// Blood pressure
    /// </summary>
    [Serializable]
    public struct BloodPressure : IEquatable<BloodPressure>
    {
        public static BloodPressure Zero = new BloodPressure(0, 0);

        public int Systolic;        // systolic blood pressure
        public int Diastolic;       // diastolic blood pressure


        public BloodPressure(int systolic, int diastolic)
        {
            this.Systolic = systolic;
            this.Diastolic = diastolic;
        }

        public override string ToString()
        {
            return Systolic.ToString() + "/" + Diastolic.ToString();
        }

        public bool Equals(BloodPressure other)
        {
            return this.Systolic == other.Systolic && this.Diastolic == other.Diastolic;
        }
    }


    /// <summary>
    /// Injury Slot
    /// </summary>
    [Serializable]
    public class InjurySlot
    {
        public int InjuryIdx;                             // the index for the injury in the injury list
        public string TreatmentTags;                      // the list of treatment tags required to cure this injury. separated by '|' character


        public InjurySlot()
        {
            InjuryIdx = -1;
            TreatmentTags = string.Empty;
        }

        public InjurySlot(int injuryIdx, string treatmentTags)
        {
            InjuryIdx = injuryIdx;
            TreatmentTags = treatmentTags;
        }

        public string FindTreatmentTag(string tag)
        {
            if (TreatmentTags.Contains(tag))
            {
                string[] tags = TreatmentTags.Split(',');
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i].Equals(tag))
                        return tags[i];
                }
            }

            return null;
        }

        public bool RemoveTreatmentTag(string tag)
        {
            if (TreatmentTags.Contains(tag))
            {
                string[] tags = TreatmentTags.Split(',');
                TreatmentTags = string.Empty;

                // reconstruct the treatment tags string
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i].Equals(tag))
                        continue;
                    if (i < tags.Length - 1)
                        TreatmentTags = string.Concat(TreatmentTags, tags[i], ",");
                    else
                        TreatmentTags = string.Concat(TreatmentTags, tags[i]);
                }

                return true;
            }

            return false;
        }

        public bool ContainsTag(string tag)
        {
            return TreatmentTags.Contains(tag);
        }

        public bool HasTags()
        {
            return !string.IsNullOrEmpty(TreatmentTags);
        }

        public bool Equals(InjurySlot other)
        {
            return InjuryIdx == other.InjuryIdx && TreatmentTags.Equals(other.TreatmentTags);
        }
    }
}
