using UnityEngine;

public static class Consts
{
    public const string LOCALHOST_ADDRESS = "localhost";
    public const ushort LOCALHOST_PORT = 7777;

    //public static string SaveGameStorePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\EMSGame\\";

#if !UNITY_EDITOR 
    public static string SaveGameStorePath = Application.dataPath + "\\Saves\\";
#else
    public static string SaveGameStorePath = Application.dataPath + "\\..\\Saves\\";
#endif
    
    public const string PLAYER_NAME_DEFAULT = "Player";
    public const string PLAYER_NAME_EMPTY = "unnamed";

    // input consts
    public const int LEFT_MOUSE_BUTTON = 0;
    public const int RIGHT_MOUSE_BUTTON = 1;
    public const float MOUSEBTN_CLICK_TIME = 0.2f;      // How long before a mouse button press must be held and released before it is considered a click.
    public const float MOUSEBTN_HOLD_TIME = 0.25f;      // How long before a mouse button press must be held to be considered holding.
    public const float KINDA_SMALL_NUMBER = 0.0001f;
    public static readonly KeyCode[] alphaNumKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

    public const int MIN_DIALOGUE_PRINT_SPD = 10;
    public const int MAX_DIALOGUE_PRINT_SPD = 100;

    // GUI controller screen indexes
    public const int MAIN_MENU = 0;
    public const int TRAINING_MENU = 1;
    public const int CONFIG_MENU = 2;
    public const int CREDITS_SCREEN = 3;
    public const int CLASS_SELECT_MENU = 4;
    public const int PAUSE_SCREEN = 5;
    public const int BRIEFING_SCREEN = 6;
    public const int RESULT_SCREEN = 7;

    // default colours for patient acuity scale p3, p2, p1, p0
    public static readonly Color COLOR_PAC_OK1 = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public static readonly Color COLOR_PAC_OK2 = new Color(0.047f, 0.588f, 0.769f, 1.0f);
    public static readonly Color COLOR_P3 = new Color(0.125f, 0.749f, 0.42f, 1.0f);
    public static readonly Color COLOR_P2 = new Color(0.992f, 0.588f, 0.267f, 1.0f);
    public static readonly Color COLOR_P1 = new Color(0.922f, 0.231f, 0.353f, 1.0f);
    public static readonly Color COLOR_P0 = new Color(0.173f, 0.243f, 0.314f, 1.0f);

    // time scales
    public const float TRAVEL_TIME_SCALE = 1.0f;    // every one minute is an hour (60 minutes). make sure all scales are in minutes

    // min and maxes
    public static int MIN_VICTIM_COUNT = 1;
    public static int MAX_VICTIM_COUNT = 100;
    public static int MIN_VICTIM_AGE_GAP = 1;
    public static int MAX_VICTIM_AGE_GAP = 30;
    public static int MIN_VICTIM_PAC_PROB = 0;
    public static int MAX_VICTIM_PAC_PROB = 100;
    public static int MAX_ITEMS = 99;
    public static float MAX_VICTIM_HEALTH = 100.0f;
    public static float VERY_SMOL_HEALTH = 1.0f;

    // game defaults
    public const int BOT_CAPS_NOVICE = 0;
    public const int BOT_CAPS_NORMAL = 1;
    public const int BOT_CAPS_EXPERT = 2;
    public const int VICTIM_COUNT_DEFAULT = 10;
    public const int VICTIM_PROB_P3_DEFAULT = 25;
    public const int VICTIM_PROB_P2_DEFAULT = 25;
    public const int VICTIM_PROB_P1_DEFAULT = 25;
    public const int VICTIM_PROB_P0_DEFAULT = 25;
    public const int VICTIM_AGE_GAP_DEFAULT = 5;

    // scores
    public const int SCORE_TRIAGE_CORRECT = 5;
    public const int SCORE_TRIAGE_OVER1 = 4;
    public const int SCORE_TRIAGE_OVER2 = 3;
    public const int SCORE_TRIAGE_UNDER1 = 2;
    public const int SCORE_TRIAGE_UNDER2 = 1;
    public const int SCORE_TRIAGE_BAD = 0;
    public const int SCORE_TRIAGE_DAMAGE_RANGE0 = 10;
    public const int SCORE_TRIAGE_DAMAGE_RANGE1 = 9;
    public const int SCORE_TRIAGE_DAMAGE_RANGE2 = 8;
    public const int SCORE_TRIAGE_DAMAGE_RANGE3 = 7;
    public const int SCORE_TRIAGE_DAMAGE_RANGE4 = 6;
    public const int SCORE_TRIAGE_DAMAGE_RANGE5 = 5;
    public const int SCORE_TRIAGE_DAMAGE_RANGE6 = 4;
    public const int SCORE_TRIAGE_DAMAGE_RANGE7 = 3;
    public const int SCORE_TRIAGE_DAMAGE_RANGE8 = 2;
    public const int SCORE_TRIAGE_DAMAGE_RANGE9 = 1;
    public const int SCORE_TRIAGE_DAMAGE_RANGE10 = 0;
    public const float SCORE_TRIAGE_WEIGHT = 0.5f;
    public const int SCORE_EVAC_HOSPITAL_CORRECT = 2;
    public const int SCORE_EVAC_HOSIPTAL_BAD = 1;
    public const int SCORE_EVAC_HOSPITAL_DEAD = 0;
    public const float SCORE_EVAC_WEIGHT = 0.5f;

    // player avg action time values (counted in seconds)
    public const int SCORE_MAX_TIME = 5;
    public const int TREATMENT_AVGTIME_P1 = 1800;
    public const int TREATMENT_AVGTIME_P2 = 600;
    public const int TREATMENT_AVGTIME_P3 = 30;
    public const int TREATMENT_AVGTIME_P0 = 0;
    public const float SCORE_TREATMENT_WEIGHT = 0.5f;
    public const float STABLE_TIME_P3 = 5.0f;
    public const float STABLE_TIME_P2 = 3.0f;
    public const float STABLE_TIME_P1 = 2.0f;

    public static string[] ClearInjuryTags = { "no action", "chest tube", "oxygen", "intubation", "IV drip", "dressing", "analgesia", "immobilization", "torniquet" };
    public static string[] DefaultMaleVictimNames = { "Satisfied Customer", "Goat", "Dislocated Elbow", "!Xok" };
    public static string[] DefaultFemaleVictimNames = { "Cardboard", "Vauxhall", "Ahn" };
}

public static class StatusCodes
{
    public static uint SUCCESS = 0;
    public static uint FAILED = 1;
    public static uint VICTIM_NOT_SELECTED = 2;
    public static uint AMBULANCE_NOT_SELECTED = 3;
    public static uint HOSPITAL_NOT_SELECTED = 4;
    public static uint AMBULANCE_FULL = 5;
    public static uint AMBULANCE_EMPTY = 6;
    public static uint VICTIM_ALREADY_LOADED_INTO_AMBULANCE = 7;
    public static uint VICTIM_USED_BY_OTHER_PLAYER = 8;
}
