using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryManager : MonoBehaviour
{
    public Image m_ImgBg;
    [ReorderableList]
    public List<Sprite> m_BgImages = new List<Sprite>();


    private void Awake()
    {
    }

    private IEnumerator Start()
    {
        ScreenWiper.Instance.SetFilled(true);

        yield return new WaitForSeconds(1.0f);

        // load a random background image and load it
        if (m_BgImages.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, m_BgImages.Count);

            Sprite selectedBg = m_BgImages[idx];

            m_ImgBg.color = Color.white;
            m_ImgBg.sprite = selectedBg;
        }

        // now unfade the thing
        ScreenWiper.Instance.DoFade(ScreenWiper.FillMode.Clear, 1.0f, 0.0f, () => {

            // now open the menu using the GUIController
            GUIScreen mainMenu = GUIController.Instance.m_CachedScreens[Consts.MAIN_MENU];
            GUIController.Instance.OpenScreen(mainMenu);

        });
    }
}
