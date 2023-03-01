using UnityEngine;
using UnityEngine.Events;

namespace E3D
{
    public class AEmtBase : MonoBehaviour
    {
        protected E3DPlayer m_player;
        protected AVictimPlaceableArea m_curArea;

        public AreaEnterExitEvent onAreaEnterExitFunc = new AreaEnterExitEvent();
        public AreaVictimNumChangedEvent onAreaVictimNumChangedFunc = new AreaVictimNumChangedEvent();
        public ActorNetResponseEvent onResponseRecvFunc = new ActorNetResponseEvent();


        public E3DPlayer Player { get => m_player; }

        public AVictimPlaceableArea CurrentArea { get => m_curArea; }

        protected virtual void Awake()
        {
            m_player = null;
            m_curArea = null;
        }

        protected virtual void Reset() { }
        
        protected virtual void Start()
        {
            if (GameState.Current != null)
                GameState.Current.AddEmtActor(this);
        }

        public virtual void Possess(E3DPlayer player)
        {
            m_player = player;
        }

        public virtual void UnPossess(E3DPlayer player)
        {
            if (m_player == player)
                m_player = null;
        }

        public virtual void EnterArea(AVictimPlaceableArea newArea)
        {
            var oldArea = m_curArea;
            
            if (oldArea != null)
            {
                oldArea.victimListChangedFunc -= OnAreaVictimChanged;
            }

            if (onAreaEnterExitFunc != null)
                onAreaEnterExitFunc.Invoke(m_curArea, newArea);

            m_curArea = newArea;

            if (m_curArea != null)
                m_curArea.victimListChangedFunc += OnAreaVictimChanged;
        }

        protected virtual void OnAreaVictimChanged(EListOperation op, AVictim oldItem, AVictim newItem)
        {
            if (onAreaVictimNumChangedFunc != null)
                onAreaVictimNumChangedFunc.Invoke(op, oldItem, newItem);
        }

        protected void SendResponse(string type, string msg, object data = null)
        {
            if (onResponseRecvFunc != null)
            {
                ActorNetResponse response = new ActorNetResponse();

                response.m_ResponseType = type;
                response.m_Message = msg;
                response.m_Data = data;

                onResponseRecvFunc.Invoke(response);
            }
        }

        protected virtual void OnDestroy()
        {
            if (GameState.Current != null)
                GameState.Current.RemoveEmtActor(this);
        }
    }


    // events
    public class AreaEnterExitEvent : UnityEvent<AVictimPlaceableArea, AVictimPlaceableArea> { }

    public struct ActorNetResponse
    {
        public string m_ResponseType;
        public string m_Message;
        public object m_Data;
    }

    public class ActorNetResponseEvent : UnityEvent<ActorNetResponse> { }

    public class ListChangedEvent<T> : UnityEvent<EListOperation, T, T> { }

    public class AreaVictimNumChangedEvent : UnityEvent<EListOperation, AVictim, AVictim> { }
}
