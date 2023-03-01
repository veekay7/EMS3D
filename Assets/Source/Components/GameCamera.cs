using UnityEngine;

public class GameCamera : MonoBehaviour
{
    // main camera component that is attached
    [HideInInspector]
    public Camera m_CameraComponent;

    // reference to the user interface sub camera
    public Camera m_UICam;

    // current main camera fov
    public float m_FieldOfView;

    // main camera near clip plane 
    public float m_NearClipPlane;

    // main camera far clip plane
    public float m_FarClipPlane;

    // minimum main camera fov
    public float m_MinFov;

    // maximum main camera field of view
    public float m_MaxFov;

    private Vector3 m_initialPos;
    private Quaternion m_initialRot;

    public static GameCamera Current { get; private set; }


    private void Awake()
    {
        if (Current != null && Current == this)
            return;

        m_CameraComponent = GetComponent<Camera>();
        m_initialPos = transform.position;
        m_initialRot = transform.rotation;
        Current = this;
    }

    private void Reset()
    {
        m_FieldOfView = 60.0f;
        m_NearClipPlane = 0.5f;
        m_FarClipPlane = 1500.0f;
        m_MinFov = 10.0f;
        m_MaxFov = 100.0f;
    }

    private void LateUpdate()
    {
        UpdateEye();
    }

    public void ResetToInitialTransform()
    {
        transform.SetParent(null);
        transform.position = m_initialPos;
        transform.rotation = m_initialRot;
    }

    public void Zoom(float amount)
    {
        m_FieldOfView += amount;
        m_FieldOfView = Mathf.Clamp(m_FieldOfView, m_MinFov, m_MaxFov);
    }

    private void UpdateEye()
    {
        if (m_CameraComponent == null)
            return;

        m_CameraComponent.fieldOfView = m_FieldOfView;
        m_CameraComponent.nearClipPlane = m_NearClipPlane;
        m_CameraComponent.farClipPlane = m_FarClipPlane;
    }

    private void OnValidate()
    {
        UpdateEye();
    }
}