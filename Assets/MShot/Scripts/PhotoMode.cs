using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PhotoMode : MonoBehaviour
{
    [SerializeField]
    bool canUserTakeScreenShot = true;
    [SerializeField]
    private KeyCode screenshotKey = KeyCode.F12;


    [SerializeField]
    private string screenshotFolder = "Screenshots";

   public static PhotoMode instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(screenshotKey) && canUserTakeScreenShot)
        {
            CaptureScreenshot(null);
        }
    }

    public void CaptureScreenshot(string screenShotName)
    {
        
        string folderPath = System.IO.Path.Combine(Application.persistentDataPath, screenshotFolder);

        
        System.IO.Directory.CreateDirectory(folderPath);

       
        string fileName;
        if(!string.IsNullOrEmpty(screenShotName)){
            fileName = $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        }
        else{
            fileName = screenShotName;
        }

        
        string filePath = System.IO.Path.Combine(folderPath, fileName);

        
        ScreenCapture.CaptureScreenshot(filePath);

       
        Debug.Log($"Screenshot saved to: {filePath}");
    }
}
