using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : SingletonBehaviour<SoundSystem>
{
    public const int AUDIO_SOURCE_BGM = 0;
    public const int AUDIO_SOURCE_SFX = 1;

    [HideInInspector]
    public AudioSource[] m_AudioSources;


    protected override void AfterAwake()
    {
        m_AudioSources = GetComponents<AudioSource>();
    }

    public void PlaySoundFx(AudioClip clip)
    {
        m_AudioSources[AUDIO_SOURCE_SFX].PlayOneShot(clip);
    }

    public void StopSoundFx()
    {
        m_AudioSources[AUDIO_SOURCE_SFX].Stop();
    }

    public void PlayMusic(AudioClip clip)
    {
        m_AudioSources[AUDIO_SOURCE_BGM].clip = clip;
        m_AudioSources[AUDIO_SOURCE_BGM].Play();
    }

    public void StopMusic()
    {
        m_AudioSources[AUDIO_SOURCE_BGM].Stop();
        m_AudioSources[AUDIO_SOURCE_BGM].clip = null;
    }
}
