using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
public class ScreenshotToolWindow : EditorWindow
{
    private Camera selectedCamera;
    private RenderTexture previewTexture;
    private Vector2 previewSize = new Vector2(750, 500);
    private Vector2 scrollPosition;

    public string _path { get; set; } = "Assets/";
    private string fileName;
    private int height = 512;
    private int width = 512;
    private bool canShowUI;
    private bool is3D;

    private bool hideSky;

    private string cameraIdentifier;
    public ImageFormat imageFormat = ImageFormat.PNG;
    public enum ImageFormat
    {
        PNG,
        JPEG,
        EXR,
#if UNITY_2019_OR_NEWER
        TGA,
#endif
    }

    private GUIStyle HeaderStyle;
    private GUIStyle smallHeader;
    private GUIStyle TitleStyle;

    private GUIStyle toggleStyle;

    [MenuItem("Tools/MShot")]
    public static void ShowWindow()
    {
        GetWindow<ScreenshotToolWindow>("MShot");

    }
    private void OnEnable()
    {
        width = LoadWidth();
        height = LoadHeight();
        _path = LoadPath();

        cameraIdentifier = LoadCamera();
        UpdateSelectedCamera();
        imageFormat = LoadImageFormat();
        fileName = LoadFileName();
        canShowUI = LoadToggleUI();
        is3D = LoadCameraPrespective();
    }
    private void OnDisable()
    {
        SaveImageFormat(imageFormat);
        SavePath(_path);
        SaveCamera(selectedCamera);
        SaveFileName(fileName);
        SaveResolution(width, height);
        SaveToggleUI(canShowUI);
        SaveCameraPrespective(is3D);
    }

    #region SaveAndLoad

    private void SaveResolution(int w, int h)
    {
        EditorPrefs.SetInt("Width", w);
        EditorPrefs.SetInt("Height", h);
    }
    private int LoadWidth()
    {
        return EditorPrefs.GetInt("Width", 512);
    }
    private int LoadHeight()
    {
        return EditorPrefs.GetInt("Height", 512);
    }
    private void SaveFileName(string fileName)
    {
        EditorPrefs.SetString("fileName", fileName);
    }
    private string LoadFileName()
    {
        return EditorPrefs.GetString("fileName", "");
    }
    private void SaveCamera(Camera camera)
    {
        if (camera != null)
        {
            EditorPrefs.SetString("CameraIdentifier", camera.GetInstanceID().ToString());
        }
        else
        {
            EditorPrefs.DeleteKey("CameraIdentifier");
        }
    }

    private string LoadCamera()
    {
        return EditorPrefs.GetString("CameraIdentifier", "");
    }
    private void UpdateSelectedCamera()
    {
        if (!string.IsNullOrEmpty(cameraIdentifier))
        {
            int instanceID;
            if (int.TryParse(cameraIdentifier, out instanceID))
            {
                selectedCamera = EditorUtility.InstanceIDToObject(instanceID) as Camera;
            }
        }
    }

    private void SavePath(string path)
    {
        EditorPrefs.SetString("SavePath", path);
    }

    private string LoadPath()
    {
        return EditorPrefs.GetString("SavePath", "Assets/");
    }
    private void SaveImageFormat(ImageFormat format)
    {
        int formatAsInt = (int)format;
        EditorPrefs.SetInt("ImageFormat", formatAsInt);
    }

