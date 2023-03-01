using System;
using UnityEngine;

namespace E3D
{
    public class E3DLarryModel : MonoBehaviour
    {
        [Serializable]
        public struct SBodyPartVolumes
        {
            public BodyPartVolume head;
            public BodyPartVolume neck;
            public BodyPartVolume chest;
            public BodyPartVolume abdomen;
            public BodyPartVolume leftArm;
            public BodyPartVolume rightArm;
            public BodyPartVolume groin;
            public BodyPartVolume leftLeg;
            public BodyPartVolume rightLeg;
        }

        public GameObject m_ModelObject;
        public Camera m_Camera;
        public BoxCollider m_CeilCollider;
        public BoxCollider m_FloorCollider;
        //public SBodyPartVolumes m_BodyPartAnchors;
        public float m_MoveSpd;
        public float m_TransformResetTime;

        private AVictim m_victim;
        private bool m_isResetting;
        private float m_resetTimeCounter;
        private Vector2 m_vecInput;
        private Vector3 m_initialLocalPos;
        private Vector3 m_lastLocalPos;

        public static E3DLarryModel Current { get; private set; } = null;

        public AVictim Victim { get => m_victim; }

        public bool IsResetting { get => m_isResetting; }

        public bool IsActive { get => gameObject.activeInHierarchy; }


        private void Awake()
        {
            if (Current != null && Current == this)
                return;

            m_victim = null;
            m_isResetting = false;
            m_resetTimeCounter = 0.0f;
            m_vecInput = Vector2.zero;
            m_initialLocalPos = transform.localPosition;
            m_lastLocalPos = transform.localPosition;
            Current = this;
        }

        private void Reset()
        {
            m_MoveSpd = 5.0f;
            m_TransformResetTime = 0.3f;
        }

        private void Update()
        {
            if (!m_isResetting)
            {
                // restrict camera position by doing raycast to colliders from camera pos locally
                Vector3 velocity = m_vecInput * m_MoveSpd;

                Vector3 dir = Vector3.zero;
                if (m_vecInput.y > 0.0f)
                    dir = m_Camera.transform.up;
                else if (m_vecInput.y < 0.0f)
                    dir = m_Camera.transform.up * -1.0f;

                //Debug.DrawLine(m_Camera.transform.position, m_Camera.transform.position + dir, Color.red);

                m_Camera.transform.position += velocity * Time.deltaTime;

                // collision detection for the camera to not go out of bounds
                if (Physics.Raycast(m_Camera.transform.position, dir, out RaycastHit hit, 0.15f))
                {
                    m_Camera.transform.position = hit.point + (hit.normal * 0.1f);
                }
            }
            else
            {
                m_resetTimeCounter += Time.deltaTime;
                if (m_resetTimeCounter >= m_TransformResetTime)
                {
                    m_isResetting = false;
                    m_lastLocalPos = Vector3.zero;
                    return;
                }

                float perc = m_resetTimeCounter / m_TransformResetTime;
                m_Camera.transform.position = Vector3.Lerp(m_lastLocalPos, m_initialLocalPos, perc);
            }
        }

        public bool DoRaycast(Vector3 screenPos, out BodyPartVolume bodyPart)
        {
            bodyPart = null;

            Ray ray = m_Camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Victim Model")))
            {
                GameObject hitObject = hit.transform.gameObject;
                BodyPartVolume hitBodyPart = hitObject.GetComponent<BodyPartVolume>();
                if (hitBodyPart != null)
                {
                    bodyPart = hitBodyPart;
                    return true;
                }
            }

            return false;
        }

        public void SetVictim(AVictim victim)
        {
            m_victim = victim;
        }

        public void SetInputVector(Vector2 vecInput)
        {
            // We should ignore any input from players when the rig is having it's transforms reset.
            // The current input vector should also be zero.
            if (m_isResetting)
            {
                m_vecInput = Vector2.zero;
                return;
            }

            m_vecInput = vecInput;
            m_vecInput.Normalize();
        }

        public void ResetView()
        {
            m_isResetting = true;
            m_lastLocalPos = m_Camera.transform.position;
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                m_Camera.transform.position = m_initialLocalPos;
            }

            gameObject.SetActive(active);
        }
    }
}
