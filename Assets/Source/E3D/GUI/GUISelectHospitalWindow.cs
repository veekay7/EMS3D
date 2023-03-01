using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace E3D
{
    public class GUISelectHospitalWindow : GUIBase
    {
        public Image m_ImgLogo;
        public TMP_Text m_TxtInfo;
        public TMP_Text m_TxtSpecialties;
        public Button m_ButtonPrefab;
        public RectTransform m_BtnsContentTransform;
        public Button m_BtnOk;
        public Button m_BtnCancel;

        public Sprite m_NullLogoSprite;
        public HudEvacOffr m_Hud;

        private AHospital m_hospital;
        private GameState m_gameState;
        private List<Button> m_buttons = new List<Button>();

        public event UnityAction<Route> onOkBtnClickedFunc;
        public event UnityAction onCancelBtnClickedFunc;

        public Route HospitalRoute { get; private set; }


        protected override void Awake()
        {
            base.Awake();

            m_gameState = null;
        }

        private void OnEnable()
        {
            m_BtnOk.interactable = false;

            m_gameState = GameState.Current;
            if (m_gameState != null)
            {
                var hospitals = m_gameState.m_Hospitals.ToArray();
                for (int i = 0; i < hospitals.Length; i++)
                {
                    var newBtn = Instantiate(m_ButtonPrefab);

                    newBtn.gameObject.SetActive(true);
                    newBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = hospitals[i].m_PrintName;
                    newBtn.transform.SetParent(m_BtnsContentTransform, false);

                    m_buttons.Add(newBtn);
                }

                // if there are buttons, select the first one as the option
                if (m_buttons.Count > 0)
                {
                    m_buttons[0].onClick.Invoke();
                }
            }
        }

        public void ClearInfo()
        {
            m_ImgLogo.sprite = m_NullLogoSprite;

            string capacityString = "Unknown";
            string distString = "Unknown";
            string etaString = "Unknown";
            m_TxtInfo.text = capacityString + "\n\n" +
                distString + "\n\n" +
                etaString;

            m_TxtSpecialties.text = "No specialties available.";
        }

        public void Clear()
        {
            ClearInfo();

            for (int i = 0; i < m_buttons.Count; i++)
            {
                Destroy(m_buttons[i].gameObject);
            }
            m_buttons.Clear();
        }

        // called by GUI buttons set in editor
        public void Callback_ButtonClicked(GameObject button)
        {
            ClearInfo();

            m_hospital = null;
            m_BtnOk.interactable = false;

            // find the index of the button and if there is a match, we update the display
            var buttonComponent = button.GetComponent<Button>();
            int buttonIndex = m_buttons.IndexOf(buttonComponent);
            
            if (buttonIndex != -1)
            {
                var hospitals = m_gameState.m_Hospitals.ToArray();
                m_hospital = hospitals[buttonIndex];
                
                m_ImgLogo.sprite = m_hospital.m_Logo != null ? m_hospital.m_Logo : null;
                
                m_TxtSpecialties.text = m_hospital.m_Specialties.ToString();

                GameState gameState = GameState.Current;
                if (gameState != null)
                {
                    var route = gameState.m_RouteController.GetRoute(m_hospital.m_LocationId);
                    HospitalRoute = route;
                }

                RefreshInfo();

                m_BtnOk.interactable = true;
            }
        }

        private void RefreshInfo()
        {
            if (HospitalRoute == null || m_hospital == null)
                return;

            string capacityString = m_hospital.NumVictims.ToString() + " / " + m_hospital.m_MaxCapacity.ToString();
            string distString = HospitalRoute.m_Distance.ToString();
            string timeString = HospitalRoute.m_TravelTime.ToString();
            
            m_TxtInfo.text = capacityString + "\n\n" + distString + "\n\n" + timeString;
        }

        public void Callback_OkBtnClicked()
        {
            if (onOkBtnClickedFunc != null)
                onOkBtnClickedFunc.Invoke(HospitalRoute);

            Clear();
            gameObject.SetActive(false);
        }

        public void Callback_CancelBtnClicked()
        {
            if (onCancelBtnClickedFunc != null)
                onCancelBtnClickedFunc.Invoke();
            
            Clear();
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            HospitalRoute = null;
            m_hospital = null;
        }

        protected void OnDestroy()
        {
            HospitalRoute = null;
            m_hospital = null;

            Clear();
        }
    }
}
