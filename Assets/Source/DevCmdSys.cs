using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Globalization;

public class DevCmdSys : SingletonBehaviour<DevCmdSys>, ILogHandler
{
    public const int MAX_BUFFER_SIZE = 256;        // must be divisible by 2 or power of 2. maximum is 256!!
    public const string STD_COLOR = "#FFFFFF";
    public const string REQUIRED_COLOR = "#FA8072";
    public const string OPTIONAL_COLOR = "#00FF7F";
    public const string WARNING_COLOR = "#ffcc00";
    public const string EXECUTED_COLOR = "#e600e6";
    public readonly string[] SEPARATORS = new string[] { "\r\n", "\r", "\n" };

    public GameObject m_GUIConWndPrefab;

    private ILogHandler m_defaultLogHandler;
    private GUIConWnd m_conWnd;
    private int m_lineCount;
    private string m_bufferString;
    private string m_inputString;
    private List<ConCommand> m_cmds = new List<ConCommand>();

    public bool LogEnabled { get; set; }

    public LogType FilterLogType { get; set; }

    public string Buffer { get => m_bufferString; }


    protected override void AfterAwake()
    {
        base.AfterAwake();

        m_defaultLogHandler = null;
        m_conWnd = null;
        m_lineCount = 0;
        m_bufferString = string.Empty;
        m_inputString = string.Empty;

        LogEnabled = true;
        FilterLogType = LogType.Log;
    }

    private void Start()
    {
        m_defaultLogHandler = Debug.unityLogger.logHandler;

        // initialise the basic commands
        AddCommand(new ConCommand("echo <input>", "Usage: echo <message>\nPrints a message to the console.", (cmd, args) => Echo(args)));
        AddCommand(new ConCommand("cls", "Clears the console screen.", (cmd, args) => ClearBuffer()));
        AddCommand(new ConCommand("exit", "Exits the game.", (cmd, args) => GameSystem.QuitGame()));

        // create the GUI window
        CreateWindow();
        //StartCoroutine(Co_WaitThenOpenWindow(3.0f));
    }

    internal void UpdateInputText(string arg0)
    {
        m_inputString = arg0;
    }

    private IEnumerator Co_WaitThenOpenWindow(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_conWnd.Show();
    }

