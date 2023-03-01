using System;
using UnityEngine;

namespace E3D
{
    public class E3DPlayerState : MonoBehaviour
    {
        [Header("Triage Officer State")]
        public float m_TriageActionScore;
        public float m_TriageDmgScore;
        public int m_CorrectTriageP3Num;
        public int m_CorrectTriageP2Num;
        public int m_CorrectTriageP1Num;
        public int m_CorrectTriageDeadNum;
        public int m_UnderTriageNum;
        public int m_OverTriageNum;
        public float m_TotalTriageTime;

        [Header("First Aid Point Officer State")]
        //public SyncList<E3D_VictimTreatmentState> m_TreatmentStates = new SyncList<E3D_VictimTreatmentState>();
        public bool m_WasDeadWhileInCare;
        public int m_TotalVictimsAttendedNum;
        public int m_TotalTreatmentNum;
        public int m_CorrectTreatmentNum;
        public float m_TreatmentActionScore;
        public float m_TreatmentDmgScore;
        public float m_TotalTreatmentTime;

        [Header("Evacuation Officer State")]
        public float m_EvacActionScore;
        public float m_EvacDmgScore;

        public E3DPlayer Player { get; private set; }


        private void Awake()
        {
            // triage officer state
            //m_AttendedVictimNum = 0;
            m_CorrectTriageP3Num = 0;
            m_CorrectTriageP2Num = 0;
            m_CorrectTriageP1Num = 0;
            m_CorrectTriageDeadNum = 0;
            m_UnderTriageNum = 0;
            m_OverTriageNum = 0;

            //// first aid point doc state
            //m_IsPerfect = true;

            //// evac officer state
            //m_SuccessfulEvacCount = 0;
        }

        public void SetPlayer(E3DPlayer player)
        {
            Player = player;
        }
    }


    // TODO: consider putting some of the vars in this in the victim state for tracking
    // the same variables can then be added to the player state
    [Serializable]
    public class E3D_VictimTreatmentState
    {
        public uint m_VictimNetId;              // the victim that was being treated
        public bool m_WasDeadWhileInCare;       // did the victim die while being in care
        public int m_TotalTreatmentNum;         // total treatments applied
        public int m_CorrectTreatmentNum;       // total correct treatments applied
        public int m_BadTreatmentNum;           // total useless treatments applied
        public int m_FailedTreatmentCount;      // total failed treatments applied
        public float m_TotalTreatmentTime;      // total treatment time


        public bool IsPerfect()
        {
            return (m_FailedTreatmentCount != 0) && (m_BadTreatmentNum != 0);
        }
    }
}
