using UnityEngine;

namespace E3D
{
    public class VictimState : MonoBehaviour
    {
        public bool m_IsTriaged;
        public bool m_IsTreated;
        public bool m_IsEvacuated;
        public bool m_CheckedCanWalk;
        public bool m_CheckedHeartRate;
        public bool m_CheckedRespiration;
        public bool m_CheckedBloodPressure;
        public bool m_CheckedSpO2;
        public bool m_CheckedGCS;
        public bool m_IsLoadedIntoVehicle;
        public EPACS m_GivenPACS;
        public float m_AmbulanceTime;   // i know it makes no sense, but shaddap about it


        private void Awake()
        {
            Clear();
        }

        private void Reset()
        {
            Clear();
        }

        public void Clear()
        {
            m_IsTriaged = false;
            m_IsTreated = false;
            m_IsEvacuated = false;
            m_CheckedCanWalk = false;
            m_CheckedHeartRate = false;
            m_CheckedRespiration = false;
            m_CheckedBloodPressure = false;
            m_CheckedSpO2 = false;
            m_CheckedGCS = false;
            m_GivenPACS = EPACS.None;
            m_AmbulanceTime = 0.0f;
        }

        public void SetAllVitalsCheckedFlag(bool value)
        {
            m_CheckedCanWalk = value;
            m_CheckedHeartRate = value;
            m_CheckedRespiration = value;
            m_CheckedBloodPressure = value;
            m_CheckedSpO2 = value;
            m_CheckedGCS = value;
        }

        public void CmdSetIsTriagedFlag(bool value) { m_IsTriaged = value; }

        public void CmdSetIsTreatedFlag(bool value) { m_IsTreated = value; }

        public void CmdSetIsEvacuatedFlag(bool value) { m_IsEvacuated = value; }

        public void CmdSetCanWalkFlag(bool value) { m_CheckedCanWalk = value; }

        public void CmdSetCheckedHeartRateFlag(bool value) { m_CheckedHeartRate = value; }

        public void CmdSetCheckedRespirationFlag(bool value) { m_CheckedRespiration = value; }

        public void CmdSetCheckedBloodPressureFlag(bool value) { m_CheckedBloodPressure = value; }

        public void CmdSetCheckedSpO2Flag(bool value) { m_CheckedSpO2 = value; }

        public void CmdSetCheckedCRTFlag(bool value) { m_CheckedGCS = value; }

        public void CmdSetGivenTag(EPACS pac) { m_GivenPACS = pac; }

        public bool Equals(VictimState other)
        {
            bool isEqual = this.m_IsTriaged == other.m_IsTriaged &&
                this.m_IsTreated == other.m_IsTreated &&
                this.m_IsEvacuated == other.m_IsEvacuated &&
                this.m_CheckedCanWalk == other.m_CheckedCanWalk &&
                this.m_CheckedHeartRate == other.m_CheckedHeartRate &&
                this.m_CheckedRespiration == other.m_CheckedRespiration &&
                this.m_CheckedBloodPressure == other.m_CheckedBloodPressure &&
                this.m_CheckedSpO2 == other.m_CheckedSpO2 &&
                this.m_CheckedGCS == other.m_CheckedGCS &&
                this.m_GivenPACS == other.m_GivenPACS;
            return isEqual;
        }
    }
}
