using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class PlayButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerUpHandler
{
    [HideInInspector]
    public Button m_ButtonComponent;

    public AudioClip m_ClickSound;
    public AudioClip m_DenySound;
    public AudioClip m_ReleaseSound;
    public AudioClip m_RollOverSound;


    private void Awake()
    {
        m_ButtonComponent = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_ButtonComponent.interactable)
            PlaySound(m_ClickSound);
        else
            PlaySound(m_DenySound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlaySound(m_ClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(m_RollOverSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlaySound(m_ReleaseSound);
    }

    public void PlaySound(AudioClip clip)
    {
        if (SoundSystem.Instance == null)
            return;

        SoundSystem.Instance.PlaySoundFx(clip);
    }

    private void OnValidate()
    {
        m_ButtonComponent = GetComponent<Button>();
    }
}
