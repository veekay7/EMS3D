using UnityEngine;

public class GameConfigManager : MonoBehaviour
{
    private GameConfig m_newConfig;

    public GameConfig NewConfig { get => m_newConfig; }

    private void Awake()
    {
        m_newConfig = null;
    }

    private void CreateNewConfigIfRequired()
    {
        if (m_newConfig == null)
        {
            m_newConfig = new GameConfig(Globals.m_UserGameConfig);
        }
    }

    public void SetResolution(int selection)
    {
        var selected = Screen.resolutions[selection];

        CreateNewConfigIfRequired();

        m_newConfig.m_ResWidth = selected.width;
        m_newConfig.m_ResHeight = selected.height;
        m_newConfig.m_RefreshRate = selected.refreshRate;
    }

    public void SetFullScreen(bool value)
    {
        CreateNewConfigIfRequired();
        
        m_newConfig.m_FullScreen = value;
    }

    public void SetTextPrintSpeed(float spd)
    {
        CreateNewConfigIfRequired();

        m_newConfig.m_PrintSpd = (int)spd;
    }

    public void SetMasterVolumeLevel(float level)
    {
        AudioListener.volume = level / 100.0f;

        CreateNewConfigIfRequired();

        m_newConfig.m_MasterVolume = (int)level;
    }

    public void SetBgmVolumeLevel(float level)
    {
        if (GameSystem.Instance != null)
            GameSystem.Instance.SetBgmVolume((int)level);
        else
            Debug.Log("Cannot set bgm volume level, GameSystem.Instance is null");

        CreateNewConfigIfRequired();

        m_newConfig.m_BgmVolume = (int)level;
    }

    public void SetSfxVolumeLevel(float level)
    {
        if (GameSystem.Instance != null)
            GameSystem.Instance.SetSfxVolume((int)level);
        else
            Debug.Log("Cannot set bgm volume level, GameSystem.Instance is null");

        CreateNewConfigIfRequired();

        m_newConfig.m_SfxVolume = (int)level;
    }

    public void CancelChanges()
    {
        AudioListener.volume = Globals.m_UserGameConfig.m_MasterVolume / 100.0f;
        
        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.SetBgmVolume(Globals.m_UserGameConfig.m_BgmVolume);
            GameSystem.Instance.SetSfxVolume(Globals.m_UserGameConfig.m_SfxVolume);
        }
        else
        {
            Debug.Log("Cannot set volume level for bgm and sfx, GameSystem.Instance is null");
        }
            
        m_newConfig = null;
    }

    public void Apply()
    {
        if (m_newConfig != null)
        {
            Globals.m_UserGameConfig.OverwriteFrom(m_newConfig);
            Apply(m_newConfig);
            m_newConfig = null;
        }
    }

    public void Apply(GameConfig gameConfig)
    {
        Screen.SetResolution(gameConfig.m_ResWidth, gameConfig.m_ResHeight, gameConfig.m_FullScreen);
        GameConfig.WriteToFile(gameConfig, GameSystem.GetConfigFilePath());
    }
}
