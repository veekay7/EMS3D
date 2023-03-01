using UnityEngine;

public class PopupboxFactory : MonoBehaviour
{
    public GUIPopupAlert        m_AlertBoxPrefab;
    public GUIPopupConfirm      m_ConfirmBoxPrefab;
    public GUIDialogueBox01     m_DialogueBox01Prefab;

    public static PopupboxFactory Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance == this)
            return;

        Instance = this;
    }

    public T Create<T>(RectTransform parent = null) where T : GUIPopupBase
    {
        GUIPopupBase instance = null;
        if (typeof(T) == typeof(GUIPopupAlert))
        {
            instance = Instantiate(m_AlertBoxPrefab);
        }
        else if (typeof(T) == typeof(GUIPopupConfirm))
        {
            instance = Instantiate(m_ConfirmBoxPrefab);
        }
        else if (typeof(T) == typeof(GUIDialogueBox01))
        {
            instance = Instantiate(m_DialogueBox01Prefab);
        }

        // do parenting
        if (instance != null)
        {
            if (parent != null)
                instance.m_RectTransform.SetParent(parent, true);
        }

        return (T)instance;
    }
}
