using DG.Tweening;
using E3D;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Victim Settings
/// settings for how victims will be generated in the game
/// </summary>
[Serializable]
public struct VictimSettings
{
    // number of victims that will be created for this map
    [Range(1, 100)]
    public int m_NumVictims;
    [Range(0, 100)]
    public int m_Probability_P3;
    [Range(0, 100)]
    public int m_Probability_P2;
    [Range(0, 100)]
    public int m_Probability_P1;
    [Range(0, 100)]
    public int m_Probability_P0;
    [Min(1)]
    public int m_AgeGap;


    public VictimSettings(int victimCount, int prob_p0, int prob_p1, int prob_p2, int prob_p3, int ageGap)
    {
        m_NumVictims = victimCount < 1 ? 1 : victimCount;
        m_Probability_P0 = prob_p0;
        m_Probability_P1 = prob_p1;
        m_Probability_P2 = prob_p2;
        m_Probability_P3 = prob_p3;
        m_AgeGap = ageGap <= 0 ? 5 : ageGap;
    }
}


/// <summary>
/// Game System
/// </summary>
[DisallowMultipleComponent]
public class GameSystem : MonoBehaviour
{
    [HideInInspector]
    public NetworkManager m_NetManager;
    [HideInInspector]
    public GameConfigManager m_GameConfigManager;

    public AudioMixer m_AudioMixer;
    public VictimSettings m_VictimSettings;
    public List<MapListEntry> m_MapList = new List<MapListEntry>();

    [ReorderableList]
    public List<GameObject> m_SpawnablePrefabs = new List<GameObject>();


