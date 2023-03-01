using System;
using UnityEngine;
using UnityEngine.Events;

namespace E3D
{
    // patient actuity scale
    public enum EPACS
    {
        None = 0,
        P3 = 1,
        P2 = 2,
        P1 = 3,
        P0 = 4
    }

    // ids for interactable body parts
    public enum EBodyPartId
    {
        Unknown = 0,
        Head = 1,
        Face = 2,
        Neck = 3,
        Chest = 4,
        UpperLimbs = 5,
        Abdomen = 6,
        Pelvis = 7,
        LowerLimbs = 8,
        Skin = 9
    }

    [Serializable]
    public class PACSColorBlock
    {
        public Color m_P3Color = new Color(0.125f, 0.749f, 0.42f, 1.0f);
        public Color m_P2Color = new Color(0.992f, 0.588f, 0.267f, 1.0f);
        public Color m_P1Color = new Color(0.922f, 0.231f, 0.353f, 1.0f);
        public Color m_P0Color = new Color(0.173f, 0.243f, 0.314f, 1.0f);
        public Color m_OkColor = new Color(0.047f, 0.588f, 0.769f, 1.0f);
    }

    // delegates
    public enum EListOperation { Add, Remove }

    public delegate void ListChangedFuncDelegate<T>(EListOperation op, T oldItem, T newItem);

    public delegate void AreaChangedFuncDelegate(AVictimPlaceableArea oldArea, AVictimPlaceableArea newArea);

    // events

    public class EvacPointVehicleChangedFuncEvent : UnityEvent<EListOperation, AAmbulance, AAmbulance> { }

    //public class VictimSelectedEvent : UnityEvent<E3DGUIAmbulanceWindow> { }

    //public class HospitalSelectedEvent : UnityEvent<E3DGUIAmbulanceWindow, AHospital> { }
}