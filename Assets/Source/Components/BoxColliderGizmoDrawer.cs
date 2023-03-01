using UnityEngine;

public class BoxColliderGizmoDrawer : MonoBehaviour
{
    public bool m_DrawGizmosSelected = false;
    public Color m_Color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    [HideInInspector]
    public BoxCollider m_BoxCollider;


    private void OnDrawGizmos()
    {
        if (!m_DrawGizmosSelected)
            DrawBoxCollider();
    }

    private void OnDrawGizmosSelected()
    {
        if (m_DrawGizmosSelected)
            DrawBoxCollider();
    }

    private void DrawBoxCollider()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = m_Color;

        if (m_BoxCollider != null)
        {
            Gizmos.DrawWireCube(transform.position + m_BoxCollider.center, m_BoxCollider.size);
        }

        Gizmos.color = oldColor;
    }

    private void OnValidate()
    {
        if (m_BoxCollider == null)
            m_BoxCollider = GetComponent<BoxCollider>();
    }
}