using UnityEngine;
using UnityEngine.UI;

namespace E3D
{
    public class HudTriageOffr : Hud
    {
        public const int VICTIM_SELECT_SCREEN = 0;
        public const int TRIAGE_SCREEN = 1;

        [Header("Triage Officer HUD")]
        public Image m_ImgBackground;
        public Image m_ImgVictim;
        public GUIVictimCardList m_VictimCardList;
        public GUIKrankenakte m_Karte;
        public Button m_BtnFinishTriage;
        public LayerMask m_RaycastIgnoreLayers;

        private AEmtTriageOffr m_triageOffr;


        public AEmtTriageOffr PossessedEmt { get => m_triageOffr; }

        protected override void Start()
        {
            base.Start();

            m_VictimCardList.onCardClickedFunc.AddListener(VictimCard_Clicked);
        }

        public override void SetPlayer(E3DPlayer newPlayer)
        {
            base.SetPlayer(newPlayer);
            
            if (m_triageOffr != null)
            {
                m_triageOffr.onAreaEnterExitFunc.RemoveListener(Callback_AreaEnterExit);
                m_triageOffr.onAreaVictimNumChangedFunc.RemoveListener(Callback_AreaVictimChanged);
                m_triageOffr.onResponseRecvFunc.RemoveListener(Callback_ResponseReceived);

                m_triageOffr = null;
            }

            if (Player != null)
            {
                if (Player.PossessedEmt != null)
                {
                    m_triageOffr = (AEmtTriageOffr)Player.PossessedEmt;

                    m_triageOffr.onAreaEnterExitFunc.AddListener(Callback_AreaEnterExit);
                    m_triageOffr.onAreaVictimNumChangedFunc.AddListener(Callback_AreaVictimChanged);
                    m_triageOffr.onResponseRecvFunc.AddListener(Callback_ResponseReceived);
                }
            }
        }

        private void LateUpdate()
        {
            if (Player != null && m_triageOffr != null)
            {
                // do a raycast and try to move player. totally depends on the type of controlling officer
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Player.CurrentCamera.m_CameraComponent.ScreenPointToRay(Input.mousePosition);

                    // if mouse if over the a GUI object or there is a prompt waiting, don't bother raycasting and do any shit
                    if (Utils.Input_IsPointerOnGUI() || Player.IsWaitingForPrompt)
                        return;

                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, ~m_RaycastIgnoreLayers))
                    {
                        // query the hit object to get areas instead
                        GameObject hitObject = hit.transform.gameObject;

                        AVictimPlaceableArea casualtyPoint = hitObject.GetComponent<AVictimPlaceableArea>();

                        if (casualtyPoint != null && !(casualtyPoint is AFirstAidPoint) && !(casualtyPoint is AEvacPoint))
                            m_triageOffr.EnterArea(casualtyPoint);
                    }
                    else
                    {
                        m_triageOffr.EnterArea(null);
                    }
                }

