using System;

public class ConCommand
{
    private string m_name;
    private string m_helpString;
    private Action<ConCommand, string> m_callback;
    

    public string Name { get => m_name; }

    public string HelpString { get => m_helpString; }

    public bool RequireValue { get; private set; }


    public ConCommand(string name, string helpString, Action<ConCommand, string> callback)
    {
        string[] props = name.Split(' ');
        m_name = props[0];
        m_helpString = helpString;
        m_callback = callback;

        if (props.Length > 1)
        {
            if (!string.IsNullOrEmpty(props[1]))
                RequireValue = true;
        }
    }

    public void Invoke(string args)
    {
        if (m_callback != null)
            m_callback.Invoke(this, args);
    }
}