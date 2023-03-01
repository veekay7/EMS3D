using UnityEngine;

public class GUIBase : MonoBehaviour
{
    [HideInInspector]
    public RectTransform m_RectTransform;


    protected virtual void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    protected virtual void OnValidate()
    {
        m_RectTransform = gameObject.GetOrAddComponent<RectTransform>();
    }
}
