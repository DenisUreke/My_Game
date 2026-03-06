using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SequenceAsset))]
public class SequenceAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var stepsProp = serializedObject.FindProperty("steps");
        EditorGUILayout.PropertyField(stepsProp, new GUIContent("Steps"), includeChildren: false);

        // Draw each step
        for (int i = 0; i < stepsProp.arraySize; i++)
        {
            var stepProp = stepsProp.GetArrayElementAtIndex(i);
            var actionsProp = stepProp.FindPropertyRelative("actions");

            EditorGUILayout.Space(6);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField($"Step {i}", EditorStyles.boldLabel);

                // Show existing actions (with their fields)
                for (int a = 0; a < actionsProp.arraySize; a++)
                {
                    var actionProp = actionsProp.GetArrayElementAtIndex(a);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(actionProp, GUIContent.none, includeChildren: true);

                        if (GUILayout.Button("X", GUILayout.Width(22)))
                        {
                            actionsProp.DeleteArrayElementAtIndex(a);
                            break;
                        }
                    }

                    EditorGUILayout.Space(4);
                }

                // Buttons that add correctly-typed actions
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Add Move"))
                        AddManaged(actionsProp, typeof(MoveToAction));

                    if (GUILayout.Button("Add Wait"))
                        AddManaged(actionsProp, typeof(WaitAction));
                }
            }
        }

        // Add a new step button (optional, but handy)
        EditorGUILayout.Space(8);
        if (GUILayout.Button("Add Step"))
        {
            stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
            var newStep = stepsProp.GetArrayElementAtIndex(stepsProp.arraySize - 1);
            var newActions = newStep.FindPropertyRelative("actions");
            if (newActions != null) newActions.ClearArray();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AddManaged(SerializedProperty actionsProp, Type t)
    {
        int idx = actionsProp.arraySize;
        actionsProp.InsertArrayElementAtIndex(idx);
        var elem = actionsProp.GetArrayElementAtIndex(idx);
        elem.managedReferenceValue = Activator.CreateInstance(t);
    }
}