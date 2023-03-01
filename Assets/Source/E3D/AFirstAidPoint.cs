using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    public class AFirstAidPoint : AVictimPlaceableArea
    {
        public static int MAX_ITEM_NUM = 8;

        public ItemAttrib[] m_ItemAttribs = new ItemAttrib[MAX_ITEM_NUM];
        public Sprite[] m_ItemSprites = new Sprite[MAX_ITEM_NUM];

        [SerializeField, ReadOnlyVar]
        public List<int> m_ItemQuantities = new List<int>();

        private int[] m_itemQuantitiesCopy = new int[MAX_ITEM_NUM];

        public ItemAttrib[] ItemAttribs
        {
            get => m_ItemAttribs;
        }


        protected override void Awake()
        {
            base.Awake();

            VehicleCanPass = false;
        }

        protected override void Start()
        {
            for (int i = 0; i < MAX_ITEM_NUM; i++)
            {
                if (m_ItemAttribs[i] != null)
                    m_ItemQuantities.Add(m_ItemAttribs[i].m_DefaultCarry);
                else
                    m_ItemQuantities.Add(-1);
            }

            base.Start();
        }

        /// <summary>
        /// Use an item from the first aid point.
        /// Codes:
        ///     -1: No item effect applied to item.
        ///      0: Success
        ///      1: No effect
        ///      2: Insufficient quantities
        /// </summary>
        /// <param name="itemIdx"></param>
        /// <param name="victim"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public int UseItem(int itemIdx, AVictim victim, out string msg)
        {
            ItemAttrib attrib = m_ItemAttribs[itemIdx];

            // if insufficient, we get outta here!!
            if (m_ItemQuantities[itemIdx] == 0 && !attrib.m_IsInfinite)
            {
                msg = "ERROR: Insufficient quantity of " + attrib.m_PrintName.ToUpper() + ". ";
                return 2;
            }
            
            // reduce quantity if not infinite
            if (!attrib.m_IsInfinite)
            {
                m_ItemQuantities[itemIdx] -= 1;
                if (m_ItemQuantities[itemIdx] < 0)
                    m_ItemQuantities[itemIdx] = 0;
            }

            // apply effect
            ItemEffect effect = attrib.m_Effect;
            if (effect == null)
            {
                msg = "ERROR: No ItemEffect applied to item attrib " + attrib.m_PrintName.ToUpper();
                return -1;
            }

            msg = "Used " + attrib.m_PrintName.ToUpper() + ". ";

            // if applied effect doesn't work, then say it doesn't work and get outta here!!
            if (!effect.ApplyEffect(victim))
            {
                msg = "\n" + effect.m_NothingMsg;
                return 1;
            }

            return 0;
        }

        public int FindItemIdxByItemFlag(EItemIdFlag itemId)
        {
            for (int i = 0; i < m_ItemAttribs.Length; i++)
            {
                if (m_ItemAttribs[i].m_Id == itemId)
                    return i;
            }

            return -1;
        }
    }
}
