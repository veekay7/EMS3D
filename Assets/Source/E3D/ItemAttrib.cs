using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    public enum EItemIdFlag
    {
        Unknown = -1, Analgesia, ChestTube, Immobilization, Intubation, IVDrip, O2Tank, Torniquet, WoundDressing
    }

    public class ItemAttrib : ScriptableObject
    {
        public EItemIdFlag m_Id;
        public string m_PrintName;
        public Sprite m_Sprite;
        [Multiline(3)]
        public string m_Desc;
        public bool m_IsInfinite;
        public int m_DefaultCarry;
        public int m_MaxCarry;
        public ItemEffect m_Effect;
        public List<EBodyPartId> m_BodyPartIds = new List<EBodyPartId>();


        public static ItemAttrib Create(EItemIdFlag id, string printName, Sprite sprite, string desc, bool isInfinite, int defaultAmount, int maxAmount)
        {
            var newItemAttrib = CreateInstance<ItemAttrib>();

            newItemAttrib.m_Id = id;
            newItemAttrib.m_PrintName = printName;
            newItemAttrib.m_Sprite = sprite;
            newItemAttrib.m_Desc = desc;
            newItemAttrib.m_IsInfinite = isInfinite;
            newItemAttrib.m_DefaultCarry = defaultAmount;
            newItemAttrib.m_MaxCarry = maxAmount;

            if (string.IsNullOrEmpty(newItemAttrib.m_PrintName))
                newItemAttrib.m_PrintName = "Item";

            if (string.IsNullOrEmpty(newItemAttrib.m_Desc))
                newItemAttrib.m_Desc = "No information available";

            return newItemAttrib;
        }

        private void Reset()
        {
            m_Id = EItemIdFlag.Unknown;
            m_PrintName = "Item";
            m_Sprite = null;
            m_Desc = "No description available.";
            m_IsInfinite = false;
            m_DefaultCarry = 0;
            m_MaxCarry = 1;
        }

        public bool ContainsBodyPartId(EBodyPartId id)
        {
            return m_BodyPartIds.Contains(id);
        }
    }
}
