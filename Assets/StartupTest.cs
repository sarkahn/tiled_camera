using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class StartupTest : MonoBehaviour
{
    PixelPerfectCamera _pixelCam;
    Camera _cam;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _cam = Camera.main;
        _pixelCam = _cam.GetComponent<PixelPerfectCamera>();
        PrintViewportStats();
        yield return null;
        PrintViewportStats();
        yield return null;
        PrintViewportStats();
        yield return null;
    }

    void PrintViewportStats()
    {
        Debug.Log($"Cam rect {_cam.pixelRect}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
