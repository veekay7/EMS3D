using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    public static class InjuryList
    {
        private static List<Injury> m_injuries = new List<Injury>();
        private static List<Injury> m_injuriesP3 = new List<Injury>();
        private static List<Injury> m_injuriesP2 = new List<Injury>();
        private static List<Injury> m_injuriesP1 = new List<Injury>();
        private static List<Injury> m_injuriesP0 = new List<Injury>();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init()
        {
            Add(new Injury(1, "Brain herniation", "Brain content out from skull.", EBodyPartId.Head, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 0, "no action"));
            Add(new Injury(2, "Severe neck deformity", "Neck deformed.", EBodyPartId.Neck, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 0, "no action"));
            Add(new Injury(3, "Subtotal decapitation.", "Head almost dropped off.", EBodyPartId.Neck, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 0, "no action"));
            Add(new Injury(4, "Total decapitation", "Head totally dropped off.", EBodyPartId.Head, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 0, "no action"));
            Add(new Injury(5, "Severe evisceration", "Bowel drops out.", EBodyPartId.Abdomen, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 0, "no action"));
            Add(new Injury(6, "Facial burn unrecognisable features", "Facial burn totally damage face.", EBodyPartId.Face, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 0, "no action"));
            Add(new Injury(7, "Depressed skull fracture.", "Skull depressed.", EBodyPartId.Head, 40.0f, 2.5f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, 1, "intubation", "oxygen"));
            Add(new Injury(8, "Skull open fracture", "Skull fracture with overlying open wound.", EBodyPartId.Head, 40.0f, 2.5f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, 1, "intubation", "oxygen"));
            Add(new Injury(9, "Severe facial injury", "Facial deformity.", EBodyPartId.Abdomen, 40.0f, 2.5f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, 1, "intubation", "oxygen"));
            Add(new Injury(10, "Facial open fracture", "Facial fracture with overlying open wound.", EBodyPartId.Abdomen, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "intubation", "intubation"));
            Add(new Injury(11, "Sucking chest wound", "Chest wound bubbling.", EBodyPartId.Chest, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "chest tube", "oxygen"));
            Add(new Injury(12, "Flail chest", "Segment of chest wall loose.", EBodyPartId.Face, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "chest tube", "oxygen"));
            Add(new Injury(13, "70 % BSA 2nd burn", "70% body surface area painful, blister, wet looking burn.", EBodyPartId.Skin, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "IV drip", "dressing", "analgesia"));
            Add(new Injury(14, "Impaled object on chest", "Something stuck into chest.", EBodyPartId.Face, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "IV drip", "analgesia"));
            Add(new Injury(15, "Impaled object on abdomen", "Something stuck into abdomen.", EBodyPartId.Abdomen, 20.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "IV drip", "analgesia"));
            Add(new Injury(16, "50 % BSA 2nd burn", "50% body surface area painful, blister, wet looking burn.", EBodyPartId.Skin, 30.0f, 2, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "IV drip", "dressing", "analgesia"));
            Add(new Injury(17, "Neck wound with gurgling", "Neck open wound bubbling.", EBodyPartId.Neck, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "intubation", "oxygen"));
            Add(new Injury(18, "Penetrating chest wound", "Chest open wound caused by sharp object.", EBodyPartId.Face, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "chest tube", "oxygen"));
            Add(new Injury(19, "Tension pneumothorax", "Building pressure in chest.", EBodyPartId.Face, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "chest tube", "oxygen"));
            Add(new Injury(20, "Facial burn", "Face burn.", EBodyPartId.Face, 40.0f, 2.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 1, "analgesia"));
            Add(new Injury(21, "Scalp laceration", "Scalp cut.", EBodyPartId.Head, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(22, "Scalp hematoma", "Scalp baluku.", EBodyPartId.Head, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(23, "Scalp abrasion", "Scalp abrasion.", EBodyPartId.Head, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(24, "Traumatic brain injury", "Severe brain injury.", EBodyPartId.Head, 30.0f, 2, -1.0f, -1.0f, 1.0f, 1.0f, -1.0f, 1, "intubation", "oxygen"));
            Add(new Injury(25, "Facial laceration", "Facial cut.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(26, "Facial abrasion", "Facial abrasion.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(27, "Bleeding from nose", "Nose bleeding.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(28, "Bleeding from ear", "Ear bleeding.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(29, "Facial hematoma", "Face baluku.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(30, "Singed hair", "Hair burn until charcoal.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "oxygen"));
            Add(new Injury(31, "Blood in eye", "Blood in eye.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(32, "Neck laceration", "Neck cut.", EBodyPartId.Neck, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(33, "Neck abrasion", "Neck abrasion.", EBodyPartId.Neck, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(34, "Severe neck pain", "Neck pain.", EBodyPartId.Neck, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(35, "Chest burn", "Chest burn.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(36, "Chest hematoma", "Chest baluku.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(37, "Chest abrasion", "Chest abrasion.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(38, "Abdominal burn", "Abdomen burn.", EBodyPartId.Face, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(39, "Abdominal abrasion", "Abdomen abrasion.", EBodyPartId.Abdomen, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "dressing"));
            Add(new Injury(40, "Abdominal hematoma", "Abdomen baluku.", EBodyPartId.Abdomen, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(41, "Open abdominal wound", "Abdomen big open wound.", EBodyPartId.Abdomen, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "IV drip", "analgesia", "dressing"));
            Add(new Injury(42, "Open wound on groin", "Groin open wound.", EBodyPartId.Abdomen, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "IV drip", "analgesia", "dressing"));
            Add(new Injury(43, "Fracture (pelvic)", "Involved pelvis deformed, swelling, pain.", EBodyPartId.Pelvis, 30.0f, 2, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "immobilization"));
            Add(new Injury(44, "Fracture (humeral)", "Involved arm deformed, swelling, pain.", EBodyPartId.UpperLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "immobilization"));
            Add(new Injury(45, "Fracture (elbow / forearm / wrist / finger)", "Involved hand deformed, swelling, pain.", EBodyPartId.UpperLimbs, 15.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "immobilization"));
            Add(new Injury(46, "Amputation (thigh)", "Involved limb cut (thigh).", EBodyPartId.LowerLimbs, 30.0f, 2, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "torniquet"));
            Add(new Injury(47, "Amputation (arm)", "Involved limb cut (arm).", EBodyPartId.UpperLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "torniquet"));
            Add(new Injury(48, "Amputation (forearm / wrist / fingers)", "Involved limb cut.", EBodyPartId.UpperLimbs, 15.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "IV drip", "analgesia", "torniquet"));
            Add(new Injury(49, "Dislocation (shoulder / elbow / wrist)", "Involved limb deformed, swelling, pain.", EBodyPartId.UpperLimbs, 15.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "immobilization"));
            Add(new Injury(50, "Open fracture (humeral / elbow / forearm / wrist / finger)", "Involved limb deformed with open wound.", EBodyPartId.UpperLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "immobilization"));
            Add(new Injury(51, "Mangled upper limbs", "Mangled upper limbs.", EBodyPartId.UpperLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "immobilization"));
            Add(new Injury(52, "Degloving injury of hand", "Hand skin come out like a glove.", EBodyPartId.UpperLimbs, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia"));
            Add(new Injury(53, "Crush injury (right / left / bilateral)(upper limbs)", "Involved limb crushed with pain and deformity.", EBodyPartId.UpperLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia"));
            Add(new Injury(54, "30 % BSA 1st burn", "30% body surface area burn, looks like sunburn, scalded.", EBodyPartId.Skin, 10.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "IV drip", "dressing", "analgesia"));
            //Add(new E3D_Injury(55, "No injury", "No injury.", EBodyPartId.Unknown, 0.0f, 0, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 3, "no action"));
            Add(new Injury(55, "Dislocation (femur / knee / ankle)", "Involved limb deformed, swelling, pain.", EBodyPartId.LowerLimbs, 15.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "immobilization"));
            Add(new Injury(56, "Crush injury (right / left / bilateral)(lower limbs)", " Involved limb crushed with pain and deformity.", EBodyPartId.LowerLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia"));
            Add(new Injury(57, "Mangled lower limbs", "Mangled lower limbs.", EBodyPartId.LowerLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "immobilization"));
            Add(new Injury(58, "Open fracture (femur / tibia / fibula)", "Involved limb deformed with open wound (femur/tibia/fibula).", EBodyPartId.LowerLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "immobilization"));
            Add(new Injury(59, "Open fracture (pelvic)", "Involved pelvis deformed with open wound.", EBodyPartId.Pelvis, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "immobilization"));
            Add(new Injury(60, "Fracture (femur)", "Involved thigh deformed, swelling, pain (femur).", EBodyPartId.LowerLimbs, 20.0f, 1, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 2, "IV drip", "analgesia", "immobilization"));
            Add(new Injury(61, "Fracture (tibia / fibula / ankle)", "Involved leg deformed, swelling, pain (tibia/fibula/ankle).", EBodyPartId.LowerLimbs, 15.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "analgesia", "immobilization"));
            Add(new Injury(62, "Amputation (leg / foot)", "Involved limb cut (leg/foot).", EBodyPartId.LowerLimbs, 15.0f, 0.5f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, 3, "IV drip", "analgesia", "torniquet"));

            Debug.Log("Injuries initialised!");
        }

        public static void Add(Injury injury)
        {
            m_injuries.Add(injury);

            if (injury.m_PAC == EPACS.P3)
                m_injuriesP3.Add(injury);
            else if (injury.m_PAC == EPACS.P2)
                m_injuriesP2.Add(injury);
            else if (injury.m_PAC == EPACS.P1)
                m_injuriesP1.Add(injury);
            else if (injury.m_PAC == EPACS.P0)
                m_injuriesP0.Add(injury);
        }

        public static Injury FindByIndex(int idx)
        {
            return m_injuries[idx];
        }

        public static Injury FindById(uint id)
        {
            for (int i = 0; i < m_injuries.Count; i++)
            {
                if (id == m_injuries[i].m_Id)
                    return m_injuries[i];
            }

            return null;
        }

        public static Injury FindRandom()
        {
            if (m_injuries == null || m_injuries.Count == 0)
                return null;

            int index = UnityEngine.Random.Range(0, m_injuries.Count);
            return m_injuries[index];
        }

        public static Injury FindByPACTag(EPACS pac)
        {
            if (m_injuries == null || m_injuries.Count == 0)
                return null;

            if (pac == EPACS.P3)
            {
                int randIdx = UnityEngine.Random.Range(0, m_injuriesP3.Count);
                return m_injuriesP3[randIdx];
            }
            else if (pac == EPACS.P2)
            {
                int randIdx = UnityEngine.Random.Range(0, m_injuriesP2.Count);
                return m_injuriesP2[randIdx];
            }
            else if (pac == EPACS.P1)
            {
                int randIdx = UnityEngine.Random.Range(0, m_injuriesP1.Count);
                return m_injuriesP1[randIdx];
            }
            else if (pac == EPACS.P0)
            {
                int randIdx = UnityEngine.Random.Range(0, m_injuriesP0.Count);
                return m_injuriesP0[randIdx];
            }

            return null;
        }

        public static int IndexOf(Injury injury)
        {
            return m_injuries.IndexOf(injury);
        }
    }
}
