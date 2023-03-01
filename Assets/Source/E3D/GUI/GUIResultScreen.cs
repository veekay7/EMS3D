using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace E3D
{
    public class GUIResultScreen : GUIScreen
    {
        [Header("Result Screen")]
        public GameObject m_MissionReportPanel;
        public GameObject m_PersonalReportPanel;

        [Space]

        public Toggle m_ChkboxMission;
        public Toggle m_ChkboxPersonal;

        [Space]

        public TMP_Text m_TxtMissionReport;
        public TMP_Text m_TxtPersonalReport;
        public TMP_Text m_TxtRank;


        protected override void Start()
        {
            base.Start();

            m_MissionReportPanel.SetActive(false);

            m_ChkboxMission.gameObject.SetActive(false);

            // because this is singleplayer, so personal checkbox must be set isOn to true
            m_ChkboxMission.isOn = false;
            m_ChkboxPersonal.isOn = true;

            SetMissionReportVisible(m_ChkboxMission.isOn);
            SetPersonalReportVisible(m_ChkboxPersonal.isOn);
        }

        public void SetMissionReportVisible(bool value)
        {
            m_MissionReportPanel.SetActive(value);
        }

        public void SetPersonalReportVisible(bool value)
        {
            m_PersonalReportPanel.SetActive(value);
        }

        public override void Open(UnityAction onFinishAnim = null)
        {
            // print the reports
            PrintMissionReport();
            PrintPersonalReport();

            base.Open(onFinishAnim);
        }

        public void CloseAndReturnToMainMenu()
        {
            //if (ScreenWiper.Instance != null)
            //    ScreenWiper.Instance.SetFilled(true);

            //SceneManager.LoadScene("gamemenu");

            if (E3DPlayer.Local != null)
                E3DPlayer.Local.Disconnect();
        }

        protected void PrintMissionReport()
        {
            E3DPlayer player = E3DPlayer.Local;
            GameState gameState = GameState.Current;

            m_TxtMissionReport.text = "Not available in singleplayer";
        }

        protected void PrintPersonalReport()
        {
            E3DPlayer player = E3DPlayer.Local;

            if (player != null)
            {
                if (player.PossessedEmt is AEmtTriageOffr)
                    PrintTriageOfficerReport(player);

                if (player.PossessedEmt is AEmtFirstAidDoc)
                    PrintFirstAidDocReport(player);

                if (player.PossessedEmt is AEmtEvacOffr)
                    PrintEvacOfficerReport(player);
            }
        }

        private void PrintTriageOfficerReport(E3DPlayer player)
        {
            AEmtTriageOffr role = (AEmtTriageOffr)player.PossessedEmt;
            E3DPlayerState playerState = player.m_State;

            int totalCorrectNum = playerState.m_CorrectTriageP3Num + playerState.m_CorrectTriageP2Num + playerState.m_CorrectTriageP1Num + playerState.m_CorrectTriageDeadNum;

            int totalNum = totalCorrectNum + playerState.m_UnderTriageNum + playerState.m_OverTriageNum;

            float correctPc = (totalCorrectNum / (float)totalNum) * 100.0f;

            float underTriagePc = (playerState.m_UnderTriageNum / (float)totalNum) * 100.0f;

            float overTriagePc = (playerState.m_OverTriageNum / (float)totalNum) * 100.0f;

            float avgTime = playerState.m_TotalTriageTime / totalNum;

            float maxActionScore = totalNum * Consts.SCORE_TRIAGE_CORRECT;

            float maxDmgScore = totalNum * Consts.SCORE_TRIAGE_DAMAGE_RANGE0;

            string reportString = string.Format("<b>Subject:</b> {0}\n" +
                "<b>Role:</b> Triage Officer\n\n" +
                "<b>Correct Triage (%): </b> {1}\n" +
                "<indent=15%>P3: {2}</indent>\n" +
                "<indent=15%>P2: {3}</indent>\n" +
                "<indent=15%>P1: {4}</indent>\n" +
                "<indent=15%>Dead: {5}</indent>\n\n" +
                "<b>Under Triage (%):</b> {6}\n\n" +
                "<b>Over Triage (%):</b> {7}\n\n" +
                "<b>Average Decision Time (secs):</b> {8}\n",
                playerState.Player.m_DisplayName,
                (int)correctPc, playerState.m_CorrectTriageP3Num, playerState.m_CorrectTriageP2Num, playerState.m_CorrectTriageP1Num, playerState.m_CorrectTriageDeadNum,
                (int)underTriagePc, (int)overTriagePc, avgTime);

            m_TxtPersonalReport.text = reportString;
            //m_TxtRank.text = playerState.m_TriageDmgScore.ToString();
            //m_TxtRank.text = Utils.Score_CalcTriageOfficerTotal(playerState.m_TriageActionScore / maxActionScore, playerState.m_TriageDmgScore / maxDmgScore).ToString();

            Debug.Log("Total act score: " + playerState.m_TriageActionScore + " / " + maxActionScore);
            Debug.Log("Total dmg score: " + playerState.m_TriageDmgScore + " / " + maxDmgScore);

            int score = (int)Utils.Score_CalcTriageOfficerTotal(playerState.m_TriageActionScore / maxActionScore, playerState.m_TriageDmgScore / maxDmgScore);

            m_TxtRank.text = CalcRank(score) + " " + "(" + score.ToString() + " / 100)";
        }

        private void PrintFirstAidDocReport(E3DPlayer player)
        {
            AEmtFirstAidDoc role = (AEmtFirstAidDoc)player.PossessedEmt;
            E3DPlayerState playerState = player.m_State;

            int totalNum = playerState.m_TotalVictimsAttendedNum;

            float maxDmgScore = totalNum * 10;      // TODO: put this as constant???

            float actionScore = playerState.m_TreatmentActionScore / (float)totalNum;

            float avgTime = playerState.m_TotalTreatmentTime / (float)totalNum;

            float dmgScore = playerState.m_TreatmentDmgScore / maxDmgScore;

            float accuracy = actionScore * 100.0f;

            string reportString = string.Format("<b>Subject:</b> {0}\n" +
                "<b>Role:</b> First Aid Point Officer\n\n" +
                "<b>Total victims attended to:</b> {1}\n" +
                "<b>Average treatment time per victim (secs):</b> {2}\n" +
                "<b>Treatment accuracy:</b> {3} %\n", //+
                //"<b>Victims dead under treatment:</b> {4}\n",
                player.m_DisplayName, totalNum, avgTime, accuracy.ToString("0.00"), "?");

            //Debug.Log("Action Score: " + actionScore + ", Dmg Score: " + playerState.m_TreatmentDmgScore);

            m_TxtPersonalReport.text = reportString;

            int score = (int)Utils.Score_CalcFAPDocTotal(actionScore, dmgScore);
            m_TxtRank.text = CalcRank(score) + " " + "(" + score.ToString() + " / 100)";
        }

        private void PrintEvacOfficerReport(E3DPlayer player)
        {
            AEmtEvacOffr role = (AEmtEvacOffr)player.PossessedEmt;
            E3DPlayerState playerState = player.m_State;

            int totalNum = playerState.m_TotalVictimsAttendedNum;

            float maxDmgScore = totalNum * 10;      // NOTE: put this as constant???

            float actionScore = playerState.m_EvacActionScore / (float)totalNum;

            //float avgTime = playerState.m_TotalTreatmentTime / (float)totalNum;

            float dmgScore = playerState.m_EvacDmgScore / maxDmgScore;

            float accuracy = actionScore * 100.0f;

            //Debug.Log("Action Score: " + actionScore + " Dmg Score: " + dmgScore);

            string reportString = string.Format("<b>Subject: {0}\n" +
               "Role: Evacuation Officer</b>\n\n" +
                "<b>Evacuated victims:</b> {1}\n",
               player.m_DisplayName, totalNum);

            m_TxtPersonalReport.text = reportString;

            int score = (int)Utils.Score_CalcEvacOfficerTotal(actionScore, dmgScore);

            m_TxtRank.text = CalcRank(score) + " " + "(" + score.ToString() + " / 100)";
        }

        public string CalcRank(int score)
        {
            if (score >= 95)
            {
                return "A+";
            }
            else if (score >= 90)
            {
                return "A";
            }
            else if (score >= 85)
            {
                return "A-";
            }
            else if (score >= 80)
            {
                return "B+";
            }
            else if (score >= 75)
            {
                return "B";
            }
            else if (score >= 70)
            {
                return "B-";
            }
            else if (score >= 65)
            {
                return "C";
            }
            else if (score >= 60)
            {
                return "D";
            }
            else if (score >= 55)
            {
                return "E";
            }
            else
            {
                return "F";
            }
        }
    }
}
