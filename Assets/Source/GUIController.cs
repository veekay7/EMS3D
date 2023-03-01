using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIController : SingletonBehaviour<GUIController>
{
	[HideInInspector]
	public RectTransform m_RectTransform;
	[HideInInspector]
	public Canvas m_Canvas;

	public GUIScreen m_StartScreen;
	public List<GUIScreen> m_CachedScreens = new List<GUIScreen>();

	private bool m_screenIsChanging;
	private GameObject m_prevSelectable;


	public GUIScreen CurrentActiveScreen
    {
		get; private set;
    }

    protected override void AfterAwake()
    {
        base.AfterAwake();

        m_screenIsChanging = false;
		m_prevSelectable = null;

		DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
		if (m_StartScreen != null)
			OpenScreen(m_StartScreen);
    }

    private void Update()
    {
        if (!m_screenIsChanging)
        {
			if (Input.GetKeyDown(KeyCode.Escape))
            {
				// TODO: go back to previous screen, or cancel certain tabbed screen, or something IDK
            }
        }
    }

	public void OpenScreen(GUIScreen newScreen)
    {
		// if the screen we want to open is already open, don't bother
		if (CurrentActiveScreen == newScreen)
			return;

		m_screenIsChanging = true;

		// new screen has not been opened, activate it
		newScreen.gameObject.SetActive(true);
		GameObject newPrevSelectable = EventSystem.current.currentSelectedGameObject;

		// put this new screen in front of everything else
		newScreen.transform.SetAsLastSibling();

		// close the current screen and remove current screen from the stack
		CloseCurrentScreen();

		// set the previous Selectable object
		m_prevSelectable = newPrevSelectable;

		// now open the new screen
		CurrentActiveScreen = newScreen;

		newScreen.Open(() => 
		{ 
			m_screenIsChanging = false; 
		});

		GameObject newScreenFirstSelectable = FindFirstEnabledSelectable(newScreen.gameObject);
		EventSystem.current.SetSelectedGameObject(newScreenFirstSelectable);
	}

	public void CloseCurrentScreen()
    {
		if (CurrentActiveScreen == null)
			return;

		m_screenIsChanging = true;

		EventSystem.current.SetSelectedGameObject(m_prevSelectable);

		GUIScreen cur = CurrentActiveScreen;

		cur.Close(() => 
		{
			cur.gameObject.SetActive(false); 
		});
		
		CurrentActiveScreen = null;
	}

	private GameObject FindFirstEnabledSelectable(GameObject screen)
    {
		GameObject found = null;

		var selectables = screen.GetComponentsInChildren<Selectable>(true);
		foreach (var selectable in selectables)
		{
			if (selectable.IsActive() && selectable.IsInteractable())
			{
				found = selectable.gameObject;
				break;
			}
		}

		return found;
	}

    private void OnValidate()
    {
		m_RectTransform = gameObject.GetOrAddComponent<RectTransform>();
		m_Canvas = gameObject.GetOrAddComponent<Canvas>();
    }
}