                // update finish triage button interactable state
                m_BtnFinishTriage.interactable = m_triageOffr.CurrentVictim?.m_State.m_GivenPACS != EPACS.None;
            }
        }

        private void Callback_AreaEnterExit(AVictimPlaceableArea oldArea, AVictimPlaceableArea newArea)
        {
            if (oldArea != null && oldArea == m_triageOffr.CurrentArea)
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
            if (m_triageOffr != null && m_triageOffr.CurrentArea != null)
            {
                m_VictimCardList.Clear();
                m_VictimCardList.CreateCards(m_triageOffr.CurrentArea.GetVictims());
            }
        }

        public void ExitCasualtyPoint()
        {
            m_triageOffr.EnterArea(null);

            if (m_curView == m_CachedViews[VICTIM_SELECT_SCREEN])
                CloseCurrentView();

            m_ImgBackground.enabled = false;
            m_ImgVictim.enabled = false;

            Player.CanMove = true;
        }

        public void VictimCard_Clicked(GUIVictimCard card)
        {
            if (m_triageOffr == null)
                return;

            m_triageOffr.SetVictim(card.Victim);

            m_ImgVictim.sprite = card.Victim.m_PortraitSprite;
            m_ImgVictim.enabled = true;

            m_Karte.SetVictim(card.Victim);

            OpenView(m_CachedViews[TRIAGE_SCREEN]);
        }

        [EnumAction(typeof(ECheckVitalAct))]
        public void CheckVictimVitals(int act)
        {
            if (m_triageOffr == null)
                return;

            m_triageOffr.CheckVictimVital((ECheckVitalAct)act);
        }

        [EnumAction(typeof(EPACS))]
        public void SetPACTag(int tag)
        {
            if (m_triageOffr == null)
                return;

            m_triageOffr.SetPACSTag((EPACS)tag);
        }

        public void FinishTriage()
        {
            if (m_triageOffr == null)
                return;

            m_triageOffr.FinishTriage();

            // NOTE: view changes back to victim select screen after response is received
        }

        public void CancelTriage()
        {
            if (m_triageOffr == null)
                return;

            m_triageOffr.StopTriage();

            // NOTE: view changes back to victim select screen after response is received
        }

        private void Callback_ResponseReceived(ActorNetResponse response)
        {
            if (response.m_ResponseType.Equals("triage_complete"))
            {
                ShowTextBoxPrompt(response.m_Message, () => {

                    m_ImgVictim.enabled = false;
                    m_Karte.SetVictim(null);

                    OpenView(m_CachedViews[VICTIM_SELECT_SCREEN]);
                });
            }
            else if (response.m_ResponseType.Equals("triage_cancel"))
            {
                m_ImgVictim.enabled = false;
                m_Karte.SetVictim(null);

                OpenView(m_CachedViews[VICTIM_SELECT_SCREEN]);
            }
            else if (response.m_ResponseType.Equals("check_vital"))
            {
                string outputString = "The victim";
                switch ((ECheckVitalAct)response.m_Data)
                {
                    case ECheckVitalAct.CanWalk:
                        outputString += m_triageOffr.CurrentVictim.m_CanWalk ? " is AMBULANT." : " is NOT AMBULANT.";
                        break;

                    case ECheckVitalAct.HeartRate:
                        outputString += "'s HEART RATE is " + m_triageOffr.CurrentVictim.m_HeartRate.ToString() + ".";
                        break;

                    case ECheckVitalAct.Respiration:
                        outputString += "'s RESPIRATION RATE is " + m_triageOffr.CurrentVictim.m_Respiration.ToString() + ".";
                        break;

                    case ECheckVitalAct.BloodPressure:
                        outputString += "'s BLOOD PRESSURE is " + m_triageOffr.CurrentVictim.m_BloodPressure.ToString() + ".";
                        break;

                    case ECheckVitalAct.SpO2:
                        outputString += "'s BLOOD OXYGEN LEVEL is " + m_triageOffr.CurrentVictim.m_SpO2.ToString() + ".";
                        break;

                    case ECheckVitalAct.GCS:
                        outputString += "'s GLASGOW COMA SCALE is " + m_triageOffr.CurrentVictim.m_GCS.ToString() + ".";
                        break;
                }

                ShowTextBoxPrompt(outputString);
            }
            else if (response.m_ResponseType.Equals("set_pacs"))
            {
                EPACS pacs = (EPACS)response.m_Data;

                if (pacs == EPACS.None)
                    return;

                string outputString = "You tagged " + pacs.ToString() + " on " + m_triageOffr.CurrentVictim.m_GivenName + " .";
                ShowTextBoxPrompt(outputString);
            }
            else
            {
                ShowTextBoxPrompt(response.m_Message);
            }
        }

        protected override void OnDestroy()
        {
            if (m_triageOffr != null)
            {
                m_triageOffr.onAreaEnterExitFunc.RemoveListener(Callback_AreaEnterExit);
                m_triageOffr.onAreaVictimNumChangedFunc.RemoveListener(Callback_AreaVictimChanged);
                m_triageOffr.onResponseRecvFunc.RemoveListener(Callback_ResponseReceived);

                m_triageOffr = null;
            }
        }
    }
}
