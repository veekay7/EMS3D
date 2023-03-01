using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

namespace E3D
{
    public class GUICreditsScreen : GUIScreen
    {
        [Header("Credits Screen")]
        public Button m_BtnPrev;
        public Button m_BtnNext;

        [ReorderableList]
        public List<CanvasGroup> m_Pages = new List<CanvasGroup>();

        private int m_curPageIdx;


        protected override void Awake()
        {
            base.Awake();
            m_curPageIdx = -1;
        }

        protected override void Start()
        {
            base.Start();

            if (m_Pages.Count == 0)
            {
                m_BtnPrev.interactable = false;
                m_BtnNext.interactable = false;
            }
        }

        public override void Open(UnityAction onFinishAnim = null)
        {
            base.Open(onFinishAnim);

            if (m_Pages.Count > 1)
            {
                m_BtnPrev.interactable = true;
                m_BtnNext.interactable = true;

                for (int i = 0; i < m_Pages.Count; i++)
                {
                    m_Pages[i].alpha = 0.0f;
                    m_Pages[i].interactable = false;
                    m_Pages[i].blocksRaycasts = false;
                }
            }

            m_curPageIdx = 0;

            m_Pages[m_curPageIdx].alpha = 1.0f;
            m_Pages[m_curPageIdx].interactable = true;
            m_Pages[m_curPageIdx].blocksRaycasts = true;
        }

        public void NextPage()
        {
            if (m_Pages.Count == 1)
                return;

            m_BtnPrev.interactable = false;
            m_BtnNext.interactable = false;

            int lastIdx = m_curPageIdx;

            m_Pages[lastIdx].interactable = false;
            m_Pages[lastIdx].blocksRaycasts = false;

            m_curPageIdx++;
            m_curPageIdx = (int)Mathf.Repeat(m_curPageIdx, m_Pages.Count);

            var closeTween = DOTween.To(() => m_Pages[lastIdx].alpha, (a) => m_Pages[lastIdx].alpha = a, 0.0f, 0.1f);
            
            closeTween.OnComplete(() => 
            {
                var showTween = DOTween.To(() => m_Pages[m_curPageIdx].alpha, (a) => m_Pages[m_curPageIdx].alpha = a, 1.0f, 0.1f);
                showTween.OnComplete(() => 
                {
                    m_BtnPrev.interactable = true;
                    m_BtnNext.interactable = true;

                    m_Pages[m_curPageIdx].interactable = true;
                    m_Pages[m_curPageIdx].blocksRaycasts = true;
                });
            });
        }

        public void PreviousPage()
        {
            if (m_Pages.Count == 1)
                return;

            m_BtnPrev.interactable = false;
            m_BtnNext.interactable = false;

            int lastIdx = m_curPageIdx;

            m_Pages[lastIdx].interactable = false;
            m_Pages[lastIdx].blocksRaycasts = false;

            m_curPageIdx--;
            m_curPageIdx = (int)Mathf.Repeat(m_curPageIdx, m_Pages.Count);

            var closeTween = DOTween.To(() => m_Pages[lastIdx].alpha, (a) => m_Pages[lastIdx].alpha = a, 0.0f, 0.1f);
            
            closeTween.OnComplete(() => 
            {
                var showTween = DOTween.To(() => m_Pages[m_curPageIdx].alpha, (a) => m_Pages[m_curPageIdx].alpha = a, 1.0f, 0.1f);
                showTween.OnComplete(() => 
                {
                    m_BtnPrev.interactable = true;
                    m_BtnNext.interactable = true;

                    m_Pages[m_curPageIdx].interactable = true;
                    m_Pages[m_curPageIdx].blocksRaycasts = true;
                });
            });
        }
    }
}
