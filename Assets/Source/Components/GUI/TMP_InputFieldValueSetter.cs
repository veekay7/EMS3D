using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class TMP_InputFieldValueSetter : MonoBehaviour
{
    [HideInInspector]
    public TMP_InputField m_InputField;


    private void Awake()
    {
        m_InputField = GetComponent<TMP_InputField>();
    }

    public void SetInt(int value)
    {
        m_InputField.text = value.ToString();
    }

    public void SetFloat(float value)
    {
        m_InputField.text = value.ToString();
    }

    public void SetDouble(double value)
    {
        m_InputField.text = value.ToString();
    }

    private void OnValidate()
    {
        m_InputField = GetComponent<TMP_InputField>();
    }
}