    private void Update()
    {
        if (m_conWnd != null)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ParseInput(m_inputString);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tilde))
            {
                m_conWnd.Hide();
            }
        }
    }

    public void CreateWindow()
    {
        if (m_GUIConWndPrefab != null)
        {
            var conWndObject = Instantiate(m_GUIConWndPrefab);
            m_conWnd = conWndObject.GetComponent<GUIConWnd>();
            m_conWnd.SetCmdSystem(this);
        }
        else
        {

            Debug.LogError("Cannot create console window. m_GUIConWndPrefab is null.");
        }
    }

    public void DestroyWindow()
    {
        if (m_conWnd != null)
            Destroy(m_conWnd.gameObject);
    }

    public void AddCommand(ConCommand newCmd)
    {
        // check if a command with an id is already registered
        for (int i = 0; i < m_cmds.Count; i++)
        {
            if (m_cmds[i].Name.Equals(newCmd.Name))
                return;
        }

        m_cmds.Add(newCmd);
    }

    public ConCommand FindCommand(string name)
    {
        for (int i = 0; i < m_cmds.Count; i++)
        {
            if (m_cmds[i].Name.Contains(name))
                return m_cmds[i];
        }

        return null;
    }

    public ConCommand[] GetCommands()
    {
        return m_cmds.ToArray();
    }

    public static void Echo(string message)
    {
        if (Instance == null)
            return;

        // find empty space to add to buffer
        if (string.IsNullOrEmpty(message))
            Instance.m_bufferString = string.Concat(Instance.m_bufferString, "<color={STD_COLOR}>echo</color>\n");
        else
            Instance.m_bufferString = string.Concat(Instance.m_bufferString, "<color={STD_COLOR}>" + message + "</color>\n");

        Instance.ResizeBuffer();
    }

    public static void ClearBuffer()
    {
        if (Instance == null)
            return;

        Instance.m_lineCount = 0;
        Instance.m_bufferString = string.Empty;
    }

    public static void Dump()
    {
        if (string.IsNullOrEmpty(Instance.m_bufferString))
        {
            Echo("Cannot dump console text. Console buffer is empty.");
            return;
        }

        DateTime now = DateTime.Now;
        string dateString = now.Date.Day.ToString() + now.Date.Month.ToString() + now.Date.Year.ToString();
        string timeString = "h" + now.TimeOfDay.Hours.ToString() + "m" + now.TimeOfDay.Minutes.ToString() + "s" + now.TimeOfDay.Seconds.ToString();
        string dumpFilePathString = Application.dataPath + "\\Logs\\Log_Dt" + dateString + "_" + timeString + ".txt";
#if UNITY_EDITOR
        dumpFilePathString = Application.dataPath + "\\..\\Logs\\Log_Dt" + dateString + "_T" + timeString + ".txt";
#endif

        FileStream m_fileStream = new FileStream(dumpFilePathString, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter m_streamWriter = new StreamWriter(m_fileStream);

        // print to file
        m_streamWriter.WriteLine("*************************************************\n");
        m_streamWriter.WriteLine("*                  Console Log                  *\n");
        m_streamWriter.WriteLine("*************************************************\n");
        m_streamWriter.WriteLine("\nDate: " + dateString + "\n");
        m_streamWriter.WriteLine("\nTime: " + timeString + "\n\n");

        m_streamWriter.Write(Instance.m_bufferString.ToCharArray());
        m_streamWriter.WriteLine("\n\nEND OF FILE");
        m_streamWriter.Flush();
        
        m_streamWriter.Close();
        m_fileStream.Close();

        Echo("Dumped console text to " + dumpFilePathString);
    }

    public void Log(object message)
    {
        LogFormat(LogType.Log, null, "{0}", (object)GetStringFromMsgObject(message));
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        string line = string.Format(format, args);

        // append to buffer
        m_bufferString = string.Concat(m_bufferString, "<color={STD_COLOR}>" + line + "</color>");
        m_bufferString = string.Concat(m_bufferString, "\n");

        // print on unity console
        m_defaultLogHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        // print on unity console
        m_defaultLogHandler.LogException(exception, context);
    }

    public bool IsLogTypeAllowed(LogType logType)
    {
        if (logType == LogType.Exception)
            return true;
        if (FilterLogType != LogType.Exception)
            return logType <= this.FilterLogType;

        return false;
    }

    private void ParseInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;

        Echo("] " + input);
        string[] props = input.Split(' ');

        // try to see if the command exists
        bool found = false;
        var cmds = GetCommands();
        for (int i = 0; i < cmds.Length; i++)
        {
            var cmd = cmds[i];
            if (props[0].Equals(cmd.Name))
            {
                found = true;
                if (cmd.RequireValue)
                {
                    if (props.Length == 1)
                        Echo(cmd.HelpString);
                    else
                        cmd.Invoke(props[1]);
                }
                else
                {
                    cmd.Invoke(null);
                }

                break;
            }
        }

        if (!found)
        {
            Echo("Invalid command " + props[0]);
        }

        m_inputString = string.Empty;
    }

    private void ResizeBuffer()
    {
        string[] lines = m_bufferString.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
        m_lineCount = lines.Length;

        // if line count is more than buffer size, clear the first half of the buffer and recombine the strings
        if (m_lineCount > MAX_BUFFER_SIZE)
        {
            m_bufferString = string.Empty;
            m_lineCount = MAX_BUFFER_SIZE / 2;
            for (int i = 0; i < m_lineCount - 1; i++)
            {
                m_bufferString = string.Concat(m_bufferString, lines[i] + "\n");
            }
        }
    }

    private static string GetStringFromMsgObject(object message)
    {
        if (message == null)
            return "Null";

        IFormattable formattable = message as IFormattable;
        if (formattable != null)
            return formattable.ToString((string)null, (IFormatProvider)CultureInfo.InvariantCulture);

        return message.ToString();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            if (m_GUIConWndPrefab != null)
            {
                if (!m_GUIConWndPrefab.TryGetComponent(out GUIConWnd conWnd))
                {
                    Debug.LogError("m_GUIConWndPrefab does not contain GUIConWnd component.");
                    m_GUIConWndPrefab = null;
                }
            }
        }
#endif
    }
}
