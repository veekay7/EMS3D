using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace E3D
{
    /// <summary>
    /// Route controller
    /// </summary>
    public class ARouteController : MonoBehaviour
    {
        // all routes are calculated from incident scene to destination
        [ReorderableList]
        public List<Route> m_RoutesList = new List<Route>();

        private Dictionary<string, Route> m_routes = new Dictionary<string, Route>();


        private IEnumerator Start()
        {
            for (int i = 0; i < m_RoutesList.Count; i++)
            {
                var r = m_RoutesList[i];
                if (m_routes.ContainsKey(r.m_Key))
                {
                    //DevCon.Instance.Log("A route with key " + r.m_Key + " already exists, ignoring this route");
                    continue;
                }

                m_routes.Add(r.m_Key, r);
            }

            while (GameMode.Current == null && GameState.Current == null)
            {
                yield return null;
            }

            GameState gameState = GameState.Current;
            if (gameState.m_RouteController == null)
                gameState.m_RouteController = this;
        }

        public Route GetRoute(string locationId)
        {
            return m_routes[locationId];
        }

        private void OnDestroy()
        {
            GameState gameState = GameState.Current;
            if (gameState.m_RouteController == this)
                gameState.m_RouteController = null;
        }
    }


    /// <summary>
    /// Represents the route information between the incident scene and a target location.
    /// </summary>
    [Serializable]
    public class Route
    {
        public string m_Key;
        public ALocationPoint m_Location;
        public float m_Distance;
        public float m_TravelTime;


        public Route()
        {
            m_Key = "key";
            m_Location = null;
            m_Distance = 0.0f;
            m_TravelTime = 0.0f;
        }

        public Route(string key, ALocationPoint loc, float dist, float travelTime)
        {
            m_Key = key;
            m_Location = loc;
            m_Distance = dist;
            m_TravelTime = travelTime;
        }
    }
}