    public static GameSystem Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Sys_BeforeSceneLoad()
    {
        // if the game system has not been initialised, then we must load the "sys_main" scene file
        if (!Instance)
        {
            Scene rootScene = SceneManager.GetSceneByName("sys_main");
            if (rootScene.buildIndex == -1)
                SceneManager.LoadScene("sys_main", LoadSceneMode.Additive);
        }

        // build resolution list
        // reference: https://answers.unity.com/questions/1463609/screenresolutions-returning-duplicates.html
        if (Globals.m_SupportedResolutions.Count == 0)
        {
            HashSet<Resolution> resFiltered = new HashSet<Resolution>();
            foreach (var r in Screen.resolutions)
            {
                Resolution newRes = new Resolution();
                newRes.width = r.width;
                newRes.height = r.height;
                newRes.refreshRate = 0;

                resFiltered.Add(newRes);
            }

            //var resFiltered = Screen.resolutions
            //    .GroupBy(r => new { r.width, r.height })
            //    .Select(grp => new Resolution { width = grp.Key.width, height = grp.Key.height, refreshRate = 0 });
            var resolutions = resFiltered.ToArray();

            for (int i = 0; i < resolutions.Length; i++)
            {
                Globals.m_SupportedResolutions.Add(resolutions[i]);
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance == this)
            return;

        m_GameConfigManager = GetComponent<GameConfigManager>();

        m_VictimSettings = new VictimSettings(Consts.VICTIM_COUNT_DEFAULT, 
            Consts.VICTIM_PROB_P0_DEFAULT, 
            Consts.VICTIM_PROB_P1_DEFAULT, 
            Consts.VICTIM_PROB_P2_DEFAULT, 
            Consts.VICTIM_PROB_P3_DEFAULT, 
            Consts.VICTIM_AGE_GAP_DEFAULT);

        DOTween.Init();

        // reigister scene manager events
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        Instance = this;
    }

    private void Start()
    {
        // create the game save storage path
        if (!Directory.Exists(Consts.SaveGameStorePath))
            Directory.CreateDirectory(Consts.SaveGameStorePath);

        string configFilePath = GetConfigFilePath();

        if (!File.Exists(configFilePath))
        {
            // if no game config file exists, create one now!!
            Globals.m_UserGameConfig.m_ResWidth = Screen.currentResolution.width;
            Globals.m_UserGameConfig.m_ResHeight = Screen.currentResolution.height;
            Globals.m_UserGameConfig.m_RefreshRate = Screen.currentResolution.refreshRate;
            Globals.m_UserGameConfig.m_FullScreen = true;

            // serialize the default game config file as the new usercfg file
            GameConfig.WriteToFile(Globals.m_UserGameConfig, configFilePath);
        }
        else
        {
            // otherwise just load it from file and apply the game config
            if (!GameConfig.LoadFromFile(ref Globals.m_UserGameConfig, configFilePath))
                throw new ApplicationException("Failed to load game config file.");
        }

        // check if the resolution attached to the current game config is valid
        // apply supported resolution if unsupported resolution is found in game config
        bool isValidResolution = false;
        for (int i = 0; i < Globals.m_SupportedResolutions.Count; i++)
        {
            var res = Globals.m_SupportedResolutions[i];

            if (res.width == Globals.m_UserGameConfig.m_ResWidth && 
                res.height == Globals.m_UserGameConfig.m_ResHeight)
            {
                isValidResolution = true;
            }
        }

        if (!isValidResolution)
        {
            Debug.LogError("Invalid resolution!");

            // find the highest supported resolution and set that
            var bestSupportedRes = Globals.m_SupportedResolutions[Screen.resolutions.Length - 1];

            Globals.m_UserGameConfig.m_ResWidth = bestSupportedRes.width;
            Globals.m_UserGameConfig.m_ResHeight = bestSupportedRes.height;
        }

        // apply game config to system
        m_GameConfigManager.Apply(Globals.m_UserGameConfig);
    }

    public MapListEntry[] GetMapList()
    {
        return m_MapList.ToArray();
    }

    public void DisconnectPlayer(E3DPlayer player)
    {
        Globals.m_CurrentMap = null;

        if (GUIController.Instance != null)
        {
            if (GUIController.Instance.CurrentActiveScreen != null && GUIController.Instance.CurrentActiveScreen.m_Classname == "Pause")
                GUIController.Instance.CloseCurrentScreen();

            if (GUIController.Instance.CurrentActiveScreen != null && GUIController.Instance.CurrentActiveScreen.m_Classname == "Result")
                GUIController.Instance.CloseCurrentScreen();
        }

        if (ScreenWiper.Instance != null)
            ScreenWiper.Instance.SetFilled(true);

        SceneManager.LoadScene("gamemenu");
    }

    /// <summary>
    /// Returns the path of the game configuration file.
    /// </summary>
    /// <returns></returns>
    public static string GetConfigFilePath()
    {
        //if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor)
        //{
        //    gameDataPath += "..\\..\\EmuUserData\\";
        //    if (!Directory.Exists(gameDataPath))
        //        Directory.CreateDirectory(gameDataPath);
        //}

        return Consts.SaveGameStorePath + "usercfg.xml";
    }

    /// <summary>
    /// Quits the game. If the game is in Unity Editor mode, the play mode will just stop.
    /// </summary>
    public static void QuitGame()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }
#else
        Application.Quit();
#endif
    }

    public void ApplyGameConfigToSystem(GameConfig config)
    {
        Screen.SetResolution(config.m_ResWidth, config.m_ResHeight, config.m_FullScreen);
        AudioListener.volume = config.m_MasterVolume / 100.0f;
    }

    public void SetBgmVolume(int level)
    {
        float log = GetLogarithmicVolumeLvl(level);
        m_AudioMixer.SetFloat("BgmVolume", log);
    }

    public void SetSfxVolume(int level)
    {
        float log = GetLogarithmicVolumeLvl(level);
        m_AudioMixer.SetFloat("SfxVolume", log);
    }

    public T Spawn<T>() where T : MonoBehaviour
    {
        for (int i = 0; i < m_SpawnablePrefabs.Count; i++)
        {
            var prefab = m_SpawnablePrefabs[i].GetComponent<T>();
            if (prefab is T)
            {
                GameObject newSpawnObject = Instantiate(m_SpawnablePrefabs[i]);
                T newSpawn = newSpawnObject.GetComponent<T>();
                
                return newSpawn;
            }
        }

        return null;
    }

    private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
    {
        if ((newScene.name.Equals("dev_guiedit") || newScene.name.Equals("dev_prefabedit") ||
            newScene.name.Equals("gamemenu") || newScene.name.Equals("loading") || newScene.name.Equals("sys_main")))
            return;

        m_VictimSettings.m_NumVictims = Globals.m_NumVictims;
        m_VictimSettings.m_Probability_P3 = Globals.m_Probability_P3;
        m_VictimSettings.m_Probability_P2 = Globals.m_Probability_P2;
        m_VictimSettings.m_Probability_P1 = Globals.m_Probability_P1;
        m_VictimSettings.m_Probability_P0 = Globals.m_Probability_P0;

        ScreenWiper.Instance.SetFilled(false);
    }

    private float GetLogarithmicVolumeLvl(float level)
    {
        if (level <= Mathf.Epsilon)
            level = Mathf.Epsilon;

        float log10 = Mathf.Log10(level);
        float result = log10 * 20.0f;

        return result;
    }

    private float GetLogarithmicVolumeLvl(int level)
    {
        float normalizedLevel = level / 100.0f;

        if (level == 0)
            normalizedLevel = Mathf.Epsilon;

        float log10 = Mathf.Log10(normalizedLevel);
        float result = log10 * 20.0f;

        return result;
    }

    private void OnValidate()
    {
        m_GameConfigManager = GetComponent<GameConfigManager>();
    }
}
