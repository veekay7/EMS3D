using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldValueSetter : MonoBehaviour
{
    [HideInInspector]
    public InputField m_InputField;


    private void Awake()
    {
        m_InputField = GetComponent<InputField>();
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
        m_InputField = GetComponent<InputField>();
    }
}
