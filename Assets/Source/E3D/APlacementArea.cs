using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class APlacementArea : MonoBehaviour
{
    [HideInInspector]
    public SphereCollider m_Collider;

    public float m_Radius;


    private void Reset()
    {
        m_Radius = 1.0f;
    }

    private void OnDrawGizmos()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = Color.cyan;

        Matrix4x4 originalMtx = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireSphere(Vector3.zero, m_Radius);

        Gizmos.matrix = originalMtx;
        Gizmos.color = oldColor;

        // draw icon too
        Gizmos.DrawIcon(transform.position + Vector3.up, "build_zone.png", true);
    }

    private void OnValidate()
    {
        m_Collider = gameObject.GetOrAddComponent<SphereCollider>();

        if (m_Radius <= 0.0f)
            m_Radius = 0.1f;

        if (m_Collider != null)
            m_Collider.radius = m_Radius;
    }
}
