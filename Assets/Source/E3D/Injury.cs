using System;
using UnityEngine;

namespace E3D
{
    [Serializable]
    public class Injury
    {
        public uint m_Id;                               // Id of the injury.
        public string m_PrintName;                      // The name of the injury when displayed in the UI.
        public string m_Description;                    // The description of the injury. Should use some doctor's dictionary definition for this.
        public EBodyPartId m_BodyPartId;                // The location which the injury is on. It's an enum.
        public float m_Damage;                          // The initial damage value that will be dealt to the victim.
        public float m_DamagePerSec;                    // The damage value that will be applied per second.
        public float m_HeartRateOffsetScale;
        public float m_RespirationOffsetScale;
        public float m_SystolicOffsetScale;
        public float m_DiastolicOffsetScale;
        public float m_SpO2OffsetScale;
        public EPACS m_PAC;
        public string m_TreatmentTags;


        public Injury()
        {
            m_Id = 0;
            m_PrintName = null;
            m_Description = null;
            m_BodyPartId = EBodyPartId.Unknown;
            m_Damage = 0;
            m_DamagePerSec = 0;
            m_HeartRateOffsetScale = 0;
            m_RespirationOffsetScale = 0;
            m_SystolicOffsetScale = 0;
            m_DiastolicOffsetScale = 0;
            m_SpO2OffsetScale = 0;
            m_PAC = EPACS.None;
            m_TreatmentTags = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="printName"></param>
        /// <param name="description"></param>
        /// <param name="location"></param>
        /// <param name="damage"></param>
        /// <param name="damagePerSec"></param>
        /// <param name="heartRateOffsetScale"></param>
        /// <param name="respirationOffsetScale"></param>
        /// <param name="systolicOffsetScale"></param>
        /// <param name="diastolicOffsetScale"></param>
        /// <param name="spO2OffsetScale"></param>
        /// <param name="pacLevel">Integer between 0 and 3 representing the PAC level. 0 - dead, 1 - P1, 2 - P2, 3 - P3</param>
        /// <param name="treatmentTags"></param>
        public Injury(uint id, string printName, string description, EBodyPartId location, float damage, float damagePerSec,
            float heartRateOffsetScale, float respirationOffsetScale, float systolicOffsetScale, float diastolicOffsetScale, float spO2OffsetScale,
            int pacLevel, params string[] treatmentTags)
        {
            Debug.Assert(pacLevel >= 0 && pacLevel <= 3);

            m_Id = id;
            m_PrintName = printName;
            m_Description = description;
            m_BodyPartId = location;
            m_Damage = damage;
            m_DamagePerSec = damagePerSec;
            m_HeartRateOffsetScale = heartRateOffsetScale;
            m_RespirationOffsetScale = respirationOffsetScale;
            m_SystolicOffsetScale = systolicOffsetScale;
            m_DiastolicOffsetScale = diastolicOffsetScale;
            m_SpO2OffsetScale = spO2OffsetScale;

            switch (pacLevel)
            {
                case 0:
                    m_PAC = EPACS.P0;
                    break;

                case 1:
                    m_PAC = EPACS.P1;
                    break;

                case 2:
                    m_PAC = EPACS.P2;
                    break;

                case 3:
                    m_PAC = EPACS.P3;
                    break;
            }

            if (treatmentTags != null)
            {
                // check the tags in input, clean up and lower before joining them
                for (int i = 0; i < treatmentTags.Length; i++)
                {
                    if (string.IsNullOrEmpty(treatmentTags[i]))
                    {
                        Debug.Log("Error: Injury clear tag null or empty. Injury name: " + m_PrintName + " tag: " + treatmentTags[i]);
                        continue;
                    }
                    else
                    {
                        // convert the found treatment tag to lower acse
                        string lower = treatmentTags[i].ToLower();
                        if (DoesTreatmentTagExist(lower))
                            treatmentTags[i] = lower;
                    }
                }
                m_TreatmentTags = string.Join(",", treatmentTags);
            }
        }

        public void ApplyEffect(AVictim victim)
        {
            // FIXME: 
            victim.RemoveHealth(m_DamagePerSec / 4.0f);
        }

        public static bool DoesTreatmentTagExist(string tag)
        {
            Debug.Assert(!string.IsNullOrEmpty(tag));

            for (int i = 0; i < Consts.ClearInjuryTags.Length; i++)
            {
                if (tag.Equals(Consts.ClearInjuryTags[i]))
                    return true;
            }

            return false;
        }

        public string[] GetTreatmentTags()
        {
            return m_TreatmentTags.Split(',');
        }
    }
}
