using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomConstructor : MonoBehaviour {
    [SerializeField] private GameObject[] m_prefabs = new GameObject[4];
    [SerializeField] private GameObject[] m_sockets = new GameObject[16];

    public void Construct(){
        // var prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
        int id = Convert.ToInt32(name, 16);
        for(int i = 0; i < 8; i++){
            var tile = id % 4;
            var prefab = m_prefabs[tile];
            for (int j = 0; j < 2; j++) {
                ReplaceTile(i * 2 + j, prefab);
            }
            id /= 4;
        }
    }

    private void ReplaceTile(int id, GameObject prefab) {
        var oldObject = m_sockets[id];
        var newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        // -- set up "undo" features for the new prefab, like setting up the old transform
        Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
        newObject.transform.parent = oldObject.transform.parent;
        newObject.transform.localPosition = oldObject.transform.localPosition;
        newObject.transform.localRotation = oldObject.transform.localRotation;
        newObject.transform.localScale = oldObject.transform.localScale;
        newObject.transform.SetSiblingIndex(oldObject.transform.GetSiblingIndex());
        // -- now delete the old prefab
        Undo.DestroyObjectImmediate(oldObject);
        m_sockets[id] = newObject;
    }
}
