using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace E3D
{
    public class GUIVictimCard : GUIBase, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector]
        public Button m_ButtonComponent;

        public TMP_Text m_TxtGivenName;
        public TMP_Text m_TxtPAC;
        public Image m_ImgPAC;
        public Image m_ImgPortrait;
        public Image m_ImgSex;
        public TMP_Text m_TxtAgeGroup;
        public Image m_ImgIsPregnant;

        public readonly Color m_Colour_P3;
        public readonly Color m_Colour_P2;
        public readonly Color m_Colour_P1;
        public readonly Color m_Colour_P0;
        public readonly Color m_Colour_Generic;

        public Sprite m_MaleGenderIconSprite;
        public Sprite m_FemaleGenderIconSprite;
        public Sprite m_NullMalePortraitSprite;
        public Sprite m_NullFemalePortraitSprite;

        public Sprite m_PregnantSprite;
        public Sprite m_NotPregnantSprite;


        public GUIVictimCardList CardList { get; set; }

        public AVictim Victim { get; private set; }

        private void Start()
        {
            m_ButtonComponent.onClick.AddListener(Button_Clicked);
        }

        private void LateUpdate()
        {
            if (Victim == null)
                return;

            if (m_TxtPAC != null && m_ImgPAC != null)
            {
                EPACS pac = Victim.m_State.m_GivenPACS;

                m_TxtPAC.text = pac.ToString();

                switch (pac)
                {
                    case EPACS.None:
                        m_ImgPAC.color = Consts.COLOR_PAC_OK2;
                        break;

                    case EPACS.P3:
                        m_ImgPAC.color = Consts.COLOR_P3;
                        break;

                    case EPACS.P2:
                        m_ImgPAC.color = Consts.COLOR_P2;
                        break;

                    case EPACS.P1:
                        m_ImgPAC.color = Consts.COLOR_P1;
                        break;

                    case EPACS.P0:
                        m_ImgPAC.color = Consts.COLOR_P0;
                        break;
                }
            }
        }

        public void SetVictim(AVictim newVictim)
        {
            Victim = newVictim;
            
            Clear();
            UpdateProfileDisplay();
        }

        public void Clear()
        {
            if (m_TxtGivenName != null && m_TxtPAC != null && m_ImgPAC != null && m_ImgPortrait != null &&
                m_ImgSex != null && m_TxtAgeGroup != null && m_ImgIsPregnant != null)
            {
                m_TxtGivenName.text = "No data";
                m_TxtPAC.text = "No data";
                m_ImgPAC.color = m_Colour_Generic;
                m_ImgPortrait.sprite = m_NullMalePortraitSprite;
                m_ImgSex.sprite = m_MaleGenderIconSprite;
                m_TxtAgeGroup.text = "?";
                m_ImgIsPregnant.sprite = m_NotPregnantSprite;
            }
        }

        private void UpdateProfileDisplay()
        {
            if (Victim == null)
                return;

                if (m_TxtGivenName != null && m_ImgSex != null && m_TxtAgeGroup != null && m_ImgIsPregnant != null)
            {
                m_TxtGivenName.text = Victim.m_GivenName;
                m_ImgSex.sprite = Victim.m_Sex == ESex.Male ? m_MaleGenderIconSprite : m_FemaleGenderIconSprite;

                if (Victim.m_PortraitSprite != null)
                {
                    m_ImgPortrait.sprite = Victim.m_PortraitSprite;
                }
                else
                {
                    if (Victim.m_Sex == ESex.Male)
                        m_ImgPortrait.sprite = m_NullMalePortraitSprite;
                    else
                        m_ImgPortrait.sprite = m_NullFemalePortraitSprite;
                }

                m_TxtAgeGroup.text = Victim.m_AgeLo.ToString() + " ~ " + Victim.m_AgeHi.ToString();
                m_ImgIsPregnant.sprite = Victim.m_IsPregnant ? m_PregnantSprite : m_NotPregnantSprite; ;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // TODO: show glow
            if (m_ButtonComponent.interactable)
            {
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // TODO: fade glow out
            if (m_ButtonComponent.interactable)
            {
            }
        }

        private void Button_Clicked()
        {
            if (m_ButtonComponent.interactable)
            {
                if (CardList != null)
                {
                    ExecuteEvents.Execute<IClickedHandler>(CardList.gameObject, null,
                        (handler, e) => { handler.VictimCard_Clicked(this); });
                }
            }
        }

        private void OnDestroy()
        {
            m_ButtonComponent.onClick.RemoveListener(Button_Clicked);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            m_ButtonComponent = gameObject.GetOrAddComponent<Button>();
        }


        public interface IClickedHandler : IEventSystemHandler
        {
            void VictimCard_Clicked(GUIVictimCard card);
        }
    }
}
