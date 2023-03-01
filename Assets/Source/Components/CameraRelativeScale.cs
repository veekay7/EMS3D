using UnityEngine;

/// <summary>
/// This script scales an object relative to a camera's distance. This gives the appearance of the object size being the same. Useful for GUI objects that appear within the game scene.
/// Often useful when combined with CameraFacingBillboard.
/// This measures the distance from the Camera plane, rather than the camera itself, and uses the initial scale as a basis. Use the public objectScale variable to adjust the object size.
/// Place this script on the gameobject you wish to keep a constant size.
/// Reference: https://wiki.unity3d.com/index.php/CameraRelativeScale
/// </summary>
[ExecuteInEditMode]
public class CameraRelativeScale : MonoBehaviour
{
    public Camera m_EventCamera;
    public float m_ObjectScale = 1.0f;

    private Vector3 m_InitialScale;


    /// <summary>
    /// Set the initial scale, and setup reference camera.
    /// </summary>
    private void Start()
    {
        // If no specific camera, grab the default camera.
        if (m_EventCamera == null)
            m_EventCamera = Camera.main;

        // Record initial scale, use this as a basis.
        m_InitialScale = transform.localScale;

        if (m_EventCamera == null)
            Debug.LogError("CameraRelativeScale::m_eventCamera");
    }

    /// <summary>
    /// Scale object relative to distance from camera plane.
    /// </summary>
    private void Update()
    {
        if (m_EventCamera == null)
            return;

        Plane plane = new Plane(m_EventCamera.transform.forward, m_EventCamera.transform.position);
        float dist = plane.GetDistanceToPoint(transform.position);
        transform.localScale = m_InitialScale * dist * m_ObjectScale;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (m_EventCamera == null)
                m_EventCamera = Camera.main;

            //if (m_EventCamera == null)
            //    Debug.LogError("CameraRelativeScale::m_eventCamera");
        }
    }
}