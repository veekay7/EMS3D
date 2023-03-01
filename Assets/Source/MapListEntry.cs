using UnityEngine;
using UnityEngine.Video;

public class MapListEntry : ScriptableObject
{
    public string m_SceneFileName;
    public Sprite m_Thumbnail;
    public string m_DisplayName;
    public VideoClip m_SceneVideoClip;
    public TextAsset m_ObjectivesDesc;
}
