using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace E3D
{
    public enum ESelectVictimWindowMode
    {
        None, LoadVictims, UnloadVictims
    }

    public class GUISelectVictimWindow : GUIBase
    {
        public TMP_Text m_TxtWindowTitle;
        public Button m_VictimBtnPrefab;
        public RectTransform m_VictimBtnListContentTransform;
        public GUIKrankenakte m_Karte;

        [Header("P3 Button Colors")]
        public ColorBlock m_P3ButtonColors;
        [Header("P2 Button Colors")]
        public ColorBlock m_P2ButtonColors;
        [Header("P1 Button Colors")]
        public ColorBlock m_P1ButtonColors;
        [Header("P0 Button Colors")]
        public ColorBlock m_P0ButtonColors;

        private ESelectVictimWindowMode m_mode;
        private AVictim m_selVictim;
        private AVictim[] m_victims;
        private List<Button> m_victimBtns = new List<Button>();

        public event UnityAction<AVictim> onOkBtnClickedFunc;
        public event UnityAction onCancelBtnClickedFunc;


        public ESelectVictimWindowMode CurrentMode { get => m_mode; }

        public AVictim SelectedVictim { get => m_selVictim; }

        protected override void Awake()
        {
            base.Awake();

            m_mode = ESelectVictimWindowMode.LoadVictims;
            m_selVictim = null;
            m_victims = null;
        }

        public void SetMode(ESelectVictimWindowMode mode)
        {
            m_mode = mode;
            if (m_mode == ESelectVictimWindowMode.LoadVictims)
                m_TxtWindowTitle.text = "Load Victims";
            else if (m_mode == ESelectVictimWindowMode.UnloadVictims)
                m_TxtWindowTitle.text = "Unload Victims";
            else
                m_TxtWindowTitle.text = "Select Victims";
        }

        public void ClearVictimButtons()
        {
            m_Karte.SetVictim(null);

            m_mode = ESelectVictimWindowMode.None;
            m_selVictim = null;
            m_victims = null;

            ClearButtonList();
        }

        public void ClearButtonList()
        {
            for (int i = 0; i < m_victimBtns.Count; i++)
            {
                Destroy(m_victimBtns[i].gameObject);
            }
            m_victimBtns.Clear();
        }

        public void PopulateVictimButtons(AVictim[] victims)
        {
            m_victims = victims;

            for (int i = 0; i < victims.Length; i++)
            {
                var newItemBtn = Instantiate(m_VictimBtnPrefab);

                newItemBtn.gameObject.SetActive(true);

                var pac = m_victims[i].m_State.m_GivenPACS;

                if (pac == EPACS.P3)
                    newItemBtn.GetComponent<Button>().colors = m_P3ButtonColors;
                else if (pac == EPACS.P2)
                    newItemBtn.GetComponent<Button>().colors = m_P2ButtonColors;
                else if (pac == EPACS.P1)
                    newItemBtn.GetComponent<Button>().colors = m_P1ButtonColors;
                else if (pac == EPACS.P0)
                    newItemBtn.GetComponent<Button>().colors = m_P0ButtonColors;

                newItemBtn.GetComponent<RectTransform>().GetChild(0).GetComponent<TMP_Text>().text = victims[i].m_GivenName;
                newItemBtn.GetComponent<RectTransform>().SetParent(m_VictimBtnListContentTransform, false);

                m_victimBtns.Add(newItemBtn);
            }

            // if there are more than 1 button, we select the first one
            if (m_victimBtns.Count > 0)
            {
                m_victimBtns[0].onClick.Invoke();
            }
        }

        public void Callback_OnItemClicked(GameObject buttonObject)
        {
            int idx = m_victimBtns.IndexOf(buttonObject.GetComponent<Button>());

            if (idx != -1)
            {
                if (m_victims != null)
                {
                    m_selVictim = m_victims[idx];
                    m_Karte.SetVictim(m_selVictim);
                }
            }
        }

        public void OkBtn_Clicked()
        {
            if (m_selVictim != null)
            {
                if (onOkBtnClickedFunc != null)
                    onOkBtnClickedFunc.Invoke(m_selVictim);

                ClearVictimButtons();

                gameObject.SetActive(false);
            }
        }

        public void CancelBtn_Clicked()
        {
            if (onCancelBtnClickedFunc != null)
                onCancelBtnClickedFunc.Invoke();

            ClearVictimButtons();

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            ClearVictimButtons();
        }
    }
}
