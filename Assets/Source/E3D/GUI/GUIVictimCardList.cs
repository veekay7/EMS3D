using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace E3D
{
    public class GUIVictimCardList : GUIBase, GUIVictimCard.IClickedHandler
    {
        public GameObject m_VictimCardPrefab;
        public RectTransform m_SortBtnsTransform;
        public RectTransform m_ContentTransform;
        public bool m_ShowSortControls;

        private List<GUIVictimCard> m_cards = new List<GUIVictimCard>();

        public CardClickedEvent onCardClickedFunc;


        public void Clear()
        {
            for (int i = 0; i < m_cards.Count; i++)
            {
                Destroy(m_cards[i].gameObject);
            }
            m_cards.Clear();
        }

        public void CreateCard(AVictim victim)
        {
            GUIVictimCard cardPrefab = m_VictimCardPrefab.GetComponent<GUIVictimCard>();

            var newCard = Instantiate(cardPrefab);

            newCard.gameObject.SetActive(true);
            newCard.CardList = this;
            newCard.SetVictim(victim);
            newCard.m_RectTransform.SetParent(m_ContentTransform, false);
            
            m_cards.Add(newCard);
        }

        public void CreateCards(AVictim[] victims)
        {
            for (int i = 0; i < victims.Length; i++)
            {
                CreateCard(victims[i]);
            }
        }

        [EnumAction(typeof(EPACS))]
        public void ShowCardsWithPACScale(int pacs)
        {
            for (int i = 0; i < m_cards.Count; i++)
            {
                if (pacs == (int)EPACS.None)
                {
                    m_cards[i].gameObject.SetActive(true);
                    continue;
                }

                if (m_cards[i].Victim.m_PACS == (EPACS)pacs)
                    m_cards[i].gameObject.SetActive(true);
                else
                    m_cards[i].gameObject.SetActive(false);
            }
        }

        public void ShowSortingOptions(bool show)
        {
            m_SortBtnsTransform.gameObject.SetActive(show);
            m_ShowSortControls = show;
        }

        public void VictimCard_Clicked(GUIVictimCard card)
        {
            if (onCardClickedFunc != null)
                onCardClickedFunc.Invoke(card);
        }

        private void OnDestroy()
        {
            Clear();
        }

        protected override void OnValidate()
        {
            if (m_VictimCardPrefab != null)
            {
                GUIVictimCard component = m_VictimCardPrefab.GetComponent<GUIVictimCard>();
                if (component == null)
                {
                    Debug.LogError("E3D_GUIVictimList::m_VictimCardPrefab does not contain a E3D_GUIVictimCard component.");
                    m_VictimCardPrefab = null;
                }
            }

            if (m_SortBtnsTransform != null)
            {
                m_SortBtnsTransform.gameObject.SetActive(m_ShowSortControls);
            }

            base.OnValidate();
        }


        /// <summary>
        /// Card clicked event class
        /// </summary>
        [Serializable]
        public class CardClickedEvent : UnityEvent<GUIVictimCard> { }
    }
}
