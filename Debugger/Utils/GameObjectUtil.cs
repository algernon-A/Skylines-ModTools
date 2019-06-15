﻿using System.Collections.Generic;
using UnityEngine;

namespace ModTools
{
    internal static class GameObjectUtil
    {
        public static Dictionary<GameObject, bool> FindSceneRoots()
        {
            var roots = new Dictionary<GameObject, bool>();

            foreach (var obj in Object.FindObjectsOfType<GameObject>())
            {
                if (!roots.ContainsKey(obj.transform.root.gameObject))
                {
                    roots.Add(obj.transform.root.gameObject, true);
                }
            }

            return roots;
        }

        public static List<KeyValuePair<GameObject, Component>> FindComponentsOfType(string typeName)
        {
            var roots = FindSceneRoots();
            var list = new List<KeyValuePair<GameObject, Component>>();
            foreach (var root in roots.Keys)
            {
                FindComponentsOfType(typeName, root, list);
            }

            return list;
        }

        public static void FindComponentsOfType(string typeName, GameObject gameObject, List<KeyValuePair<GameObject, Component>> list)
        {
            var component = gameObject.GetComponent(typeName);
            if (component != null)
            {
                list.Add(new KeyValuePair<GameObject, Component>(gameObject, component));
            }

            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                FindComponentsOfType(typeName, gameObject.transform.GetChild(i).gameObject, list);
            }
        }
    }
}