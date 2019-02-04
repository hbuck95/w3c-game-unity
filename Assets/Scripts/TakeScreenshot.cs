using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        print("Hi");
        if (Input.GetKeyDown(KeyCode.P))
        {
            print("Clicked");
            ScreenCapture.CaptureScreenshot("Screenshots/Figure" + System.Guid.NewGuid() + ".png", 1);
        }
    }
}
