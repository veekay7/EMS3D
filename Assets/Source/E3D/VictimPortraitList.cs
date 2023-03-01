using System.Collections.Generic;
using UnityEngine;

public class VictimPortraitList : ScriptableObject
{
    public Sprite m_NullMaleSprite;
    public Sprite m_NullFemaleSprite;

    public List<Sprite> m_Female_JS = new List<Sprite>();
    public List<Sprite> m_Female_JCJK = new List<Sprite>();
    public List<Sprite> m_Female_Adult = new List<Sprite>();
    public List<Sprite> m_Female_Elderly = new List<Sprite>();

    public List<Sprite> m_Male_JS = new List<Sprite>();
    public List<Sprite> m_Male_JCJK = new List<Sprite>();
    public List<Sprite> m_Male_Adult = new List<Sprite>();
    public List<Sprite> m_Male_Elderly = new List<Sprite>();


    /// <summary>
    /// Note that age range cannot be larger than 5.
    /// </summary>
    /// <returns></returns>
    public Sprite GetFemalePortraitByAgeRange(int age)
    {
        int randIdx = -1;
        if (age >= 5 && age <= 15)
        {
            randIdx = UnityEngine.Random.Range(0, m_Female_JS.Count - 1);
            return m_Female_JS[randIdx];
        }
        else if (age >= 16 && age <= 25)
        {
            randIdx = UnityEngine.Random.Range(0, m_Female_JCJK.Count - 1);
            return m_Female_JCJK[randIdx];
        }
        else if (age >= 26 && age <= 35)
        {
            randIdx = UnityEngine.Random.Range(0, m_Female_JCJK.Count - 1);
            return m_Female_JCJK[randIdx];
        }
        else if (age >= 36 && age <= 45)
        {
            randIdx = UnityEngine.Random.Range(0, m_Female_Adult.Count - 1);
            return m_Female_Adult[randIdx];
        }
        else if (age >= 46 && age <= 55)
        {
            randIdx = UnityEngine.Random.Range(0, m_Female_Adult.Count - 1);
            return m_Female_Adult[randIdx];
        }

        return m_NullFemaleSprite;
    }

    public Sprite GetMalePortraitByAgeRange(int age)
    {
        int randIdx = -1;
        if (age >= 5 && age <= 15)
        {
            randIdx = UnityEngine.Random.Range(0, m_Male_JS.Count - 1);
            return m_Male_JS[randIdx];
        }
        else if (age >= 16 && age <= 25)
        {
            randIdx = UnityEngine.Random.Range(0, m_Male_JCJK.Count - 1);
            return m_Male_JCJK[randIdx];
        }
        else if (age >= 26 && age <= 35)
        {
            randIdx = UnityEngine.Random.Range(0, m_Male_JCJK.Count - 1);
            return m_Male_JCJK[randIdx];
        }
        else if (age >= 36 && age <= 45)
        {
            randIdx = UnityEngine.Random.Range(0, m_Male_Adult.Count - 1);
            return m_Male_Adult[randIdx];
        }
        else if (age >= 46 && age <= 55)
        {
            randIdx = UnityEngine.Random.Range(0, m_Male_Adult.Count - 1);
            return m_Male_Adult[randIdx];
        }

        return m_NullMaleSprite;
    }
}
