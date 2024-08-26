
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConstantScriptableObject), true)]
public class ConstantScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.HelpBox("It's important to write the identifying names of the elements with spaces as separators.", MessageType.Warning);
        EditorGUILayout.HelpBox("Please generate the constants file each time you add a new object or change it's name.", MessageType.Info);

        var content = (ConstantScriptableObject)target;

        SerializedProperty className = serializedObject.FindProperty("ClassName");
        EditorGUILayout.PropertyField(className);
        content.ClassName = className.stringValue;

        SerializedProperty pathName = serializedObject.FindProperty("PathFileConstants");
        EditorGUILayout.PropertyField(pathName);
        content.PathFileConstants = pathName.stringValue;

        if (GUILayout.Button("Generate Constants"))
        {
            string classNameValue = content.ClassName != string.Empty ? content.ClassName : target.name + "Type";
            IConstant[] constants = content.GetConstants();
            if (constants.Length > 0)
                ConstantsGenerator.WriteConListToFile(classNameValue, constants, content.PathFileConstants);
        }

        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }
}