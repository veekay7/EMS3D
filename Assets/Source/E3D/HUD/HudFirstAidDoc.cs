using UnityEngine;
using UnityEngine.UI;

namespace E3D
{
    public class HudFirstAidDoc : Hud
    {
        public const int VICTIM_SELECT_SCREEN = 0;
        public const int TREATMENT_SCREEN = 1;
        public const int APPLY_TREATMENT_SCREEN = 2;

        [Header("First Aid Doc HUD")]
        public Image m_ImgBackground;
        public Image m_ImgVictim;
        public GUIVictimCardList m_VictimCardList;

        private AEmtFirstAidDoc m_firstAidDoc;
        private int m_selItemIdx;

        public AEmtFirstAidDoc PossessedEmt { get => m_firstAidDoc; }

        public int SelectedItemIndex { get => m_selItemIdx; }


        protected override void Awake()
        {
            base.Awake();

            m_selItemIdx = -1;
        }

        protected override void Start()
        {
            base.Start();

            m_VictimCardList.onCardClickedFunc.AddListener(VictimCard_Clicked);
        }

        public override void SetPlayer(E3DPlayer newPlayer)
        {
            base.SetPlayer(newPlayer);

            if (m_firstAidDoc != null)
            {
                m_firstAidDoc.onAreaEnterExitFunc.RemoveListener(Callback_AreaEnterExit);
                m_firstAidDoc.onAreaVictimNumChangedFunc.RemoveListener(Callback_AreaVictimChanged);
                m_firstAidDoc.onResponseRecvFunc.RemoveListener(Callback_ResponseReceived);

                m_firstAidDoc = null;
            }

            if (Player != null)
            {
                if (Player.PossessedEmt != null)
                {
                    m_firstAidDoc = (AEmtFirstAidDoc)Player.PossessedEmt;

                    m_firstAidDoc.onAreaEnterExitFunc.AddListener(Callback_AreaEnterExit);
                    m_firstAidDoc.onAreaVictimNumChangedFunc.AddListener(Callback_AreaVictimChanged);
                    m_firstAidDoc.onResponseRecvFunc.AddListener(Callback_ResponseReceived);
                }
            }
        }

        private void Callback_AreaEnterExit(AVictimPlaceableArea oldArea, AVictimPlaceableArea newArea)
        {
            if (oldArea != null && oldArea == m_firstAidDoc.CurrentArea)
            {
                m_ImgBackground.sprite = null;
                m_ImgBackground.enabled = false;

                CloseCurrentView();

                Player.CanMove = true;
            }

            if (newArea != null)
            {
                m_ImgBackground.sprite = newArea.m_BackgroundSprite;
                m_ImgBackground.enabled = true;

                OpenView(m_CachedViews[VICTIM_SELECT_SCREEN]);

                // create the victim cards
                m_VictimCardList.Clear();
                m_VictimCardList.CreateCards(newArea.GetVictims());

                Player.CanMove = false;
            }
        }

        private void Callback_AreaVictimChanged(EListOperation op, AVictim oldVictim, AVictim newVictim)
        {
            if (m_firstAidDoc != null && m_firstAidDoc.CurrentArea != null)
            {
                m_VictimCardList.Clear();
                m_VictimCardList.CreateCards(m_firstAidDoc.CurrentArea.GetVictims());
            }
        }

        public void VictimCard_Clicked(GUIVictimCard card)
        {
            if (m_firstAidDoc == null)
                return;

            m_firstAidDoc.SetVictim(card.Victim);

            m_ImgVictim.sprite = card.Victim.m_PortraitSprite;
            m_ImgVictim.enabled = true;

            OpenView(m_CachedViews[TREATMENT_SCREEN]);
        }

        public void DeselectVictim()
        {
            if (m_firstAidDoc == null)
                return;

            m_firstAidDoc.SetVictim(null);

            m_ImgVictim.enabled = false;
            OpenView(m_CachedViews[VICTIM_SELECT_SCREEN]);
        }

        public void RequestRefillStock()
        {
            Debug.Log("Create the request refill stock function you dipshit!");
        }

        public void SendVictimToEvac()
        {
            if (m_firstAidDoc == null)
                return;

            m_firstAidDoc.SendVictimToEvac();

            // NOTE: view changes back to victim select screen after response is received
        }

        public void SendVictimToMorgue()
        {
            if (m_firstAidDoc == null)
                return;

            m_firstAidDoc.SendVictimToMorgue();

            // NOTE: view changes back to victim select screen after response is received
        }

        public void SelectItem(int itemIndex)
        {
            if (m_firstAidDoc == null || itemIndex < 0)
                return;

            m_selItemIdx = itemIndex;
        }

        public void UseItem()
        {
            if (m_firstAidDoc.CurrentFirstAidPoint == null || m_selItemIdx == -1)
                return;

            // check if item has sufficient quantities!!!!
            AFirstAidPoint fap = m_firstAidDoc.CurrentFirstAidPoint;

            ItemAttrib item = fap.ItemAttribs[m_selItemIdx];
            int quantity = fap.m_ItemQuantities[m_selItemIdx];

            if (quantity > 0 || item.m_IsInfinite)
            {
                // go to apply treatment screen!
                m_ImgBackground.enabled = false;
                m_ImgVictim.enabled = false;

                if (Player != null)
                {
                    Player.Blackout.gameObject.SetActive(true);
                    Player.LarryModel.gameObject.SetActive(true);
                }

                OpenView(m_CachedViews[APPLY_TREATMENT_SCREEN]);
            }
            else
            {
                ShowTextBoxPrompt("Not enough " + item.m_PrintName.ToUpper() + ".");
            }
        }

        public void ApplyTreatment()
        {
            if (m_selItemIdx == -1)
                return;

            m_firstAidDoc.UseItemOnVictim(m_selItemIdx);

            // NOTE: view changes back to treatment screen after response is received
        }

        public void CancelApplyTreatment()
        {
            m_selItemIdx = -1;

            m_ImgVictim.enabled = true;
            m_ImgBackground.enabled = true;

            if (Player != null)
            {
                Player.LarryModel.SetActive(false);
                Player.Blackout.gameObject.SetActive(false);
            }

            OpenView(m_CachedViews[TREATMENT_SCREEN]);
        }

        private void Callback_ResponseReceived(ActorNetResponse response)
        {
            if (response.m_ResponseType.Equals("send_morgue_success") || response.m_ResponseType.Equals("success"))
            {
                Debug.Log(response.m_Message);

                ShowTextBoxPrompt(response.m_Message, () => {

                    m_ImgVictim.enabled = false;

                    OpenView(m_CachedViews[VICTIM_SELECT_SCREEN]);
                });
            }
            else if (response.m_ResponseType.Equals("doc_use_item"))
            {
                ShowTextBoxPrompt(response.m_Message, () => {

                    CancelApplyTreatment();

                });
            }
        }

        protected override void OnDestroy()
        {
            m_firstAidDoc.onAreaEnterExitFunc.RemoveListener(Callback_AreaEnterExit);
            m_firstAidDoc.onAreaVictimNumChangedFunc.RemoveListener(Callback_AreaVictimChanged);
            m_firstAidDoc.onResponseRecvFunc.RemoveListener(Callback_ResponseReceived);

            m_firstAidDoc = null;
        }
    }
}
