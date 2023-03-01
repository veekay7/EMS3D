using UnityEngine;

namespace E3D
{
    public abstract class ALocationPoint : MonoBehaviour
    {
        public string m_LocationId;
        public string m_PrintName;

        public bool VehicleCanPass { get; protected set; }


        protected virtual void Awake() { return; }

        protected virtual void Start()
        {
            if (GameState.Current != null)
                GameState.Current.AddLocation(this);
        }

        protected virtual void Reset()
        {
            m_LocationId = "default";
            m_PrintName = "Default";
            VehicleCanPass = true;
        }

        protected virtual void OnDestroy()
        {
            if (GameState.Current != null)
                GameState.Current.RemoveLocation(this);
        }

        // editor only
        protected virtual void OnValidate() { return; }
    }
}
