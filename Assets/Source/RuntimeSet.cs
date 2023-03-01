using System.Collections.Generic;
using UnityEngine;

public class RuntimeSet<T> : ScriptableObject
{
    public List<T> m_Items = new List<T>();

    public T this[int index]
    {
        get => m_Items[index];
        set => m_Items[index] = value;
    }

    public int Count { get => m_Items.Count; }


    public void Add(T item)
    {
        if (!m_Items.Contains(item))
            m_Items.Add(item);
    }

    public T[] ToArray()
    {
        return m_Items.ToArray();
    }

    public void Remove(T item)
    {
        if (m_Items.Contains(item))
            m_Items.Remove(item);
    }

    public int IndexOf(T item)
    {
        return m_Items.IndexOf(item);
    }

    public bool IsIndexValid(int idx)
    {
        if (idx < 0 || idx >= m_Items.Count)
        {
            Debug.Log("idx is out of range");
            return false;
        }

        return true;
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        m_Items.Clear();
    }

    protected virtual void OnValidate() { }
}