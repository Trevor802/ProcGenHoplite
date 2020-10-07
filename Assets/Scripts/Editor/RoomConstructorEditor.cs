using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomConstructor)), CanEditMultipleObjects]
public class RoomConstructorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GUILayout.Space(20);
        if (GUILayout.Button("Construct")){
            var constructor = (RoomConstructor)target;
            constructor.Construct();
        }
        if (GUILayout.Button("Hex to Quat")){
            var constructor = (RoomConstructor)target;
            constructor.RenameToQuaternary();
            constructor.Construct();
        }

    }
} 
