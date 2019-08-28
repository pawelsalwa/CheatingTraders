using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AWI
{
    public class PrefabRespawner : MonoBehaviour
    {

        public List<GameObject> prefabs;

#if UNITY_EDITOR
        [ContextMenu("Respawn")]
        public void Respawn()
        {
            var allChilds = GetComponentsInChildren(typeof(Transform), true);
            var allGO = new List<GameObject>();
            foreach (var v in allChilds)
            {
                allGO.Add(v.gameObject);
            }
            foreach (var v in allGO)
            {
                var split = v.name.Split('(');
                v.name = split[0];
                while (v.name[v.name.Length - 1] == ' ')
                {
                    v.name = v.name.Remove(v.name.Length - 1);
                }
            }
            if (prefabs != null && prefabs.Count > 0)
            {
                var allPrefabNames = new List<string>();
                foreach (var v in prefabs)
                {
                    allPrefabNames.Add(v.name);
                }
                allGO.RemoveAll((x) => !allPrefabNames.Contains(x.name));
                allGO.Sort((x, y) => x.name.CompareTo(y.name));
                foreach (var v in allGO)
                {
                    var prefab = prefabs.Find((x) => x.name == v.name);
                    var instance = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    instance.transform.SetParent(v.transform.parent);
                    instance.transform.localPosition = v.transform.localPosition;
                    instance.transform.localRotation = v.transform.localRotation;
                    instance.transform.localScale = v.transform.localScale;
                }
                for (int i = 0; i < allGO.Count; ++i)
                {
                    DestroyImmediate(allGO[i]);
                }
            }

        }
#endif

    }

}
