#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraScreenShot))]
public class CameraScreenShotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        CameraScreenShot cameraScreenShot = (CameraScreenShot)target;

       
        if (!cameraScreenShot.Path.StartsWith("Assets/"))
        {
            cameraScreenShot.Path = "Assets/" + cameraScreenShot.Path;
            EditorUtility.SetDirty(cameraScreenShot);
        }

        if (GUILayout.Button("Capture Screenshot"))
        {
            if (!string.IsNullOrEmpty(cameraScreenShot.fileName))
            {
                cameraScreenShot.fileName = cameraScreenShot.fileName.Trim();

                if (string.IsNullOrEmpty(cameraScreenShot.previousFileName) || cameraScreenShot.previousFileName != cameraScreenShot.fileName)
                {
                    cameraScreenShot.screenshotCount = 0;
                    cameraScreenShot.previousFileName = cameraScreenShot.fileName;
                }

                string fullPath = $"{cameraScreenShot.Path}/{cameraScreenShot.fileName}";
                cameraScreenShot.TakeScreenShot(fullPath, cameraScreenShot.imageFormat);
            }
            else
            {
                Debug.LogWarning("Please enter a file name.");
            }
        }


        serializedObject.ApplyModifiedProperties();
    }
}
#endif
