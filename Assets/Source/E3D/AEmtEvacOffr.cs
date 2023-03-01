using UnityEngine;

namespace E3D
{
    public class AEmtEvacOffr : AEmtBase
    {
        [SerializeField, ReadOnlyVar]
        private AAmbulance m_curAmbulance;

        public event ListChangedFuncDelegate<AAmbulance> onAreaAmbulanceNumUpdateFunc;


        public AEvacPoint CurrentEvacPoint { get; private set; }

        public AAmbulance CurrentAmbulance { get => m_curAmbulance; }


        protected override void Awake()
        {
            base.Awake();

            m_curAmbulance = null;
        }

        protected override void Start()
        {
            base.Start();

            onAreaEnterExitFunc.AddListener(SetAmbulanceListChangedEvent);
        }

        private void SetAmbulanceListChangedEvent(AVictimPlaceableArea oldArea, AVictimPlaceableArea newArea)
        {
            if (oldArea != null)
                ((AEvacPoint)oldArea).ambulanceListChangedFunc -= Area_AmbulanceListChangedFunc;

            if (newArea != null)
                ((AEvacPoint)newArea).ambulanceListChangedFunc += Area_AmbulanceListChangedFunc;
        }

        private void Area_AmbulanceListChangedFunc(EListOperation op, AAmbulance oldItem, AAmbulance newItem)
        {
            if (onAreaAmbulanceNumUpdateFunc != null)
                onAreaAmbulanceNumUpdateFunc.Invoke(op, oldItem, newItem);
        }

        public override void EnterArea(AVictimPlaceableArea area)
        {
            base.EnterArea(area);

            if (m_curArea != null)
                CurrentEvacPoint = (AEvacPoint)m_curArea;
        }

        public void SetAmbulance(AAmbulance newAmbulance)
        {
            if (m_curAmbulance != null)
            {
                m_curAmbulance.Use(null);
                m_curAmbulance = null;
            }

            if (newAmbulance != null)
            {
                newAmbulance.Use(Player);

                m_curAmbulance = newAmbulance;
            }
        }

        public void SetHospitalToAmbulance(Route hospitalRoute)
        {
            if (m_curAmbulance == null)
                return;

            m_curAmbulance.SetDestination(hospitalRoute);
        }

        public void LoadVictimToAmbulance(AVictim victim)
        {
            if (m_curAmbulance == null)
                return;

            if (victim.IsPlayerUsing)
            {
                SendResponse("cannot_load_victim", victim.m_GivenName + " is in use.");
                return;
            }

            if (!victim.IsPlayerUsing && !victim.m_State.m_IsLoadedIntoVehicle)
            {
                // set flags for the vicitm
                victim.Use(Player);

                // load the victim into the ambulance
                m_curAmbulance.LoadVictim(victim);

                // remove the victim from the area
                CurrentEvacPoint.RemoveVictim(victim);

                SendResponse("ambulance_load_changed", victim.m_GivenName + " loaded into ambulance.", victim);
            }
        }

        public void UnloadVictimFrmAmbulance()
        {
            if (m_curAmbulance == null)
                return;

            if (m_curAmbulance.Victim != null)
            {
                AVictim victim = m_curAmbulance.Victim;

                // set the flags for the victim
                victim.Use(null);

                // unload the victim from the ambulance
                m_curAmbulance.UnloadVictim(victim);

                // move the victim back to the evac point area
                CurrentEvacPoint.AddVictim(victim);

                SendResponse("ambulance_load_changed", victim.m_GivenName + " unloaded from ambulance.", victim);
            }
        }

        public void EvacuateVictim()
        {
            if (m_curAmbulance == null)
                return;

            if (m_curAmbulance.Victim == null)
            {
                SendResponse("error", "No victim loaded in ambulance, cannot evacuate.");
            }
            else if (m_curAmbulance.CurrentRoute == null)
            {
                SendResponse("error", "Ambulance has no destination set.");
            }
            else
            {
                m_player.m_State.m_TotalVictimsAttendedNum++;

                m_curAmbulance.Move(AAmbulance.EDirection.ToDest, AAmbulance.ETripType.RoundTrip);
                
                GameMode.Current.ProcessEvacScoring(m_player, m_curAmbulance);
                
                SendResponse("success", "Ambulance is now moving to hospital.");

                SetAmbulance(null);
            }
        }

        public void RequestAmbulance(AAmbulance.EType ambulanceType)
        {
            // TODO: 
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            onAreaEnterExitFunc.AddListener(SetAmbulanceListChangedEvent);
        }
    }
}
