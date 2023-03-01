using System;
using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    public class AHospital : ALocationPoint
    {
        [Header("Hospital")]
        public Sprite m_Logo;
        public int m_MaxCapacity;
        public Specialties m_Specialties;

        [SerializeField]
        private List<AVictim> m_victimList = new List<AVictim>();

        public event ListChangedFuncDelegate<AVictim> victimListChangedFunc;

        public int NumVictims { get => m_victimList.Count; }


        protected override void Awake()
        {
            base.Awake();

            VehicleCanPass = true;
        }

        protected override void Reset()
        {
            base.Reset();

            m_Logo = null;
            m_MaxCapacity = 10;
        }

        public void AddVictim(AVictim newVictim)
        {
            if (!m_victimList.Contains(newVictim))
            {
                newVictim.m_IsActive = false;

                m_victimList.Add(newVictim);

                if (victimListChangedFunc != null)
                    victimListChangedFunc.Invoke(EListOperation.Add, null, newVictim);
            }
        }

        public void RemoveVictim(AVictim victim)
        {
            if (m_victimList.Contains(victim))
            {
                m_victimList.Remove(victim);

                if (victimListChangedFunc != null)
                    victimListChangedFunc.Invoke(EListOperation.Remove, victim, null);
            }
        }
    }


    [Serializable]
    public struct Specialties
    {
        public bool m_WomenTreatment;
        public bool m_PaediatricTreatment;
        public bool m_Burns;


        public override string ToString()
        {
            List<string> specialtyStringList = new List<string>();
            if (m_WomenTreatment)
                specialtyStringList.Add("women");
            if (m_PaediatricTreatment)
                specialtyStringList.Add("paediatrics");
            if (m_Burns)
                specialtyStringList.Add("burns");

            string outputString = null;
            if (specialtyStringList.Count != 0)
            {
                for (int i = 0; i < specialtyStringList.Count; i++)
                {
                    outputString += specialtyStringList[i];
                    if (i != specialtyStringList.Count - 1)
                        outputString += ",";
                }
            }
            else
            {
                outputString = "No specialties available.";
            }

            return outputString;
        }
    }
}
