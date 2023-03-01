using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    public class GameState : MonoBehaviour
    {
        public EMatchState m_CurrentMatchState;
        public EMatchState m_LastMatchState;

        public ARouteController m_RouteController;

        public int m_Triage_CorrectCount;
        public int m_Triage_OverCount;
        public int m_Triage_UnderCount;
        public int m_TriagedCount;
        public int m_TreatedCount;
        public int m_EvacuatedCount;
        public int m_DeadCount;

        public List<AVictim> m_Victims = new List<AVictim>();
        public List<AAmbulance> m_Ambulances = new List<AAmbulance>();

        public List<AVictimPlaceableArea> m_CasualtyPoints = new List<AVictimPlaceableArea>();
        public List<AFirstAidPoint> m_FirstAidPoints = new List<AFirstAidPoint>();
        public List<AEvacPoint> m_EvacPoints = new List<AEvacPoint>();
        public List<AHospital> m_Hospitals = new List<AHospital>();
        public List<AAmbulanceDepot> m_AmbulanceDepots = new List<AAmbulanceDepot>();

        public List<AEmtTriageOffr> m_TriageOfficers = new List<AEmtTriageOffr>();
        public List<AEmtFirstAidDoc> m_FirstAidDocs = new List<AEmtFirstAidDoc>();
        public List<AEmtEvacOffr> m_EvacOfficers = new List<AEmtEvacOffr>();

        public static GameState Current { get; private set; }


        private void Awake()
        {
            if (Current != null && Current == this)
                return;

            Clear();
            Current = this;
        }

        public void AddVictim(AVictim victim)
        {
            if (!m_Victims.Contains(victim))
            {
                m_Victims.Add(victim);
            }
        }

        public void RemoveVictim(AVictim victim)
        {
            if (m_Victims.Contains(victim))
            {
                m_Victims.Remove(victim);
            }
        }

        public void AddLocation(ALocationPoint location)
        {
            if (location is AVictimPlaceableArea)
            {
                AVictimPlaceableArea area = (AVictimPlaceableArea)location;

                if (area is AFirstAidPoint)
                {
                    if (!m_FirstAidPoints.Contains((AFirstAidPoint)location))
                        m_FirstAidPoints.Add((AFirstAidPoint)location);
                }
                else if (area is AEvacPoint)
                {
                    if (!m_EvacPoints.Contains((AEvacPoint)location))
                        m_EvacPoints.Add((AEvacPoint)location);
                }
                else
                {
                    if (!m_CasualtyPoints.Contains(area))
                        m_CasualtyPoints.Add(area);
                }
            }
            else if (location is AHospital)
            {
                if (!m_Hospitals.Contains((AHospital)location))
                    m_Hospitals.Add((AHospital)location);
            }
            else if (location is AAmbulanceDepot)
            {
                if (!m_AmbulanceDepots.Contains((AAmbulanceDepot)location))
                    m_AmbulanceDepots.Add((AAmbulanceDepot)location);
            }
        }

        public void RemoveLocation(ALocationPoint location)
        {
            if (location is AVictimPlaceableArea)
            {
                AVictimPlaceableArea area = (AVictimPlaceableArea)location;

                if (area is AFirstAidPoint)
                {
                    if (m_FirstAidPoints.Contains((AFirstAidPoint)area))
                        m_FirstAidPoints.Remove((AFirstAidPoint)area);
                }
                else if (area is AEvacPoint)
                {
                    if (m_EvacPoints.Contains((AEvacPoint)area))
                        m_EvacPoints.Remove((AEvacPoint)area);
                }
                else
                {
                    if (m_CasualtyPoints.Contains(area))
                        m_CasualtyPoints.Remove(area);
                }
            }
            else if (location is AHospital)
            {
                if (m_Hospitals.Contains((AHospital)location))
                    m_Hospitals.Remove((AHospital)location);
            }
            else if (location is AAmbulanceDepot)
            {
                if (m_AmbulanceDepots.Contains((AAmbulanceDepot)location))
                    m_AmbulanceDepots.Remove((AAmbulanceDepot)location);
            }
        }

        public void AddAmbulance(AAmbulance newAmbulance)
        {
            if (!m_Ambulances.Contains(newAmbulance))
                m_Ambulances.Add(newAmbulance);
        }

        public void RemoveAmbulance(AAmbulance ambulance)
        {
            if (m_Ambulances.Contains(ambulance))
                m_Ambulances.Remove(ambulance);
        }

        public void AddEmtActor(AEmtBase emt)
        {
            if (emt is AEmtTriageOffr)
            {
                if (!m_TriageOfficers.Contains((AEmtTriageOffr)emt))
                {
                    m_TriageOfficers.Add((AEmtTriageOffr)emt);
                }
            }
            else if (emt is AEmtFirstAidDoc)
            {
                if (!m_FirstAidDocs.Contains((AEmtFirstAidDoc)emt))
                {
                    m_FirstAidDocs.Add((AEmtFirstAidDoc)emt);
                }
            }
            else if (emt is AEmtEvacOffr)
            {
                if (!m_EvacOfficers.Contains((AEmtEvacOffr)emt))
                {
                    m_EvacOfficers.Add((AEmtEvacOffr)emt);
                }
            }
        }

        public void RemoveEmtActor(AEmtBase officer)
        {
            if (officer is AEmtTriageOffr)
            {
                if (m_TriageOfficers.Contains((AEmtTriageOffr)officer))
                {
                    m_TriageOfficers.Remove((AEmtTriageOffr)officer);
                }
            }
            else if (officer is AEmtFirstAidDoc)
            {
                if (m_FirstAidDocs.Contains((AEmtFirstAidDoc)officer))
                {
                    m_FirstAidDocs.Remove((AEmtFirstAidDoc)officer);
                }
            }
            else if (officer is AEmtEvacOffr)
            {
                if (m_EvacOfficers.Contains((AEmtEvacOffr)officer))
                {
                    m_EvacOfficers.Remove((AEmtEvacOffr)officer);
                }
            }
        }

        public void Clear()
        {
            //m_GameType = EGameType.Singleplay;
            m_CurrentMatchState = EMatchState.BeforeEnter;
            m_LastMatchState = EMatchState.BeforeEnter;

            m_Triage_CorrectCount = 0;
            m_Triage_OverCount = 0;
            m_Triage_UnderCount = 0;
            m_TriagedCount = 0;
            m_TreatedCount = 0;
            m_EvacuatedCount = 0;
            m_DeadCount = 0;

            m_Victims.Clear();

            m_AmbulanceDepots.Clear();
            m_Hospitals.Clear();
            m_CasualtyPoints.Clear();
            m_FirstAidPoints.Clear();
            m_EvacPoints.Clear();

            m_TriageOfficers.Clear();
            m_FirstAidDocs.Clear();
            m_EvacOfficers.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
