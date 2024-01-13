using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenShot : MonoBehaviour
{
    [HideInInspector]
    public int screenshotCount = 0;
    [HideInInspector]
    public string previousFileName;

    Camera camera;

    [SerializeField]
    private string _path = "Assets/";


    [HideInInspector]
    public string Path
    {
        get { return _path; }
        set { _path = value.StartsWith("Assets/") ? value : "Assets/" + value; }
    }


    public string fileName;

    [Range(1, 2048)]
    public int height;
    [Range(1, 2048)]
    public int width;

    [Tooltip("Choose Image Format")]
    public ImageFormat imageFormat = ImageFormat.PNG;
    public enum ImageFormat
    {
        PNG,
        JPEG,
        EXR,
        TGA
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeScreenShot(fileName,imageFormat);
            
        }
    }

    public void TakeScreenShot(string fullPath, ImageFormat format)
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
        }
        screenshotCount++;
        float aspectRatio = (float)width / height;

        RenderTexture rt = new RenderTexture(width, Mathf.RoundToInt(width / aspectRatio), 24);
        camera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(width, Mathf.RoundToInt(width / aspectRatio), TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, Mathf.RoundToInt(width / aspectRatio)), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        Destroy(rt);

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
            case ImageFormat.TGA:
                bytes = screenShot.EncodeToTGA();
                extension = "tga";
                break;
            default:
                bytes = screenShot.EncodeToPNG();
                extension = "png";
                break;
        }

        System.IO.File.WriteAllBytes($"{fullPath}_{screenshotCount}.{extension}", bytes);

        Debug.Log($"Screenshot taken by {camera.name} have been saved to: {fullPath}.{extension} with resolution {width}x{height} and format {imageFormat}");
    }
}
