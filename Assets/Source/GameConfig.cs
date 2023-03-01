using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
[XmlRoot("Config")]
public class GameConfig : ICloneable
{
    [XmlElement("ResolutionX")]
    public int m_ResWidth;
    [XmlElement("ResolutionY")]
    public int m_ResHeight;
    [XmlElement("RefreshRate")]
    public int m_RefreshRate;
    [XmlElement("bIsFullScreen")]
    public bool m_FullScreen;
    [XmlElement("MasterVolume")]
    public int m_MasterVolume;
    [XmlElement("SFXVolume")]
    public int m_SfxVolume;
    [XmlElement("BGMVolume")]
    public int m_BgmVolume;
    [XmlElement("PrintSpeed")]
    public int m_PrintSpd;
    [XmlElement("ScrollSensitivity")]
    public int m_ScrollSensitivity;


    public GameConfig()
    {
        Reset();
    }

    public GameConfig(GameConfig config)
    {
        OverwriteFrom(config);
    }

    public void Reset()
    {
        m_ResWidth = 1280;
        m_ResHeight = 720;
        m_RefreshRate = 60;
        m_FullScreen = false;
        m_MasterVolume = 75;
        m_SfxVolume = 100;
        m_BgmVolume = 100;
        m_PrintSpd = 60;
        m_ScrollSensitivity = 5;
    }

    public void OverwriteFrom(GameConfig config)
    {
        m_ResWidth = config.m_ResWidth;
        m_ResHeight = config.m_ResHeight;
        m_RefreshRate = config.m_RefreshRate;
        m_FullScreen = config.m_FullScreen;
        m_MasterVolume = config.m_MasterVolume;
        m_SfxVolume = config.m_SfxVolume;
        m_BgmVolume = config.m_BgmVolume;
        m_PrintSpd = config.m_PrintSpd;
    }

    public object Clone()
    {
        return new GameConfig(this);
    }

    public bool Equals(GameConfig other)
    {
        if (m_ResWidth == other.m_ResWidth && m_ResHeight == other.m_ResHeight && m_RefreshRate == other.m_RefreshRate &&
            m_FullScreen == other.m_FullScreen &&
            m_MasterVolume == other.m_MasterVolume && m_SfxVolume == other.m_SfxVolume && m_BgmVolume == other.m_BgmVolume && m_PrintSpd == other.m_PrintSpd)
            return true;

        return false;
    }

    public static void WriteToFile(GameConfig config, string path)
    {
        FileStream fs;

        if (!File.Exists(path))
            fs = new FileStream(path, FileMode.Create);
        else
            fs = new FileStream(path, FileMode.Open, FileAccess.Write);

        XmlTextWriter writer = new XmlTextWriter(fs, Encoding.UTF8);
        writer.Formatting = Formatting.Indented;

        XmlSerializer serializer = new XmlSerializer(typeof(GameConfig));
        serializer.Serialize(writer, config);

        writer.Close();
        fs.Close();
    }

    public static bool LoadFromFile(ref GameConfig config, string path)
    {
        // verify the path exists
        if (!File.Exists(path))
            return false;

        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

        XmlSerializer serializer = new XmlSerializer(typeof(GameConfig));
        config = (GameConfig)serializer.Deserialize(fs);

        fs.Close();

        return true;
    }
}
