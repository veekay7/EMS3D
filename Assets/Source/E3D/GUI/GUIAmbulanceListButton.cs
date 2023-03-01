using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace E3D
{
    public class GUIAmbulanceListButton : GUIBase
    {
        [HideInInspector]
        public Toggle m_Toggle;

        public GameObject m_Owner;
        public TMP_Text m_TxtName;
        public TMP_Text m_TxtLocation;
        public TMP_Text m_TxtCapacity;
        public TMP_Text m_TxtTimeRemaining;
        public Image m_ImgMoveState;

        public Sprite m_MovingSprite;
        public Sprite m_StoppedSprite;
        public Sprite m_ReturningSprite;


        public AAmbulance Ambulance { get; set; }

        protected override void Awake()
        {
            base.Awake();

            m_Toggle = GetComponent<Toggle>();
        }

        private void Start()
        {
            m_Toggle.onValueChanged.AddListener(Toggle_ValueChanged);
        }

        private void LateUpdate()
        {
            if (Ambulance != null)
            {
                m_Toggle.interactable = !Ambulance.InUse;

                if (Ambulance.CurrentRoute != null)
                {
                    if (Ambulance.CurrentState == AAmbulance.EState.Moving)
                    {
                        switch (Ambulance.MovingDirection)
                        {
                            case AAmbulance.EDirection.ToDest:
                                m_TxtLocation.text = Ambulance.CurrentRoute.m_Location.m_PrintName;
                                break;

                            case AAmbulance.EDirection.ToEvacPoint:
                                m_TxtLocation.text = "Evac Point";
                                break;

                            default:
                                m_TxtLocation.text = "Unknown";
                                break;
                        }
                    }
                }
                else
                {
                    m_TxtLocation.text = "Not set";
                }

                m_TxtCapacity.text = Ambulance.Victim != null ? "1 / 1" : "0 / 1";

                if (Ambulance.CurrentRoute != null)
                {
                    float timeRemain = (Ambulance.CurrentRoute.m_TravelTime - Ambulance.Progress);
                    timeRemain = (float)Math.Round(timeRemain, 2);
                    m_TxtTimeRemaining.text = timeRemain.ToString() + " min";
                }
                else
                {
                    m_TxtTimeRemaining.text = "0 mins";
                }

                if (Ambulance.CurrentState == AAmbulance.EState.Idle)
                {
                    m_ImgMoveState.sprite = m_StoppedSprite;
                }
                else
                {
                    if (Ambulance.MovingDirection == AAmbulance.EDirection.ToDest)
                        m_ImgMoveState.sprite = m_MovingSprite;
                    else if (Ambulance.MovingDirection == AAmbulance.EDirection.ToEvacPoint)
                        m_ImgMoveState.sprite = m_ReturningSprite;
                    else
                        m_ImgMoveState.sprite = m_StoppedSprite;
                }
            }
        }

        private void Toggle_ValueChanged(bool value)
        {
            if (m_Owner != null)
            {
                ExecuteEvents.Execute<IClickedHandler>(m_Owner, null,
                    (handler, e) => { handler.AmbulanceButton_Clicked(value, this); });
            }
        }

        private void OnDestroy()
        {
            m_Toggle.onValueChanged.RemoveListener(Toggle_ValueChanged);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            m_Toggle = gameObject.GetOrAddComponent<Toggle>();
        }


        public interface IClickedHandler : IEventSystemHandler
        {
            void AmbulanceButton_Clicked(bool state, GUIAmbulanceListButton button);
        }
    }
}
