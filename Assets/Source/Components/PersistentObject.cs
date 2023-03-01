using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Makes an object persistent throughout all scenes.
/// </summary>
public class PersistentObject : MonoBehaviour
{
    public bool m_SetNoDestroyOnEnabled;

    private static List<GameObject> m_persistentObjs = new List<GameObject>();


    public static GameObject GetPersistentObject(string objTargetname)
    {
        for (int i = 0; i < m_persistentObjs.Count; i++)
        {
            if (m_persistentObjs[i].name.Equals(objTargetname))
                return m_persistentObjs[i];
        }

        return null;
    }

    private void Awake()
    {
        if (!m_SetNoDestroyOnEnabled)
        {
            DontDestroyThisShit();
        }
    }

    private void OnEnable()
    {
        if (m_SetNoDestroyOnEnabled)
        {
            DontDestroyThisShit();
        }
    }

    private void DontDestroyThisShit()
    {
        if (!m_persistentObjs.Contains(gameObject))
        {
            m_persistentObjs.Add(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (m_persistentObjs.Contains(gameObject))
            m_persistentObjs.Remove(gameObject);
    }
}