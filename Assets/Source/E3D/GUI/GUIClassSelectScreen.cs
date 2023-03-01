using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace E3D
{
    public class GUIClassSelectScreen : GUIScreen
    {
        public const int CLASS_TRIAGE_OFFR = 0;
        public const int CLASS_FIRST_AID_DOC = 1;
        public const int CLASS_EVAC_OFFR = 2;
        public const int CLASS_GAME_MASTER = 3;

        [Header("Class Select Screen")]
        public Button m_BtnConfirm;
        public TMP_Text m_TxtSelected;

        private int m_selectedActorId;


        protected override void Awake()
        {
            base.Awake();

            m_Classname = "ClassSelect";
            m_selectedActorId = -1;
        }

        protected override void Reset()
        {
            base.Reset();
            m_Classname = "ClassSelect";
        }

        private void LateUpdate()
        {
            m_BtnConfirm.interactable = m_selectedActorId >= 0;
        }

        public void SelectPlayerClass(int classId)
        {
            if (GameSystem.Instance == null)
            {
                Debug.Log("No GameSystem in the scene, cannot spawn EMT actor.");
                return;
            }

            // TODO: fix this shit so that the best you can pick is evac offr
            m_selectedActorId = classId;
            switch (m_selectedActorId)
            {
                case CLASS_TRIAGE_OFFR:
                    m_TxtSelected.text = ("triage officer").ToUpper();
                    break;

                case CLASS_FIRST_AID_DOC:
                    m_TxtSelected.text = ("first aid point doctor").ToUpper();
                    break;

                case CLASS_EVAC_OFFR:
                    m_TxtSelected.text = ("evacuation officer").ToUpper();
                    break;

                default:
                    m_TxtSelected.text = "-";
                    break;
            }
        }

        public void PossessSelectedActor()
        {
            if (m_selectedActorId < 0 || m_selectedActorId > 2)
                return;

            if (E3DPlayer.Local != null)
            {
                AEmtBase emt = null;
                switch (m_selectedActorId)
                {
                    case CLASS_TRIAGE_OFFR:
                        emt = GameSystem.Instance.Spawn<AEmtTriageOffr>();
                        break;

                    case CLASS_FIRST_AID_DOC:
                        emt = GameSystem.Instance.Spawn<AEmtFirstAidDoc>();
                        break;

                    case CLASS_EVAC_OFFR:
                        emt = GameSystem.Instance.Spawn<AEmtEvacOffr>();
                        break;
                }

                if (emt != null)
                {
                    E3DPlayer.Local.Possess(emt);
                    E3DPlayer.Local.OpenBriefingScreen();
                }
            }
        }
    }
}
