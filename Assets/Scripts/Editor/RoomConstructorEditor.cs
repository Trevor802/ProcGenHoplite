using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomConstructor)), CanEditMultipleObjects]
public class RoomConstructorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Construct")){
            var constructor = (RoomConstructor)target;
            constructor.Construct();
        }
    }
} 
