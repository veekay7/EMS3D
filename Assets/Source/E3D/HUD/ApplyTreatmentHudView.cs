using UnityEngine;
using UnityEngine.UI;

namespace E3D
{
    public class ApplyTreatmentHudView : HudBaseView
    {
        public Button m_BtnBack;
        public Image m_ImgCursor;

        private E3DPlayer m_player;
        private HudFirstAidDoc m_hud;
        private E3DLarryModel m_larryModel;
        private AEmtFirstAidDoc m_firstAidDoc;
        private AVictim m_curVictim;


        protected override void Awake()
        {
            base.Awake();

            m_ImgCursor.enabled = false;
        }

        public override void Open()
        {
            m_hud = (HudFirstAidDoc)Owner;
            m_player = m_hud.Player;
            m_larryModel = m_player.LarryModel;
            m_firstAidDoc = m_hud.PossessedEmt;
            m_curVictim = m_firstAidDoc.CurrentVictim;

            var item = m_firstAidDoc.CurrentFirstAidPoint.ItemAttribs[m_hud.SelectedItemIndex];
            m_ImgCursor.sprite = item.m_Sprite;
            m_ImgCursor.enabled = true;

            base.Open();
        }

        public override void Close()
        {
            m_player = null;
            m_hud = null;
            m_larryModel = null;
            m_firstAidDoc = null;
            m_curVictim = null;

            m_ImgCursor.enabled = false;

            base.Close();
        }

        private void Update()
        {
            if (m_player == null || m_larryModel == null || m_hud == null)
                return;

            if (m_player.m_EnableInput)
            {
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    m_larryModel.SetInputVector(new Vector2(0.0f, 3.0f));
                }
                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    m_larryModel.SetInputVector(new Vector2(0.0f, -3.0f));
                }
                else
                {
                    m_larryModel.SetInputVector(Vector2.zero);
                }
                
                if (Input.GetMouseButtonDown(0))
                {
                    if (!Utils.Input_IsPointerOnGUI())
                    {
                        if (m_larryModel.DoRaycast(Input.mousePosition, out BodyPartVolume hitBodyPart))
                        {
                            ApplyHealingItemOnLarry(hitBodyPart);
                        }
                        else
                        {
                            m_hud.ShowTextBoxPrompt("You cannot apply treatment here. Make sure your treatments are applied on the model.");
                        }
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    GoBack();
                }

                if (m_ImgCursor.enabled)
                {
                    m_ImgCursor.rectTransform.position = Input.mousePosition;
                }
            }
        }

        public void GoBack()
        {
            if (m_hud == null)
                return;

            m_hud.CancelApplyTreatment();
        }

        private void ApplyHealingItemOnLarry(BodyPartVolume hitBodyPart)
        {
            // get equipped item from index
            var itemAttribs = m_firstAidDoc.CurrentFirstAidPoint.ItemAttribs;
            var equippedItem = itemAttribs[m_hud.SelectedItemIndex];

            // can the item be applied on the body part?
            if (hitBodyPart.CanApplyItemOnBodyPart(equippedItem))
            {
                Injury victimInjury = InjuryList.FindByIndex(m_curVictim.InjuryIdx);
                EBodyPartId injuryBodyPartId = victimInjury.m_BodyPartId;

                // if yes, check to see if the injury occurs on any of the limbs and that the equipped item can be used on both upper limbs and lower limbs
                if ((injuryBodyPartId == EBodyPartId.UpperLimbs || injuryBodyPartId == EBodyPartId.LowerLimbs) &&
                    equippedItem.ContainsBodyPartId(EBodyPartId.UpperLimbs) && equippedItem.ContainsBodyPartId(EBodyPartId.LowerLimbs))
                {
                    // if the above conditions are true, check to see if this item can be applied to this injury.
                    // if not it means that the injury doesn't occur on that location.
                    if (!hitBodyPart.ContainsBodyPartId(injuryBodyPartId))
                    {
                        // TODO: say that this limb doesn't seem to have an injury
                        goto WrongTreatmentLocationApplied;
                    }
                }

                m_hud.ApplyTreatment();

                return;
            }
            else
            {
                goto WrongTreatmentLocationApplied;
            }


        WrongTreatmentLocationApplied:
            m_hud.ShowTextBoxPrompt("This does not seem to be an appropriate place for the treatment. Please try again.");
        }
    }
}
