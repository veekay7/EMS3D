using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameType { Singleplay = 0, Multiplay = 1 }

public static class Globals
{
    public static bool m_DevMode = true;          // make sure this is false when set for release

    // global system settings
    public static GameConfig m_UserGameConfig = new GameConfig();
    public static List<Resolution> m_SupportedResolutions = new List<Resolution>();
    public static int m_PrintSpd = 60;

    // game load parameters
    public static EGameType m_SelectedGameType = EGameType.Singleplay;
    public static int m_SelPlayerType = -1;

    public static string m_ServerNameString = "New Multiplayer Game";
    public static MapListEntry m_CurrentMap = null;
    public static int m_MaxPlayers = 4;
    public static bool m_EnableBots = false;
    public static int m_BotCaps = 0;
    public static int m_NumVictims = Consts.VICTIM_COUNT_DEFAULT;
    public static int m_Probability_P3 = Consts.VICTIM_PROB_P3_DEFAULT;
    public static int m_Probability_P2 = Consts.VICTIM_PROB_P2_DEFAULT;
    public static int m_Probability_P1 = Consts.VICTIM_PROB_P1_DEFAULT;
    public static int m_Probability_P0 = Consts.VICTIM_PROB_P0_DEFAULT;
}
