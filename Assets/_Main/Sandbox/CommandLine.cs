#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class CommandLine : MonoBehaviour
{
    [SerializeField] private string nameStart;
    [SerializeField] private string nameEnd;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private GameObject basePrefab;
    
    [Button]
    private void Execute()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            var goName = $"{nameStart}{i + 1}{nameEnd}";
            var go = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
            go.name = goName;
            var gfx = go.transform.Find("GFX");
            var prefab = PrefabUtility.InstantiatePrefab(prefabs[i], gfx);
        }
    }
    [Button]
    private void Execute_old()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            var goName = $"{nameStart}{i + 1}{nameEnd}";
            var go = new GameObject(goName);//PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
            go.name = goName;

            var prefab = Instantiate(prefabs[i], go.transform);
            var transforms = prefab.GetComponentsInChildren<Transform>(true);
            var capsuleCollider = prefab.GetComponentInChildren<CapsuleCollider>();
            DestroyImmediate(capsuleCollider);
            
            foreach (var t in transforms)
            {
                if (!t.gameObject.activeSelf)
                {
                    DestroyImmediate(t.gameObject);
                }
            }
        }
    }
}
#endif
