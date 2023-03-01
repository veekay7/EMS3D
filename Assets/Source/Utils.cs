using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utils
{
    public static int GetEnumSize<T>() where T : Enum
    {
        int size = Enum.GetValues(typeof(T)).Cast<int>().Max();
        return size;
    }

    public static void SafeDestroyGameObject(GameObject gameObject)
    {
        if (gameObject != null)
        {
            GameObject.Destroy(gameObject);
            gameObject = null;
        }
    }

    public static void SafeDestroyGameObject(Component component)
    {
        if (component != null)
        {
            GameObject.Destroy(component.gameObject);
            component = null;
        }
    }

    /// <summary>
    /// Checks to see if a component exists or not for a target game object.
    /// If the specified component exists, the component attached to the game object is returned.
    /// Otherwise, the specified component will be added to the target game object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();

        return component;
    }

    /// <summary>
    /// Is the mouse pointer on top of a GUI object?
    /// </summary>
    /// <returns></returns>
    public static bool Input_IsPointerOnGUI()
    {
        if (EventSystem.current != null)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            EventSystem.current.RaycastAll(eventData, results);

            return (results.Count > 0);
        }

        return false;
    }

    /// <summary>
    /// Is the mouse pointer on a particular game object?
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Input_IsPointerOnThisGameObject(GameObject target)
    {
        if (EventSystem.current != null)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            EventSystem.current.RaycastAll(eventData, results);
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject == target)
                    return true;
            }
        }

        return false;
    }

    public static float Score_CalcTriageOfficerTotal(float actionScore, float dmgScore)
    {
        float r = ((Consts.SCORE_TRIAGE_WEIGHT * actionScore) + (1.0f - Consts.SCORE_TRIAGE_WEIGHT) * dmgScore) * 100.0f;
        return r;
    }

    public static float Score_CalcFAPDocTotal(float actionScore, float dmgScore)
    {
        float r = ((Consts.SCORE_TREATMENT_WEIGHT * actionScore) + (1.0f - Consts.SCORE_TREATMENT_WEIGHT) * dmgScore) * 100.0f;
        return r;
    }

    public static float Score_CalcEvacOfficerTotal(float actionScore, float dmgScore)
    {
        float r = ((Consts.SCORE_EVAC_WEIGHT * actionScore) + (1.0f - Consts.SCORE_EVAC_WEIGHT) * dmgScore) * 100.0f;
        return r;
    }

    /// <summary>
    /// Returns the distance between two GPS coordinates in degrees. The distance returned is in kilometers (km).
    /// The GPS is calculated with reference to the Earth's radius and not any other fucking planet.
    /// But seriously, I fucking hate Earth. Too many fucking humans. Fuck humans man!
    /// </summary>
    /// <param name="coord1"></param>
    /// <param name="coord2"></param>
    /// <returns></returns>
    public static float GetDistanceFromGPSCoords(Vector2 coord1, Vector2 coord2)
    {
        // Store the coord1 and coord2.
        Vector2 value1 = coord1;
        Vector2 value2 = coord2;

        // Convert value1 and value2 to radians, since GPS coordinates are in degrees.
        value1.x *= Mathf.Deg2Rad;
        value1.y *= Mathf.Deg2Rad;
        value2.x *= Mathf.Deg2Rad;
        value2.y *= Mathf.Deg2Rad;

        // Apply Haversine formula.
        float result;
        float deltaLong = value2.y - value1.y;
        float deltaLat = value2.x - value1.x;
        result =
            Mathf.Pow(
                Mathf.Sin(deltaLat / 2.0f), 2.0f) +
                Mathf.Cos(value1.x) * Mathf.Cos(value2.x) *
                Mathf.Pow(
                    Mathf.Sin(deltaLong / 2.0f), 2.0f);
        result = 2.0f * Mathf.Asin(Mathf.Sqrt(result));

        // Calculate the actual distance by applying earth radius (6371.0).
        result *= 6371.0f;

        return result;
    }
}