    private ImageFormat LoadImageFormat()
    {
        int formatAsInt = EditorPrefs.GetInt("ImageFormat", 0);
        return (ImageFormat)formatAsInt;
    }
    private void SaveToggleUI(bool toggleUi)
    {
        EditorPrefs.SetBool("UI", toggleUi);
    }
    private bool LoadToggleUI()
    {
        return EditorPrefs.GetBool("UI", false);
    }
    private void SaveCameraPrespective(bool is3D)
    {
        EditorPrefs.SetBool("3D", is3D);
    }
    private bool LoadCameraPrespective()
    {
        return EditorPrefs.GetBool("3D", false);
    }
    #endregion
    private void OnGUI()
    {
        InitializeGUI();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUILayout.Label("Settings", HeaderStyle);
        GUILayout.Space(10);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Select Camera", TitleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        selectedCamera = EditorGUILayout.ObjectField(selectedCamera, typeof(Camera), true) as Camera;
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Camera Path", TitleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.BeginHorizontal();
        _path = EditorGUILayout.TextField(_path);

        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            _path = EditorUtility.OpenFolderPanel("Select Save Path:", _path, "");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("File Name", TitleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        fileName = EditorGUILayout.TextField(fileName);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Image Format:", TitleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUIStyle dropDownStyle = new GUIStyle("Dropdown");
        dropDownStyle.normal.textColor = Color.grey;
        dropDownStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.BeginHorizontal();


        if (GUILayout.Button(imageFormat.ToString(), dropDownStyle))
        {
            GenericMenu menu = new GenericMenu();


            foreach (ImageFormat format in Enum.GetValues(typeof(ImageFormat)))
            {
                menu.AddItem(new GUIContent(format.ToString()), imageFormat == format, () => imageFormat = format);
            }

            menu.ShowAsContext();
        }


        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUIStyle buttonStyle = new GUIStyle("Button");
        buttonStyle.normal.textColor = Color.gray;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        canShowUI = GUILayout.Toggle(canShowUI, new GUIContent("Toggle UI"), buttonStyle, GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        hideSky = GUILayout.Toggle(hideSky, new GUIContent("Hide Sky"), buttonStyle, GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();


        GUIStyle activeButtonStyle3D = new GUIStyle(buttonStyle);
        GUIStyle inactiveButtonStyle3D = new GUIStyle(buttonStyle);
        activeButtonStyle3D.normal.textColor = Color.blue;
        inactiveButtonStyle3D.normal.textColor = Color.gray;

        is3D = GUILayout.Toggle(is3D, new GUIContent("3D"), is3D ? activeButtonStyle3D : inactiveButtonStyle3D, GUILayout.Width(30));


        GUIStyle activeButtonStyle2D = new GUIStyle(buttonStyle);
        GUIStyle inactiveButtonStyle2D = new GUIStyle(buttonStyle);
        activeButtonStyle2D.normal.textColor = Color.blue;
        inactiveButtonStyle2D.normal.textColor = Color.gray;

        is3D = !GUILayout.Toggle(!is3D, new GUIContent("2D"), is3D ? inactiveButtonStyle2D : activeButtonStyle2D, GUILayout.Width(30));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("Resolution", smallHeader);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Width", TitleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        width = EditorGUILayout.IntSlider(width, 768, 2048);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Height", TitleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        height = EditorGUILayout.IntSlider(height, 1024, 2048);


        if (selectedCamera != null && (previewTexture == null || previewTexture.width != (int)previewSize.x || previewTexture.height != (int)previewSize.y))
        {
            bool previousCameraView = selectedCamera.orthographic;
            if (is3D)
            {
                selectedCamera.orthographic = false;
            }
            else
            {
                selectedCamera.orthographic = true;
            }
            var previousFlags = selectedCamera.clearFlags;

            if (hideSky)
            {
                selectedCamera.clearFlags = CameraClearFlags.Nothing;
            }
            else
            {
                selectedCamera.clearFlags = previousFlags;
            }
            previewTexture = RenderPreview(selectedCamera);
            previewSize.x = width;
            previewSize.y = height;
        }
        if (selectedCamera != null)
        {
            EditorGUILayout.HelpBox($"Press 'Refresh Preview' to update the preview of the camera after making any changes.", MessageType.Info, true);
        }
        if (selectedCamera != null && previewTexture != null)
        {
            if (GUILayout.Button("Refresh Preview"))
            {
                bool previousCameraView = selectedCamera.orthographic;
                if (is3D)
                {
                    selectedCamera.orthographic = false;
                }
                else
                {
                    selectedCamera.orthographic = true;
                }
                var previousFlags = selectedCamera.clearFlags;

                if (hideSky)
                {
                    selectedCamera.clearFlags = CameraClearFlags.Nothing;
                }
                else
                {
                    selectedCamera.clearFlags = previousFlags;
                }

                if (selectedCamera != null)
                {
                    previewTexture = RenderPreview(selectedCamera);
                    previewSize.x = width;
                    previewSize.y = height;
                    selectedCamera.orthographic = previousCameraView;
                    selectedCamera.clearFlags = previousFlags;
                }
            }
        }
        GUILayout.Space(10);
        if (selectedCamera != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIStyle screenshotButtonStyle = new GUIStyle("Button");
            screenshotButtonStyle.fontSize = 16;
            screenshotButtonStyle.fontStyle = FontStyle.Bold;

            if (GUILayout.Button("Take Screenshot", screenshotButtonStyle, GUILayout.Width(200), GUILayout.Height(40)))
            {
                if (!string.IsNullOrEmpty(fileName) && Directory.Exists(_path))
                {
                    TakeScreenShot(selectedCamera, _path, imageFormat);
                }
                else
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        Debug.LogError("Please specify a name for the image");
                    }
                    if (!Directory.Exists(_path))
                    {
                        Debug.LogError("Please specify a valid directory for the image");
                    }
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        if (selectedCamera != null && previewTexture != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Camera Preview", TitleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawTexture(previewTexture, GUILayout.Width(previewSize.x), GUILayout.Height(previewSize.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        Repaint();
    }

    private void InitializeGUI()
    {
        HeaderStyle = new GUIStyle
        {
            fontSize = 50,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,

        };
        HeaderStyle.normal.textColor = Color.white;

        smallHeader = new GUIStyle
        {
            fontSize = 30,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        smallHeader.normal.textColor = Color.white;

        TitleStyle = new GUIStyle
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold
        };
        TitleStyle.normal.textColor = Color.white;

        toggleStyle = new GUIStyle(EditorStyles.toggle)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            fontStyle = FontStyle.Bold

        };
    }

    private RenderTexture RenderPreview(Camera camera)
    {
        int width = Mathf.RoundToInt(previewSize.x);
        int height = Mathf.RoundToInt(previewSize.y);

        RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.Default)
        {
            enableRandomWrite = true
        };

        if (canShowUI)
        {
            Dictionary<Canvas, Tuple<RenderMode, Camera>> canvasData = new Dictionary<Canvas, Tuple<RenderMode, Camera>>();

            foreach (Canvas canvas in FindObjectsOfType<Canvas>())
            {
                canvasData.Add(canvas, new Tuple<RenderMode, Camera>(canvas.renderMode, canvas.worldCamera));
            }

            foreach (var pair in canvasData)
            {
                Canvas canvas = pair.Key;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = selectedCamera;
            }

            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = null;
            foreach (var pair in canvasData)
            {
                Canvas canvas = pair.Key;
                (canvas.renderMode, canvas.worldCamera) = pair.Value;
            }
        }
        else
        {
            Dictionary<Canvas, Tuple<RenderMode, Camera>> canvasData = new Dictionary<Canvas, Tuple<RenderMode, Camera>>();

            foreach (Canvas canvas in FindObjectsOfType<Canvas>())
            {
                canvasData.Add(canvas, new Tuple<RenderMode, Camera>(canvas.renderMode, canvas.worldCamera));
            }
            foreach (var pair in canvasData)
            {
                Canvas canvas = pair.Key;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = selectedCamera;
            }

            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = null;
            foreach (var pair in canvasData)
            {
                Canvas canvas = pair.Key;
                (canvas.renderMode, canvas.worldCamera) = pair.Value;
            }
        }

        return renderTexture;
    }

    private void DrawTexture(Texture texture, params GUILayoutOption[] options)
    {
        Rect rect = GUILayoutUtility.GetRect(0, 0, options);
        GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
    }

    public void TakeScreenShot(Camera camera, string fullPath, ImageFormat format)
    {

        RenderTexture originalTargetTexture = camera.targetTexture;
        bool previousCameraView = selectedCamera.orthographic;
        if (is3D)
        {
            camera.orthographic = false;
        }
        else
        {
            camera.orthographic = true;
        }
        var previousFlags = selectedCamera.clearFlags;

        if (hideSky)
        {
            selectedCamera.clearFlags = CameraClearFlags.Nothing;
        }
        else
        {
            selectedCamera.clearFlags = previousFlags;
        }
        float aspectRatio = (float)width / height;

        RenderTexture rt = new RenderTexture(width, Mathf.RoundToInt(width / aspectRatio), 24);
        camera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(width, Mathf.RoundToInt(width / aspectRatio), TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, Mathf.RoundToInt(width / aspectRatio)), 0, 0);
        camera.targetTexture = originalTargetTexture;
        RenderTexture.active = null;
        camera.orthographic = previousCameraView;

        DestroyImmediate(rt);

        byte[] bytes;
        string extension;

        switch (imageFormat)
        {
            case ImageFormat.PNG:
                bytes = screenShot.EncodeToPNG();
                extension = "png";
                break;
            case ImageFormat.JPEG:
                bytes = screenShot.EncodeToJPG();
                extension = "jpg";
                break;
            case ImageFormat.EXR:
                bytes = screenShot.EncodeToEXR();
                extension = "exr";
                break;
            default:
                bytes = screenShot.EncodeToPNG();
                extension = "png";
                break;
        }
#if UNITY_2019_OR_NEWER
        if (imageFormat == ImageFormat.TGA)
        {
            bytes = screenShot.EncodeToTGA();
            extension = "tga";
        }
#endif
        int count = 0;
        string baseFileName = fileName;
        string absolutePath = Path.Combine(_path, $"{baseFileName}.{extension}");

        while (File.Exists(absolutePath))
        {
            if (count > 0)
            {
                baseFileName = $"{fileName}_{count}";
            }

            absolutePath = Path.Combine(_path, $"{baseFileName}.{extension}");
            count++;
        }

        File.WriteAllBytes(absolutePath, bytes);

        Debug.Log($"Screenshot taken by '{camera.name}' have been saved to: '{fullPath}/{fileName}.{extension}' with resolution '{width}x{height}' and format '{imageFormat}'");
    }
}
