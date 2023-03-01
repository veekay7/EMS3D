using UnityEngine;

/// <summary>
/// Makes the object which it is attached to align itself with the camera.
/// This is useful for billboards which should always face the camera and be the same way up as it is.
/// Place this script on a GameObject that you want to face the camera. Then, with the object selected, use the inspector to select the Camera you want the object to face.
/// You might want to change Vector3.back to Vector3.front, depending on the initial orientation of your object.
/// Reference: https://wiki.unity3d.com/index.php/CameraFacingBillboard
/// </summary>
[ExecuteInEditMode]
public class CameraFacingBillboard : MonoBehaviour
{
    public enum Axis { Up, Down, Left, Right, Forward, Back };

    public Camera m_EventCamera;
    public bool m_ReverseFace = false;
    public Axis m_Axis = Axis.Up;


    private void Awake()
    {
        // If no event camera has been explicitly referenced, grab the main camera!!
        if (m_EventCamera == null)
            m_EventCamera = Camera.main;

        if (m_EventCamera == null)
            Debug.LogError("CameraFacingBilboard::m_eventCamera");
    }
    
    private void LateUpdate()
    {
        if (m_EventCamera == null)
            return;

        // Orient the camera after all movement is completed this frame to avoid jittering.
        Vector3 targetPos = transform.position + m_EventCamera.transform.rotation * (m_ReverseFace ? Vector3.forward : Vector3.back);
        Vector3 targetOrientation = m_EventCamera.transform.rotation * GetAxis(m_Axis);
        transform.LookAt(targetPos, targetOrientation);
    }

    public Vector3 GetAxis(Axis axis)
    {
        switch (axis)
        {
            case Axis.Down:
                return Vector3.down;

            case Axis.Forward:
                return Vector3.forward;

            case Axis.Back:
                return Vector3.back;

            case Axis.Left:
                return Vector3.left;

            case Axis.Right:
                return Vector3.right;
        }

        // Default is Vector3.up
        return Vector3.up;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (m_EventCamera == null)
            {
                m_EventCamera = Camera.main;
                //if (m_EventCamera == null)
                //    Debug.LogError("CameraFacingBillboard::m_eventCamera");
            }
        }
    }
}