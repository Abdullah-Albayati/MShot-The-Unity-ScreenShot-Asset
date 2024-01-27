using UnityEditor;
using UnityEngine;

namespace MShot
{
    [CustomEditor(typeof(PhotoMode))]
    public class PhotoModeEditor : Editor
    {
        SerializedProperty canUserTakeScreenShot;
        SerializedProperty screenshotKey;
        SerializedProperty screenshotFolder;

        private void OnEnable()
        {
            canUserTakeScreenShot = serializedObject.FindProperty("canUserTakeScreenShot");
            screenshotKey = serializedObject.FindProperty("screenshotKey");
            screenshotFolder = serializedObject.FindProperty("screenshotFolder");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("This will create an 'AppData' folder for the player.", MessageType.Info);
            EditorGUILayout.PropertyField(canUserTakeScreenShot, new GUIContent("Can User Take Screenshot"));
            if (canUserTakeScreenShot.boolValue == true)
            {
                EditorGUILayout.PropertyField(screenshotKey, new GUIContent("Screenshot Key"));
                EditorGUILayout.PropertyField(screenshotFolder, new GUIContent("Screenshot Folder"));
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
