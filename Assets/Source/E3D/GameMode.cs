using System;
using System.Collections;
using UnityEngine;
using URand = UnityEngine.Random;

namespace E3D
{
    // the current state of the match
    public enum EMatchState { BeforeEnter = 0, Enter = 1, WarmUp = 2, InProgress = 3, GameOver = 4 }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(VictimManager))]
    [RequireComponent(typeof(GameState))]
    public class GameMode : MonoBehaviour
    {
        [HideInInspector]
        public GameState m_State;
        [HideInInspector]
        public VictimManager m_VictimManager;

        // temp:
        public GameObject m_PlayerPrefab;

        private EMatchState m_matchState;
        private float m_elapsedGameTime;

        public static GameMode Current { get; private set; }

        public bool AllVictimsTriaged { get => m_State.m_TriagedCount == m_VictimManager.NumVictims; }

        public bool AllVictimsTreated { get => m_State.m_TreatedCount == m_VictimManager.NumVictims; }

        public bool AllVictimsEvacuated { get => m_State.m_EvacuatedCount == m_VictimManager.NumVictims; }

        public bool AllVictimsDead { get => m_State.m_DeadCount == m_VictimManager.NumVictims; }

        public float ElapsedGameTime { get => m_elapsedGameTime; }


        private void Awake()
        {
            if (Current != null && Current == this)
                return;

            m_VictimManager = GetComponent<VictimManager>();
            m_State = GetComponent<GameState>();

            m_matchState = EMatchState.BeforeEnter;
            m_elapsedGameTime = 0.0f;

            Current = this;
        }

        private void Start()
        {
            GameObject newPlayerObject = Instantiate(m_PlayerPrefab);
            E3DPlayer newPlayer = newPlayerObject.GetComponent<E3DPlayer>();

            Debug.Log("Player " + newPlayer.m_DisplayName + " has joined the game.");
        }

        private void Update()
        {
            /* game just before entering */
            if (m_matchState == EMatchState.BeforeEnter)
            {
                var gameSys = GameSystem.Instance;

                int maxVictimCount = gameSys.m_VictimSettings.m_NumVictims;
                int probp3 = gameSys.m_VictimSettings.m_Probability_P3;
                int probp2 = gameSys.m_VictimSettings.m_Probability_P2;
                int probp1 = gameSys.m_VictimSettings.m_Probability_P1;
                int probp0 = gameSys.m_VictimSettings.m_Probability_P0;
                int agegap = gameSys.m_VictimSettings.m_AgeGap;

                if (!m_VictimManager.FinishedCreatingVictims)
                {
                    m_VictimManager.CreateVictims(maxVictimCount);
                }
                else
                {
                    // when all victims have spawned (on the server) initialise all
                    // spawned victims with injuries and other profile things
                    m_VictimManager.InitInfoToVictims(probp3, probp2, probp1, probp0, agegap);
                    
                    // now move to the enter state
                    SetMatchState(EMatchState.Enter);
                }
            }

            /* game in enter state */
            if (m_matchState == EMatchState.Enter)
            {
                if (E3DPlayer.Local != null)
                {
                    bool gameReady = GameReadyCheck();
                    if (gameReady)
                    {
                        OnBeginMatch();

                        SetMatchState(EMatchState.InProgress);
                    }
                }
            }

            /* game is in progress */
            if (m_matchState == EMatchState.InProgress)
            {
                m_elapsedGameTime += Time.deltaTime;

                Update_InProgress();
            }

            /* game has ended */
            if (m_matchState == EMatchState.GameOver)
            {
                return;
            }
        }

        public void SetMatchState(EMatchState newState)
        {
            if (m_matchState == newState)
                return;

            EMatchState lastState = m_matchState;
            m_matchState = newState;

            m_State.m_LastMatchState = lastState;
            m_State.m_CurrentMatchState = newState;
        }

        protected bool GameReadyCheck()
        {
            // we only track players[0] since there is only one player in single player game
            if (E3DPlayer.Local != null && !E3DPlayer.Local.IsReady)
                return false;

            return true;
        }

        protected void OnBeginMatch()
        {
            if (E3DPlayer.Local != null)
            {
                E3DPlayer.Local.EnableInput(true);

                // check the type of agent the player chose
                AEmtBase emt = E3DPlayer.Local.PossessedEmt;
                if (emt is AEmtTriageOffr)
                {
                    AVictim[] victims = m_VictimManager.GetVictims();
                    for (int i = 0; i < victims.Length; i++)
                    {
                        var v = victims[i];
                        SendVictimToRandArea(v);
                    }
                }
                else if (emt is AEmtFirstAidDoc)
                {
                    AEmtFirstAidDoc firstAidDoc = (AEmtFirstAidDoc)emt;

                    m_VictimManager.TriageAllCorrectly();

                    AFirstAidPoint[] firstAidPoints = m_State.m_FirstAidPoints.ToArray();

                    // select a first aid point
                    int idx = URand.Range(0, firstAidPoints.Length);
                    AFirstAidPoint curFap = firstAidPoints[idx];

                    // distribte victims
                    AVictim[] victims = m_VictimManager.GetVictims();
                    for (int i = 0; i < victims.Length; i++)
                    {
                        var v = victims[i];
                        curFap.AddVictim(v);
                    }

                    // send officer to that first aid point
                    firstAidDoc.EnterArea(curFap);
                }
                else if (emt is AEmtEvacOffr)
                {
                    AEmtEvacOffr evacOfficer = (AEmtEvacOffr)emt;

                    m_VictimManager.TriageAllCorrectly();
                    m_VictimManager.TreatAll();

                    AEvacPoint[] evacPoints = m_State.m_EvacPoints.ToArray();
                    AAmbulance[] ambulances = m_State.m_Ambulances.ToArray();

                    int idx = URand.Range(0, evacPoints.Length);
                    AEvacPoint curEvacPoint = evacPoints[idx];

                    // distribute ambulances
                    for (int i = 0; i < ambulances.Length; i++)
                    {
                        curEvacPoint.AddAmbulance(ambulances[i]);
                        ambulances[i].SetEvacPoint(curEvacPoint);
                    }

                    // distribute victims
                    AVictim[] victims = m_VictimManager.GetVictims();
                    for (int i = 0; i < victims.Length; i++)
                    {
                        var v = victims[i];
                        curEvacPoint.AddVictim(v);
                    }

                    evacOfficer.EnterArea(curEvacPoint);
                }

                // start updating the victim manager for damage
                m_VictimManager.StartUpdate();
            }
            else
            {
                Debug.Log("crap!!");
            }
        }

        protected void Update_InProgress()
        {
            // the game tracks to see who you are playing as instead
            if (AllVictimsTriaged || AllVictimsTreated || AllVictimsEvacuated || AllVictimsDead)
            {
                // game over!!
                OnEndMatch();

                SetMatchState(EMatchState.GameOver);
            }
        }

        protected void OnEndMatch()
        {
            // inform all players that the game is over
            if (E3DPlayer.Local == null)
                return;

            E3DPlayer.Local.GameOver();
        }

        public void SendVictimToRandArea(AVictim victim)
        {
            var areas = m_State.m_CasualtyPoints.ToArray();
            if (areas != null && areas.Length > 0)
            {
                int idx = URand.Range(0, areas.Length);
                areas[idx].AddVictim(victim);
            }
        }

        public void SendVictimToRandFirstAidPoint(AVictim victim)
        {
            var firstAidPoints = m_State.m_FirstAidPoints.ToArray();
            if (firstAidPoints != null && firstAidPoints.Length > 0)
            {
                int idx = URand.Range(0, firstAidPoints.Length);
                firstAidPoints[idx].AddVictim(victim);
            }
        }

        public void SendVictimToRandEvacPoint(AVictim victim)
        {
            var evacPoints = m_State.m_EvacPoints.ToArray();
            if (evacPoints != null && evacPoints.Length > 0)
            {
                int idx = URand.Range(0, evacPoints.Length);
                evacPoints[idx].AddVictim(victim);
            }
        }

        public bool CheckVictimWronglyTriagedDead(AVictim victim)
        {
            // End the game immediately if victim is wrongly triaged as dead and vice versa!!
            if ((victim.m_PACS != EPACS.P0 && victim.m_State.m_GivenPACS == EPACS.P0) ||
                (victim.m_PACS == EPACS.P0 && victim.m_State.m_GivenPACS != EPACS.P0))
                return true;

            return false;
        }

        public bool CanSendVictimToFirstAid(AVictim victim)
        {
            return (victim.m_State.m_GivenPACS != EPACS.None) || (victim.m_State.m_GivenPACS != EPACS.P0);
        }

        public EPACS CheckPAC(AVictim victim)
        {
            if (victim.m_Respiration == 0 || victim.m_HeartRate == 0)
            {
                return EPACS.P0;
            }
            else if (victim.m_CanWalk)
            {
                return EPACS.P3;
            }
            else if (victim.m_Respiration >= 10 && victim.m_Respiration <= 30)
            {
                if (victim.m_HeartRate >= 70 && victim.m_HeartRate <= 120)
                {
                    return EPACS.P2;
                }
                else
                {
                    return EPACS.P1;
                }
            }
            else
            {
                return EPACS.P1;
            }
        }


        #region Scoring Shit

        public int CalcDamageScore(float initialHealth, float currentHealth)
        {
            if (Mathf.Abs(initialHealth) <= Mathf.Epsilon)
                return 0;

            float fDamagePc = ((initialHealth - currentHealth) / initialHealth) * 100.0f;
            int damagePc = (int)fDamagePc;

            if (damagePc >= 0 && damagePc <= 10)
                return 10;
            if (damagePc >= 11 && damagePc <= 20)
                return 9;
            if (damagePc >= 21 && damagePc <= 30)
                return 8;
            if (damagePc >= 31 && damagePc <= 40)
                return 7;
            if (damagePc >= 41 && damagePc <= 50)
                return 6;
            if (damagePc >= 51 && damagePc <= 60)
                return 5;
            if (damagePc >= 61 && damagePc <= 70)
                return 4;
            if (damagePc >= 71 && damagePc <= 80)
                return 3;
            if (damagePc >= 81 && damagePc <= 90)
                return 2;
            if (damagePc >= 91 && damagePc <= 98)
                return 1;

            return 0;
        }

        public int GetTreatmentTimeScore(float treatmentTime, EPACS pacs)
        {
            int avgTime;

            switch (pacs)
            {
                case EPACS.P3:
                    avgTime = Consts.TREATMENT_AVGTIME_P3;
                    break;

                case EPACS.P2:
                    avgTime = Consts.TREATMENT_AVGTIME_P3;
                    break;

                case EPACS.P1:
                    avgTime = Consts.TREATMENT_AVGTIME_P3;
                    break;

                default:
                    avgTime = 0;
                    break;
            }

            if (avgTime == 0)
                return 0;

            int time = (int)((treatmentTime / avgTime) * 100.0f);

            if (time >= 100)
                return 5;
            else if (time >= 101 && time <= 125)
                return 4;
            else if (time >= 126 && time <= 150)
                return 3;
            else if (time >= 151 && time <= 175)
                return 2;
            else if (time >= 176 && time <= 200)
                return 1;

            return 0;
        }

        /// <summary>
        /// Scoring for triage officer
        /// </summary>
        /// <param name="player"></param>
        /// <param name="victim"></param>
        /// <param name="triageTime"></param>
        public void ProcessTriageScoring(E3DPlayer player, AVictim victim, float triageTime)
        {
            int actionScore = 0;

            if (player != null && victim != null)
            {
                EPACS currentPAC = CheckPAC(victim);

                if (currentPAC == victim.m_State.m_GivenPACS)
                {
                    // correct triage for mission report
                    m_State.m_Triage_CorrectCount++;

                    // for personal player report
                    switch (victim.m_State.m_GivenPACS)
                    {
                        case EPACS.P3:
                            player.m_State.m_CorrectTriageP3Num++;
                            break;

                        case EPACS.P2:
                            player.m_State.m_CorrectTriageP2Num++;
                            break;

                        case EPACS.P1:
                            player.m_State.m_CorrectTriageP1Num++;
                            break;

                        case EPACS.P0:
                            player.m_State.m_CorrectTriageDeadNum++;
                            break;
                    }

                    actionScore = Consts.SCORE_TRIAGE_CORRECT;
                }
                else
                {
                    int correctPACScale = (int)currentPAC;
                    int assignedPACScale = (int)victim.m_State.m_GivenPACS;

                    if (assignedPACScale > correctPACScale)
                    {
                        // over triaged
                        m_State.m_Triage_OverCount++;
                        player.m_State.m_OverTriageNum++;

                        // check how much the was over triaged
                        if (assignedPACScale - correctPACScale == 1)
                            actionScore = Consts.SCORE_TRIAGE_OVER1;
                        else if (assignedPACScale - correctPACScale == 2)
                            actionScore = Consts.SCORE_TRIAGE_OVER2;
                    }
                    else if (assignedPACScale < correctPACScale)
                    {
                        // under triaged
                        m_State.m_Triage_UnderCount++;
                        player.m_State.m_UnderTriageNum++;

                        if (assignedPACScale - correctPACScale == -1)
                            actionScore = Consts.SCORE_TRIAGE_UNDER1;
                        else if (assignedPACScale - correctPACScale == -2)
                            actionScore = Consts.SCORE_TRIAGE_UNDER2;
                    }
                }

                // accumulate time
                player.m_State.m_TotalTriageTime += triageTime;

                float dmgScore = (CalcDamageScore(victim.m_StartHealth, victim.m_CurHealth));

                player.m_State.m_TriageActionScore += actionScore;
                player.m_State.m_TriageDmgScore += dmgScore;

                // do the tally
                m_State.m_TriagedCount += 1;
            }
        }

        public void ProcessTreatmentScoring(E3DPlayer player, AVictim victim, float treatmentDuration)
        {
            float actionScore = player.m_State.m_CorrectTreatmentNum / (float)player.m_State.m_TotalTreatmentNum;
            float dmgScore = CalcDamageScore(victim.m_StartHealth, victim.m_CurHealth);

            Debug.Log("Dmg Score after treatment per victim: " + dmgScore);

            player.m_State.m_TreatmentActionScore += actionScore;
            player.m_State.m_TreatmentDmgScore += dmgScore;

            //float timeScore = GetTreatmentTimeScore(dmgScore, victim.m_State.m_GivenPACS);
            //float totalTimeScore = m_VictimManager.NumVictims * Consts.SCORE_MAX_TIME;

            // accumulate time
            player.m_State.m_TotalTreatmentTime += treatmentDuration;

            // do the tally
            m_State.m_TreatedCount += 1;
        }

        public void ProcessMorgueScoring(E3DPlayer player, AVictim victim, float treatmentDuration)
        {
            float actionScore = player.m_State.m_CorrectTreatmentNum / (float)player.m_State.m_TotalTreatmentNum;
            float dmgScore;

            if (victim.m_StartHealth <= float.Epsilon)
                dmgScore = 10.0f;
            else
                dmgScore = CalcDamageScore(victim.m_StartHealth, victim.m_CurHealth);

            player.m_State.m_TreatmentActionScore += actionScore;
            player.m_State.m_TreatmentDmgScore += dmgScore;

            // accumulate time
            player.m_State.m_TotalTreatmentTime += treatmentDuration;

            // do the tally
            m_State.m_DeadCount += 1;
        }

        // edited by joshua
        public void ProcessEvacScoring(E3DPlayer player, AAmbulance ambulance)
        {
            AVictim victimInAmbulance = ambulance.Victim; // because there was only one

            // get the player role
            var evacOfficer = (AEmtEvacOffr)player.PossessedEmt;

            var victimList = evacOfficer.CurrentArea.GetVictims();
            int victimListLen = victimList.Length;

            Array.Resize(ref victimList, victimListLen + 1);

            victimListLen = victimList.Length;
            victimList[victimListLen - 1] = victimInAmbulance;

            int[] currentState = { 0, 0, 0, 0, 0, 0 }; //p3,p3,p2,p2js,p1,p1js
            string[] allType = { "p3", "p3js", "p2", "p2js", "p1", "p1js" };

            ArrayList currentArrayType = new ArrayList();

            for (int i = 0; i < victimListLen; i++)
            {
                var victim = victimList[i];
                if (victim.m_State.m_GivenPACS == EPACS.P3)
                {
                    if ((victim.m_Age < 16 || victim.m_IsPregnant))
                        currentState[1] = 1;
                    else
                        currentState[0] = 1;
                }
                else if (victim.m_State.m_GivenPACS == EPACS.P2)
                {
                    if ((victim.m_Age < 16 || victim.m_IsPregnant))
                        currentState[3] = 1;
                    else
                        currentState[2] = 1;
                }
                else if (victim.m_State.m_GivenPACS == EPACS.P1)
                {
                    if ((victim.m_Age < 16 || victim.m_IsPregnant))
                        currentState[4] = 1;
                    else
                        currentState[4] = 1;
                }
                else
                {
                    currentState[5] = 1;
                }
            }

            for (int i = 0; i < currentState.Length; i++)
            {
                if (currentState[i] == 1)
                {
                    currentArrayType.Add(allType[i]);
                }
            }

            // check victim's v value
            string victim_v_score = GetVictimV(victimInAmbulance);

            float evacActionScore = CalcEvacActionScore1(victim_v_score, currentArrayType);

            Debug.Log("Evac Act Score: " + evacActionScore);
            player.m_State.m_EvacActionScore += evacActionScore;

            float delay = CalcTrafficDelay(DateTime.Now);
            float travelTime = ambulance.CurrentRoute.m_TravelTime * (1 + delay);

            victimInAmbulance.m_State.m_AmbulanceTime = travelTime - victimInAmbulance.m_CurStableTime;

            float dmgScore = CalcDamageScore(victimInAmbulance.m_StartHealth, victimInAmbulance.m_CurHealth);
            player.m_State.m_EvacDmgScore += dmgScore;

            //victim.m_IsActive = false;
        }

        /// <summary>
        /// The V value represents the possible scores of all victim PAC when calculating actual score for evac.
        /// Don't ask why it's V its not vagina bro!! Don't think you can get some!
        /// </summary>
        /// <param name="victim"></param>
        /// <returns></returns>
        /// edited by joshua
        private string GetVictimV(AVictim victim)
        {
            if (victim.m_State.m_GivenPACS == EPACS.P3)
            {
                if ((victim.m_Age < 16 || victim.m_IsPregnant))
                    return "p3js";
                else
                    return "p3";
            }
            else if (victim.m_State.m_GivenPACS == EPACS.P2)
            {
                if ((victim.m_Age < 16 || victim.m_IsPregnant))
                    return "p2js";
                else
                    return "p2";
            }
            else if (victim.m_State.m_GivenPACS == EPACS.P1)
            {
                if ((victim.m_Age < 16 || victim.m_IsPregnant))
                    return "p1js";
                else
                    return "p1";
            }

            return "p0";
        }

        // edited by Joshua (entire calctrafficdelay function)
        private float CalcTrafficDelay(DateTime now)
        {
            float currentCongestionLevel;

            // based on real data of singapore traffic on 2019 (monday to friday)
            float[] congestionLevel = {
            2, 0, 0, 0, 0, 1, 20, 48, 59, 43, 30, 27, 26, 28, 30, 30, 31, 42, 58, 41, 24, 20, 15, 7,
            2, 0, 0, 0, 0, 2, 21, 48, 58, 43, 32, 29, 28, 30, 32, 31, 31, 42, 59, 42, 25, 22, 17, 9,
            3, 0, 0, 0, 0, 2, 20, 47, 59, 45, 32, 29, 28, 29, 32, 30, 30, 43, 60, 43, 26, 22, 17, 8,
            2, 0, 0, 0, 0, 2, 20, 45, 55, 43, 33, 30, 29, 30, 33, 32, 33, 45, 62, 45, 27, 24, 19, 9,
            3, 0, 0, 0, 0, 2, 19, 44, 52, 43, 34, 32, 32, 31, 37, 35, 36, 52, 69, 49, 29, 28, 26, 15,
            7, 2, 0, 0, 0, 1, 10, 14, 21, 28, 33, 37, 37, 37, 35, 34, 33, 33, 36, 33, 25, 25, 23, 14,
            6, 1, 0, 0, 0, 0, 6, 8, 13, 16, 20, 23, 24, 24, 22, 21, 21, 22, 22, 21, 20, 18, 13, 7 };

            // use system time
            int day;

            switch (now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    day = 0;
                    break;
                case DayOfWeek.Tuesday:
                    day = 1;
                    break;
                case DayOfWeek.Wednesday:
                    day = 2;
                    break;
                case DayOfWeek.Thursday:
                    day = 3;
                    break;
                case DayOfWeek.Friday:
                    day = 4;
                    break;
                case DayOfWeek.Saturday:
                    day = 5;
                    break;
                case DayOfWeek.Sunday:
                    day = 6;
                    break;
                default:
                    day = 0;
                    break;
            }

            int hour = now.Hour;
            int minute = now.Minute;
            int n = day * 24 + hour;

            currentCongestionLevel = ((minute / 60.0f) * (congestionLevel[(n + 1) % 168] - congestionLevel[n % 168]) + congestionLevel[n % 168]) / 100.0f;

            return currentCongestionLevel;
        }

        // edited by Joshua
        private float CalcEvacActionScore1(string v, ArrayList s)
        {
            int score = s.IndexOf(v) + 1;

            return (float)score / (float)s.Count;
        }

        #endregion

        // editor only
        private void OnValidate()
        {
            m_VictimManager = gameObject.GetOrAddComponent<VictimManager>();
            m_State = gameObject.GetOrAddComponent<GameState>();
        }
    }


    /// <summary>
    /// Instance wrapper for E3D_GameMode and its derived classes
    /// The implicit operators means that you do not need to explicitly set or get the Value property of MyProp, 
    /// but can write code to access the value in a more "natural" way.
    /// Reference: https://stackoverflow.com/questions/2587236/generic-property-in-c-sharp
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GameModeInstance<T> where T : GameMode
    {
        private T m_instance;

        public T Instance
        {
            get => m_instance;
            set => m_instance = value;
        }

        public static implicit operator T(GameModeInstance<T> value)
        {
            return value.Instance;
        }

        public static implicit operator GameModeInstance<T>(T value)
        {
            return new GameModeInstance<T> { Instance = value };
        }
    }
}
