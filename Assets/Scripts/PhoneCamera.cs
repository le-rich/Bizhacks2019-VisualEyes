using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture webCamTexture;
    private Texture defaultBackground;

    public RawImage rawImage;
    public AspectRatioFitter fit;

    // Start is called before the first frame update
    void Start()
    {
        defaultBackground = rawImage.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No Camera Detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                webCamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (webCamTexture == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        rawImage.material.mainTexture = webCamTexture;
        webCamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!camAvailable)
        {
            return;
        }

        float ratio = (float)webCamTexture.width / (float)webCamTexture.height;
        fit.aspectRatio = ratio;

        int orient = -webCamTexture.videoRotationAngle;
        rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }
}
