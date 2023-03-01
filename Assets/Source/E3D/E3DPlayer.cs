using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace E3D
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(E3DPlayerState))]
    public class E3DPlayer : MonoBehaviour
    {
        public static KeyCode[] AlphaNumKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

        [HideInInspector]
        public E3DPlayerState m_State;

        [Header("Prefabs")]
        public GameObject m_DevConsoleWndPrefab;
        public GameObject m_LarryModelPrefab;
        public GameObject m_BlackoutPrefab;
        [ReorderableList]
        public List<GameObject> m_HudPrefabs = new List<GameObject>();

        public string m_DisplayName;
        public bool m_ReadyToPlay;
        public bool m_EnableInput;

        private GameCamera m_camera;
        private AEmtBase m_possessedEmt;
        private GUIController m_gameUI;
        private Hud m_hud;
        private E3DLarryModel m_larryModel;
        private Canvas m_blackout;

        private UnityAction m_enableInputFunc;
        private UnityAction m_disableInputFunc;


        public static E3DPlayer Local { get; private set; }

        public E3DLarryModel LarryModel { get => m_larryModel; }

        public Canvas Blackout { get => m_blackout; }

        public GameCamera CurrentCamera { get => m_camera; }

        public AEmtBase PossessedEmt { get => m_possessedEmt; }

        public bool IsReady { get => PossessedEmt != null && m_ReadyToPlay; }

        public bool CanMove { get; set; }

        public bool IsWaitingForPrompt { get; set; }

        private void Awake()
        {
            m_State = GetComponent<E3DPlayerState>();

            m_camera = null;
            m_possessedEmt = null;
            m_gameUI = null;
            m_hud = null;
            m_larryModel = null;
            m_blackout = null;
            CanMove = false;

            m_enableInputFunc = null;
            m_disableInputFunc = null;

            m_State.SetPlayer(this);
        }

        private void Reset()
        {
            m_DisplayName = "unnamed";
            m_ReadyToPlay = false;
            m_EnableInput = false;
        }

        private void Start()
        {
            m_camera = GameCamera.Current;
            m_gameUI = GUIController.Instance;

            //DevCmdSys.onOpenedFunc += m_disableInputFunc;
            //DevCmdSys.onClosedFunc += m_enableInputFunc;

            m_enableInputFunc = () => EnableInput(true);
            m_disableInputFunc = () => EnableInput(false);

            Local = this;

            var classSelectScreen = GUIController.Instance.m_CachedScreens[Consts.CLASS_SELECT_MENU];
            GUIController.Instance.OpenScreen(classSelectScreen);
        }

        private void Update()
        {
            if (m_EnableInput)
            {
                if (!IsWaitingForPrompt)
                {
                    /* mouse wheel */
                    float scrolledAmount = !Utils.Input_IsPointerOnGUI() ? (-Input.mouseScrollDelta.y * Mathf.Abs((float)Globals.m_UserGameConfig.m_ScrollSensitivity)) : 0.0f;
                    if (Mathf.Abs(scrolledAmount) > 0.0f)
                    {
                    }

                    /* basic player input */
                    if (Input.GetKeyDown(KeyCode.Tilde))
                    {
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Pause();
                    }
                    else if (Input.GetKeyDown(KeyCode.B))
                    {
                        OpenBriefingScreen();
                    }

                    /* movement input */
                    if (CanMove)
                    {
                        float moveSpd = 3.0f;
                        if (Input.GetKey(KeyCode.W))
                        {
                            m_camera.transform.position += (transform.forward * moveSpd) * Time.deltaTime;
                        }
                        else if (Input.GetKey(KeyCode.S))
                        {
                            m_camera.transform.position -= (transform.forward * moveSpd) * Time.deltaTime;
                        }

                        if (Input.GetKey(KeyCode.A))
                        {
                            m_camera.transform.position -= (transform.right * moveSpd) * Time.deltaTime;
                        }
                        else if (Input.GetKey(KeyCode.D))
                        {
                            m_camera.transform.position += (transform.right * moveSpd) * Time.deltaTime;
                        }
                    }
                }
            }
        }

        public void EnableInput(bool value)
        {
            m_EnableInput = value;
        }

        public void Possess(AEmtBase emt)
        {
            if (m_possessedEmt == null)
            {
                m_possessedEmt = emt;

                m_possessedEmt.Possess(this);

                if (m_possessedEmt is AEmtTriageOffr)
                {
                    m_hud = CreateHud<HudTriageOffr>();

                    if (GameState.Current != null)
                    {
                        var faps = GameState.Current.m_FirstAidPoints.ToArray();
                        var evacPoints = GameState.Current.m_EvacPoints.ToArray();
                        for (int i = 0; i < faps.Length; i++)
                        {
                            faps[i].SetVisible(false);
                        }
                        for (int i = 0; i < evacPoints.Length; i++)
                        {
                            evacPoints[i].SetVisible(false);
                        }
                    }
                }
                else if (m_possessedEmt is AEmtFirstAidDoc)
                {
                    m_hud = CreateHud<HudFirstAidDoc>();
                    
                    GameObject larryModelObject = Instantiate(m_LarryModelPrefab);
                    m_larryModel = larryModelObject.GetComponent<E3DLarryModel>();

                    GameObject blackoutObject = Instantiate(m_BlackoutPrefab);
                    m_blackout = blackoutObject.GetComponent<Canvas>();

                    m_larryModel.SetActive(false);
                    
                    m_blackout.worldCamera = m_camera.m_UICam;
                    m_blackout.gameObject.SetActive(false);

                    if (GameState.Current != null)
                    {
                        var casualtyPoints = GameState.Current.m_CasualtyPoints.ToArray();
                        var evacPoints = GameState.Current.m_EvacPoints.ToArray();
                        for (int i = 0; i < casualtyPoints.Length; i++)
                        {
                            casualtyPoints[i].SetVisible(false);
                        }
                        for (int i = 0; i < evacPoints.Length; i++)
                        {
                            evacPoints[i].SetVisible(false);
                        }
                    }
                }
                else if (m_possessedEmt is AEmtEvacOffr)
                {
                    m_hud = CreateHud<HudEvacOffr>();

                    if (GameState.Current != null)
                    {
                        var casualtyPoints = GameState.Current.m_CasualtyPoints.ToArray();
                        var faps = GameState.Current.m_FirstAidPoints.ToArray();
                        for (int i = 0; i < faps.Length; i++)
                        {
                            faps[i].SetVisible(false);
                        }
                        for (int i = 0; i < casualtyPoints.Length; i++)
                        {
                            casualtyPoints[i].SetVisible(false);
                        }
                    }
                }

                if (m_hud != null)
                {
                    m_hud.SetPlayer(this);
                    //m_hud.SetVisible(false);
                }
            }
        }

        public void UnPossess()
        {
            if (m_possessedEmt != null)
            {
                m_possessedEmt.UnPossess(this);
                m_possessedEmt = null;

                if (m_hud != null)
                {
                    m_hud.UnsetPlayer();
                    Destroy(m_hud);
                }
            }
        }

        public void GameOver()
        {
            CanMove = false;
            EnableInput(false);

            m_hud.SetVisible(false);

            GUIScreen resultScreen = m_gameUI.m_CachedScreens[Consts.RESULT_SCREEN];
            m_gameUI.OpenScreen(resultScreen);
        }

        public void SetReady()
        {
            if (!m_ReadyToPlay)
            {
                m_ReadyToPlay = true;
                CanMove = true;
            }
        }

        public void Pause()
        {
            GUIScreen pauseScreen = m_gameUI.m_CachedScreens[Consts.PAUSE_SCREEN];
            m_gameUI.OpenScreen(pauseScreen);

            EnableInput(false);
        }

        public void UnPause()
        {
            if (m_gameUI.CurrentActiveScreen != null && GUIController.Instance.CurrentActiveScreen.m_Classname == "Pause")
                m_gameUI.CloseCurrentScreen();

            EnableInput(true);
        }

        public void Disconnect()
        {
            EnableInput(false);

            if (GameSystem.Instance != null)
                GameSystem.Instance.DisconnectPlayer(this);
        }

        public void OpenBriefingScreen()
        {
            GUIScreen briefingScreen = m_gameUI.m_CachedScreens[Consts.BRIEFING_SCREEN];
            m_gameUI.OpenScreen(briefingScreen);

            EnableInput(false);
        }

        public void CloseBriefingScreen()
        {
            if (m_gameUI.CurrentActiveScreen != null && GUIController.Instance.CurrentActiveScreen.m_Classname == "Briefing")
                m_gameUI.CloseCurrentScreen();

            EnableInput(true);
        }

        public void Cleanup()
        {
            Utils.SafeDestroyGameObject(m_hud);
            Utils.SafeDestroyGameObject(m_larryModel);
            Utils.SafeDestroyGameObject(m_blackout);
        }
        
        private T CreateHud<T>() where T : Hud
        {
            T hud = null;

            for (int i = 0; i < m_HudPrefabs.Count; i++)
            {
                if (m_HudPrefabs[i].GetComponent<T>())
                {
                    GameObject newHudObject = Instantiate(m_HudPrefabs[i]);
                    hud = newHudObject.GetComponent<T>();

                    break;
                }
            }

            return hud;
        }

        private void OnDestroy()
        {
            if (m_hud != null)
                m_hud.UnsetPlayer();

            UnPossess();
            Cleanup();
        }

        // editor only
        private void OnValidate()
        {
            if (m_State == null)
            {
                m_State = GetComponent<E3DPlayerState>();
                if (m_State == null)
                    m_State = gameObject.AddComponent<E3DPlayerState>();
            }
        }
    }
}
