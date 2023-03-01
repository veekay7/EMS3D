using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer), typeof(CameraFacingBillboard), typeof(CameraRelativeScale))]
    public class AVictimPlaceableArea : ALocationPoint
    {
        [HideInInspector]
        public SpriteRenderer m_Renderer;
        [HideInInspector]
        public CameraFacingBillboard m_BillboardComponent;
        [HideInInspector]
        public CameraRelativeScale m_CameraRelativeScale;

        public Sprite m_BackgroundSprite;
        public bool m_SetVisible;

        [SerializeField, ReadOnlyVar]
        private List<E3DPlayer> m_players = new List<E3DPlayer>();
        [SerializeField, ReadOnlyVar]
        private List<AVictim> m_victimList = new List<AVictim>();

        public event ListChangedFuncDelegate<AVictim> victimListChangedFunc;

        public bool IsVisible { get => m_Renderer.enabled; }

        public int NumPlayers { get => m_players.Count; }

        public int NumVictims { get => m_victimList.Count; }

        public bool MultiplePlayersAllowed { get; protected set; }


        protected override void Awake()
        {
            base.Awake();

            m_Renderer = GetComponent<SpriteRenderer>();
            m_BillboardComponent = GetComponent<CameraFacingBillboard>();
            m_CameraRelativeScale = GetComponent<CameraRelativeScale>();

            MultiplePlayersAllowed = false;
        }

        protected override void Reset()
        {
            base.Reset();
            m_SetVisible = true;
        }

        protected override void Start()
        {
            SetVisible(true);

            transform.localScale = Vector3.one;
            m_BillboardComponent.enabled = true;
            m_CameraRelativeScale.enabled = true;

            base.Start();
        }

        public void AddPlayer(E3DPlayer newPlayer)
        {
            if (MultiplePlayersAllowed)
            {
                goto add_player;
            }
            else
            {
                if (NumPlayers == 0)
                    goto add_player;
            }

add_player:
            if (!m_players.Contains(newPlayer))
            {
                m_players.Add(newPlayer);
                return;
            }
        }

        public void RemovePlayer(E3DPlayer player)
        {
            if (m_players.Contains(player))
                m_players.Remove(player);
        }

        public E3DPlayer[] GetPlayers()
        {
            return m_players.ToArray();
        }

        public void AddVictim(AVictim newVictim)
        {
            if (!m_victimList.Contains(newVictim))
            {
                newVictim.m_StartHealth = newVictim.m_CurHealth;

                m_victimList.Add(newVictim);

                if (victimListChangedFunc != null)
                    victimListChangedFunc.Invoke(EListOperation.Add, null, newVictim);
            }
        }

        public void RemoveVictim(AVictim victim)
        {
            if (m_victimList.Contains(victim))
            {
                m_victimList.Remove(victim);

                if (victimListChangedFunc != null)
                    victimListChangedFunc.Invoke(EListOperation.Remove, victim, null);
            }
        }

        public AVictim[] GetVictims()
        {
            return m_victimList.ToArray();
        }

        public void SetVisible(bool visible)
        {
            m_Renderer.enabled = visible;
        }

        protected override void OnDestroy()
        {
            SetVisible(true);

            m_BillboardComponent.enabled = false;
            m_CameraRelativeScale.enabled = false;
            transform.localScale = Vector3.one;

            m_victimList.Clear();

            base.OnDestroy();
        }

        // editor only
        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_Renderer == null)
            {
                m_Renderer = GetComponent<SpriteRenderer>();
                if (m_Renderer == null)
                    m_Renderer = gameObject.AddComponent<SpriteRenderer>();
            }

            if (m_BillboardComponent == null)
            {
                m_BillboardComponent = GetComponent<CameraFacingBillboard>();
                if (m_BillboardComponent == null)
                    m_BillboardComponent = gameObject.AddComponent<CameraFacingBillboard>();
            }

            if (m_CameraRelativeScale == null)
            {
                m_CameraRelativeScale = GetComponent<CameraRelativeScale>();
                if (m_CameraRelativeScale == null)
                    m_CameraRelativeScale = gameObject.AddComponent<CameraRelativeScale>();
            }

            SetVisible(m_SetVisible);
        }
    }
}
