using System.Collections.Generic;
using UnityEngine;
using URand = UnityEngine.Random;

namespace E3D
{
    public class VictimManager : MonoBehaviour
    {
        public GameObject m_VictimPrefab;
        public VictimPortraitList m_PortraitList;

        private bool m_gameStarted;
        private int m_totalPACProbability;
        private float m_startTime;
        private float m_timeToNextInjury;
        public List<AVictim> m_victims = new List<AVictim>();

        public static VictimManager Current { get; private set; }

        public bool FinishedCreatingVictims { get; private set; }

        public bool ShouldUpdate { get; set; }

        public int NumVictims { get => m_victims.Count; }


        private void Awake()
        {
            if (Current != null && Current == this)
                return;

            m_totalPACProbability = 0;
            m_startTime = 0.0f;
            m_timeToNextInjury = 0.0f;

            FinishedCreatingVictims = false;
            ShouldUpdate = false;

            //DevCmdSys.Instance.AddCommand(new ConCommand("killall", "Kills all victims in the game.", (cvar, p) => KillAll()));
            //DevCmdSys.Instance.AddCommand(new ConCommand("triageall", "Triages all the victims correctly.", (cvar, p) => TriageAllCorrectly()));
            //DevCmdSys.Instance.AddCommand(new ConCommand("treatall", "Treats all victims in the game.", (cvar, p) => TreatAll()));

            Current = this;
        }

        private void Start()
        {
            m_timeToNextInjury = Time.time + 3.0f;
        }

        public void StartUpdate()
        {
            if (m_gameStarted)
                return;

            m_startTime = Time.time;
            m_gameStarted = true;
            ShouldUpdate = true;
        }

        private void Update()
        {
            if (!ShouldUpdate)
                return;

            float curTime = Time.time;
            float timeDiff = curTime - m_startTime;


            if (curTime >= m_timeToNextInjury)
            {
                for (int i = 0; i < m_victims.Count; i++)
                {
                    AVictim v = m_victims[i];

                    if (v.m_IsActive && v.IsAlive)
                    {
                        if (v.m_State.m_IsTreated)
                        {
                            // countdown stable time all the way to 0
                            v.m_CurStableTime -= Time.deltaTime;

                            if (v.m_CurStableTime <= 0.0f)
                            {
                                v.m_CurStableTime = 0.0f;
                                v.UpdateHealthAndVitals(timeDiff + v.m_State.m_AmbulanceTime);
                            }
                        }
                        else
                        {
                            v.UpdateHealthAndVitals(timeDiff + v.m_State.m_AmbulanceTime);
                        }
                    }
                }

                m_timeToNextInjury = curTime + 1.0f;
            }

            /*for (int i = 0; i < m_victims.Count; i++)
            {
                AVictim v = m_victims[i];

                if (v.m_IsActive && v.IsAlive)
                {
                    if (v.m_State.m_IsTreated)
                    {
                        // countdown stable time all the way to 0
                        v.m_CurStableTime -= Time.deltaTime;
                        
                        if (v.m_CurStableTime <= 0.0f)
                        {
                            v.m_CurStableTime = 0.0f;
                            v.UpdateHealthAndVitals(timeDiff + v.m_State.m_AmbulanceTime);
                        }
                    }
                    else
                    {
                        // means they still have injury tags, then we deteriorate them based on time to next injury
                        if (curTime >= m_timeToNextInjury)
                        {
                            v.UpdateHealthAndVitals(timeDiff + v.m_State.m_AmbulanceTime);
                            m_timeToNextInjury = curTime + 1.0f;
                        }
                    }
                }
            }*/
        }

        public void CreateVictims(int count)
        {
            if (!FinishedCreatingVictims)
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject newVictimObject = Instantiate(m_VictimPrefab);
                    AVictim victim = newVictimObject.GetComponent<AVictim>();

                    if (!m_victims.Contains(victim))
                        m_victims.Add(victim);
                }

