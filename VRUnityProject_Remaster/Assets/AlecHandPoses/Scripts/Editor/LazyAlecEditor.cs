using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandPoseMaker))]
public class LazyAlecEditor : Editor
{
    SerializedProperty handRoot;
    SerializedProperty handPose;

    public void OnEnable()
    {
        handRoot = serializedObject.FindProperty("handRoot");
        handPose = serializedObject.FindProperty("currentPose");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(handRoot);
        EditorGUILayout.PropertyField(handPose);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Make Hand Pose"))
            (target as HandPoseMaker).MakeHandPose();
        if (GUILayout.Button("Reverse Transfer Bone Rotations"))
            (target as HandPoseMaker).ReverseTransferBoneRotations();
        if (GUILayout.Button("Make JSON from Hand Pose"))
            (target as HandPoseMaker).CreateJson();
    }
}
