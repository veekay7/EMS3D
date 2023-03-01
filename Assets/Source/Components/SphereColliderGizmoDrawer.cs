using UnityEngine;

public class SphereColliderGizmoDrawer : MonoBehaviour
{
    [HideInInspector]
    public SphereCollider m_SphereCollider;

    public bool m_DrawGizmosSelected;
    public Color m_Color = new Color(1.0f, 1.0f, 1.0f, 1.0f);


    private void OnDrawGizmos()
    {
        if (!m_DrawGizmosSelected)
            DrawSphereCollider();
    }

    private void OnDrawGizmosSelected()
    {
        if (m_DrawGizmosSelected)
            DrawSphereCollider();
    }

    private void DrawSphereCollider()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = m_Color;

        if (m_SphereCollider != null)
        {
            Gizmos.DrawWireSphere(transform.position + m_SphereCollider.center, m_SphereCollider.radius);
        }

        Gizmos.color = oldColor;
    }

    private void OnValidate()
    {
        if (m_SphereCollider == null)
            m_SphereCollider = GetComponent<SphereCollider>();
    }
}