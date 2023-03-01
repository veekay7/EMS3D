using UnityEngine;

namespace E3D
{
    // TODO: 
    public class AAmbulance : MonoBehaviour
    {
        // type of ambulance
        public enum EType { Ambulance, PatientBus }

        // current state of the ambulance
        public enum EState { Idle, Moving }

        // the direction of travel
        public enum EDirection { None = -1, ToEvacPoint = 0, ToDest = 1 }

        // the type of the trip
        public enum ETripType { Single, RoundTrip }

        public string m_PrintName;
        //public EType m_Type;
        public Sprite m_ThumbnailSprite;
        //public int m_MaxCapacity;

        private EState m_state;
        private ETripType m_tripType;                    // the type of the trip
        private EDirection m_direction;                  // the current moving direction
        private bool m_inUse;                            // is the ambulance in use by a player
        private float m_progress;                        // current travel progress
        private AVictim m_victim;                        // the victim inside the ambulance
        private AEvacPoint m_evacPoint;                  // the evac point the ambulance belongs to
        private Route m_route;                           // the route that it should take

        //public event ListChangedFuncDelegate<AVictim> onVictimsChangedFunc;


        public AVictim Victim { get => m_victim; }

        public AEvacPoint EvacPoint { get => m_evacPoint; }

        public Route CurrentRoute { get => m_route; }

        public EState CurrentState { get => m_state; }

        public EDirection MovingDirection { get => m_direction; }

        public bool InUse { get => m_inUse; }

        public bool IsFull { get => m_victim != null; }

        public float Progress { get => m_progress; }

        private void Awake()
        {
            m_state = EState.Idle;
            m_evacPoint = null;
            m_route = null;
            m_progress = 0.0f;
            m_inUse = false;
        }

        private void Reset()
        {
            m_PrintName = "Unknown";
            //m_Type = EType.Ambulance;
            //m_MaxCapacity = 1;
        }

        private void Start()
        {
            if (GameState.Current != null)
                GameState.Current.AddAmbulance(this);
        }

        private void Update()
        {
            if (m_state == EState.Moving)
            {
                if (m_progress < m_route.m_TravelTime)
                {
                    m_progress += Time.deltaTime;
                    return;
                }

                if (m_progress >= m_route.m_TravelTime)
                {
                    m_progress = 0.0f;

                    // if reached the hospital, unload the patient in the hospital
                    if (m_direction == EDirection.ToDest)
                    {
                        if (m_route.m_Location is AHospital)
                        {
                            // stop first!!
                            Stop();

                            // unload them victims
                            AHospital hospital = (AHospital)m_route.m_Location;
                            GameMode gameMode = GameMode.Current;

                            if (gameMode != null)
                            {
                                m_victim.m_State.m_IsEvacuated = true;

                                hospital.AddVictim(m_victim);

                                UnloadVictim(m_victim);

                                gameMode.m_State.m_EvacuatedCount++;
                            }

                            // if goping for a round trip, restart the progress back to 0
                            if (m_tripType == ETripType.RoundTrip)
                            {
                                Move(EDirection.ToEvacPoint, ETripType.Single);
                            }
                        }
                    }
                    else if (m_direction == EDirection.ToEvacPoint)
                    {
                        // if we are going back to the evac point
                        Stop();
                        ClearRoute();
                    }
                }
            }
        }

        public void SetEvacPoint(AEvacPoint evacPoint)
        {
            m_evacPoint = evacPoint;
        }

        public void Use(E3DPlayer player)
        {
            m_inUse = player != null ? true : false;
        }

        public void LoadVictim(AVictim newVictim)
        {
            m_victim = newVictim;
            if (m_victim != null)
            {
                m_victim.m_State.m_IsLoadedIntoVehicle = true;
            }
        }

        public void UnloadVictim(AVictim victim)
        {
            if (m_victim == victim)
            {
                m_victim.m_State.m_IsLoadedIntoVehicle = false;
                m_victim = null;
            }
        }

        public bool ContainsVictim(AVictim victim)
        {
            if (m_victim == victim)
                return true;

            return false;
        }

        public void SetDestination(Route newRoute)
        {
            Debug.Assert(newRoute != null);

            m_route = newRoute;
            m_direction = EDirection.None;
        }

        public void ClearRoute()
        {
            m_route = null;
            m_direction = EDirection.None;
        }

        public void Move(EDirection direction, ETripType tripType)
        {
            if (m_route != null)
            {
                m_direction = direction;
                m_tripType = tripType;
                m_state = EState.Moving;
            }
        }

        public void Stop()
        {
            m_direction = EDirection.None;
            m_state = EState.Idle;
        }

        private void OnDestroy()
        {
            if (GameState.Current != null)
                GameState.Current.RemoveAmbulance(this);
        }
    }
}
