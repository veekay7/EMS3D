using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    [RequireComponent(typeof(BoxCollider))]
    public class BodyPartVolume : MonoBehaviour
    {
        [HideInInspector]
        public BoxCollider m_Collider;
        public List<EBodyPartId> m_BodyPartIds = new List<EBodyPartId>();


        private void Reset()
        {
            m_BodyPartIds.Clear();
        }

        private void Awake()
        {
            m_Collider = GetComponent<BoxCollider>();
        }

        public bool ContainsBodyPartId(EBodyPartId id)
        {
            return m_BodyPartIds.Contains(id);
        }

        public bool CanApplyItemOnBodyPart(ItemAttrib item)
        {
            for (int i = 0; i < item.m_BodyPartIds.Count; i++)
            {
                for (int y = 0; y < m_BodyPartIds.Count; y++)
                {
                    if (item.m_BodyPartIds[i] == m_BodyPartIds[y])
                        return true;
                }
            }

            return false;
        }

        private void OnValidate()
        {
            if (m_Collider == null)
            {
                m_Collider = GetComponent<BoxCollider>();
                if (m_Collider == null)
                    m_Collider = gameObject.AddComponent<BoxCollider>();
            }
        }
    }
}