                Shuffle();
                FinishedCreatingVictims = true;
            }
        }

        public void InitInfoToVictims(int prob_p3, int prob_p2, int prob_p1, int prob_p0, int ageGap)
        {
            for (int i = 0; i < NumVictims; i++)
            {
                int id;
                int age, age_lo, age_hi;
                ESex sex;
                EPACS pacs;

                AVictim victim = m_victims[i];

                // set up profile of the victim
                id = i;
                sex = AssignRandomSex();
                AssignAgeGroup(ageGap, out age, out age_lo, out age_hi);

                // get a random injury from a random pac tag
                pacs = GetRandomPACSTag(prob_p3, prob_p2, prob_p1, prob_p0);

                Sprite portraitSprite = null;
                if (sex == ESex.Female)
                {
                    portraitSprite = m_PortraitList.m_NullFemaleSprite;
                    portraitSprite = m_PortraitList.GetFemalePortraitByAgeRange(age);
                }
                else
                {
                    portraitSprite = m_PortraitList.m_NullMaleSprite;
                    portraitSprite = m_PortraitList.GetMalePortraitByAgeRange(age);
                }

                victim.Init(id, sex, age, age_lo, age_hi, portraitSprite, pacs);

                // set injury here bish
                Injury injury = InjuryList.FindByPACTag(pacs);
                if (injury != null)
                {
                    victim.SetInjury(injury);
                }
            }
        }

        public AVictim[] GetVictims()
        {
            return m_victims.ToArray();
        }

        public void Shuffle()
        {
            if (m_victims.Count == 0)
                return;

            int n = m_victims.Count;
            for (int i = 0; i < (n - 1); i++)
            {
                // Use Next on random instance with an argument.
                // ... The argument is an exclusive bound.
                //     So we will not go past the end of the array.
                int r = URand.Range(i, n - i);
                AVictim t = m_victims[r];

                m_victims[r] = m_victims[i];
                m_victims[i] = t;
            }
        }

        public void TriageAllCorrectly()
        {
            for (int i = 0; i < NumVictims; i++)
            {
                m_victims[i].m_State.m_CheckedCanWalk = true;
                m_victims[i].m_State.m_CheckedHeartRate = true;
                m_victims[i].m_State.m_CheckedBloodPressure = true;
                m_victims[i].m_State.m_CheckedRespiration = true;
                m_victims[i].m_State.m_CheckedSpO2 = true;
                m_victims[i].m_State.m_CheckedGCS = true;
                m_victims[i].m_State.m_GivenPACS = m_victims[i].m_PACS;
                m_victims[i].m_State.m_IsTriaged = true;
            }

            DevCmdSys.Echo("Triaged all victims correctly!");
        }

        public void TreatAll()
        {
            for (int i = 0; i < NumVictims; i++)
            {
                //m_victims[i].RemoveAllTreatmentTagsFromInjuries();
            }

            DevCmdSys.Echo("Treated all victims correctly!");
        }

        public void KillAll()
        {
            for (int i = 0; i < NumVictims; i++)
            {
                if (m_victims[i].IsAlive)
                {
                    float removeAmount = m_victims[i].m_StartHealth - m_victims[i].m_CurHealth;

                    m_victims[i].RemoveHealth(removeAmount);
                    m_victims[i].SetVitalsByPAC(EPACS.P0);

                    GameMode.Current.m_State.m_DeadCount++;
                }
            }

            DevCmdSys.Echo("All victims have been killed!");
        }

        private ESex AssignRandomSex()
        {
            int iRand = URand.Range(0, 2);
            return (iRand == 0) ? ESex.Female : ESex.Male;
        }

        private void AssignAgeGroup(int ageGap, out int age, out int lower, out int upper)
        {
            age = URand.Range(1, 60);
            lower = (int)((age / ageGap) * ageGap + 1);
            upper = (int)(((age / ageGap) + 1) * ageGap);
        }

        private EPACS GetRandomPACSTag(int prob_p3, int prob_p2, int prob_p1, int prob_p0)
        {
            m_totalPACProbability = prob_p3 + prob_p2 + prob_p1 + prob_p0;
            int x = URand.Range(0, m_totalPACProbability);
            if ((x -= prob_p1) < 0)
            {
                return EPACS.P1;
            }
            else if ((x -= prob_p2) < 0)
            {
                return EPACS.P2;
            }
            else if ((x -= prob_p3) < 0)
            {
                return EPACS.P3;
            }

            return EPACS.P0;
        }
    }
}
