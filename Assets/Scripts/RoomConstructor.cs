using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;

[InitializeOnLoad]
public class RoomConstructor : MonoBehaviour {
    [SerializeField] private GameObject[] m_prefabs = new GameObject[4];
    [SerializeField] private GameObject[] m_sockets = new GameObject[16];

    private static GameObject m_prefab;
    private static string m_prefabPath;

    public Room RoomData;
    private void Awake() {
        RoomData = new Room();
    }
    static RoomConstructor(){
        // PrefabStage.prefabSaved += OnPrefabSaved;
        PrefabStage.prefabStageOpened += OnPrefabStageOpened;
        // PrefabUtility.prefabInstanceUpdated += OnPrefabSaved;
    }

    static void OnPrefabStageOpened(PrefabStage stage){
        if (stage.stageHandle.FindComponentOfType<RoomConstructor>()){
            m_prefab = stage.prefabContentsRoot;
            m_prefabPath = stage.prefabAssetPath;
        }
    }

    static void OnPrefabSaved(GameObject prefab){
        m_prefab = prefab;
    }

    public void ConstructRunTime(int[] id){
        Debug.Assert(id.Length == 8);
        for(int i = 0; i < 8; i++){
            var prefab = m_prefabs[id[i]];
            for(int j = 0; j < 2; j++){
                var index = i * 2 + j;
                var oldObj = m_sockets[index];
                var newObj = Instantiate<GameObject>(prefab);
                newObj.transform.parent = oldObj.transform.parent;
                newObj.transform.localPosition = oldObj.transform.localPosition;
                newObj.transform.localRotation = oldObj.transform.localRotation;
                newObj.transform.localScale = oldObj.transform.localScale;
                newObj.transform.SetSiblingIndex(oldObj.transform.GetSiblingIndex());
                Destroy(m_sockets[index]);
                m_sockets[index] = newObj;
            }
        }
    }

    public void Construct(){
        Debug.Assert(name.Length == 8);
        for(int i = 0; i < 8; i++){
            var tile = (int)Char.GetNumericValue(name, 8-i-1);
            var prefab = m_prefabs[tile];
            for (int j = 0; j < 2; j++) {
                ReplaceTile(i * 2 + j, prefab);
            }
        }
    }

    public void RenameToQuaternary(){
        Debug.Assert(name.Length == 4);
        var n = Convert.ToUInt16(name, 16);
        var newName = n.ToQuatStr();
        var oldName = name;
        name = newName;
        AssetDatabase.RenameAsset(m_prefabPath, newName);
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
