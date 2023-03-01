using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    public class AEvacPoint : AVictimPlaceableArea
    {
        public int m_MaxCapacity;

        [SerializeField, ReadOnlyVar]
        private List<AAmbulance> m_ambulances = new List<AAmbulance>();

        public event ListChangedFuncDelegate<AAmbulance> ambulanceListChangedFunc;


        public int AmbulanceNum { get => m_ambulances.Count; }

        protected override void Awake()
        {
            base.Awake();

            VehicleCanPass = true;
        }

        public void AddAmbulance(AAmbulance newAmbulance)
        {
            if (!m_ambulances.Contains(newAmbulance))
            {
                m_ambulances.Add(newAmbulance);

                if (ambulanceListChangedFunc != null)
                    ambulanceListChangedFunc.Invoke(EListOperation.Add, null, newAmbulance);
            }
        }

        public void RemoveAmbulance(AAmbulance ambulance)
        {
            if (m_ambulances.Contains(ambulance))
            {
                m_ambulances.Remove(ambulance);
                if (ambulanceListChangedFunc != null)
                    ambulanceListChangedFunc.Invoke(EListOperation.Remove, ambulance, null);
            }
        }

        public AAmbulance[] GetAmbulances()
        {
            return m_ambulances.ToArray();
        }
    }
}
