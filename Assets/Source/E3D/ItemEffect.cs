using UnityEngine;

namespace E3D
{
    public class ItemEffect : ScriptableObject
    {
        public EItemIdFlag m_SelectedEffect;
        public string m_UseMsg = "Treatment applied.";
        public string m_NothingMsg = "Nothing happened.";


        public bool ApplyEffect(AVictim victim)
        {
            switch (m_SelectedEffect)
            {
                case EItemIdFlag.Unknown:
                    break;

                case EItemIdFlag.Analgesia:
                    return ClearTreatmentTagEffect("analgesia", victim);

                case EItemIdFlag.ChestTube:
                    return ClearTreatmentTagEffect("chest tube", victim);

                case EItemIdFlag.Immobilization:
                    return ClearTreatmentTagEffect("immobilization", victim);

                case EItemIdFlag.Intubation:
                    return ClearTreatmentTagEffect("intubation", victim);

                case EItemIdFlag.IVDrip:
                    return ClearTreatmentTagEffect("IV drip", victim);

                case EItemIdFlag.O2Tank:
                    return ClearTreatmentTagEffect("oxygen", victim);

                case EItemIdFlag.Torniquet:
                    return ClearTreatmentTagEffect("torniquet", victim);

                case EItemIdFlag.WoundDressing:
                    return ClearTreatmentTagEffect("dressing", victim);
            }

            return false;
        }

        private bool ClearTreatmentTagEffect(string treatmentTag, AVictim victim)
        {
            if (victim.ContainsTreatmentTag(treatmentTag))
            {
                victim.RemoveTreatmentTag(treatmentTag);
                return true;
            }

            return false;
        }
    }
}
