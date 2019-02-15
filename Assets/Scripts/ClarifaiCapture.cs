using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using Clarifai.API;
using Clarifai.DTOs.Inputs;
using Clarifai.DTOs.Predictions;

public class ClarifaiCapture : MonoBehaviour
{
    public enum Product { None, Headphones, Speakers, Phone };


    // Clarifai API Key
    private readonly string _clarifaiApiKey = "e78c1f6b15964fc3b07d1e20242a21e0";

    /// The Clarifai client that exposes all methods available.
    private ClarifaiClient _client;

    /// Whether a new Clarifai request is allowed at this point or not.
    private bool _allowNewRequest = false;

    /// The time delta per FixedUpdate.
    private float _fixedUpdateTimeDelta = 0;

    /// Number of Requests
    private readonly int NumRequests = 3;

    // Match UI Text
    public Text MatchText;
    public Product matchedProduct = Product.None;
    public Button shopperButton;
    [Space(10)]
    //Store pages for each of the trained objects.
    public GameObject headPhoneView;
    public GameObject speakerView;
    public GameObject phoneView;

    //'Thinking Ring' when you take a picture.
    public GameObject loadRing;
    public float spinSpeed = 200.0f;

    private String HighestMatch;
    private Decimal HighestMatchPercent = 0;

    private bool spinning = false;


    // Use this for initialization
    void Start()
    {
        shopperButton.GetComponent<Image>().CrossFadeAlpha(0f, 0f, true);
        loadRing.GetComponent<Image>().CrossFadeAlpha(0f, 0f, true);
        _allowNewRequest = false;
        // We set this callback in order to allow doing HTTPS requests which are done against the
        // Clarifai API endpoint.
        ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallback;

        // You can skip the API key argument if you have an environmental variable set called
        // CLARIFAI_API_KEY that contains your Clarifai API key.
        _client = new ClarifaiClient(_clarifaiApiKey);

        //ClarifaiRequestTimer();
    }

    // Use this for a button press to take a snapshot
    public void AllowRequest()
    {
        _allowNewRequest = true;
        MatchText.text = "Thinking...";
        shopperButton.GetComponent<Image>().CrossFadeAlpha(0f, 0f, true);
        shopperButton.GetComponent<ShopperManager>().bobbing = false;
        loadRing.GetComponent<Image>().CrossFadeAlpha(0.7f, 1f, true);
        spinning = true;
    }

    /// Called after camera is finished rendering the scene.
    /// We take a screenshot at this point, and then perform the Clarifai Predict request using the
    /// screenshot as an input image.
    async void OnPostRender()
    {
        if (_allowNewRequest)
        {
            _allowNewRequest = false;

            // Use this for taking snapshot of camera
            byte[] bytes = TakeScreenshot();

            // Use this for images within file system
            //byte[] bytes = File.ReadAllBytes("C:/Users/winst/Documents/VisualEyes/Assets/Images/can.jpg");

            await Task.Run(async () => {

                // Perform the Clarifai Predict request.
                var response = await _client.Predict<Concept>(
                        "VisualEyes",
                        new ClarifaiFileImage(bytes),
                        modelVersionID: "4b352078561b4f35b6827312e20e4d67",
                        maxConcepts: NumRequests)
                    .ExecuteAsync();


                if (response.IsSuccessful)
                {
                    HighestMatchPercent = 0;

                    foreach(Concept c in response.Get().Data)
                    {
                        string str = string.Format("{0} - Match: {1:N2}%", c.Name, c.Value * 100);

                        if (HighestMatchPercent == 0)
                        {
                            HighestMatchPercent = (decimal)(c.Value * 100);
                            HighestMatch = c.Name;
                        }
                        if(c.Value * 100 > HighestMatchPercent)
                        {
                            HighestMatchPercent = (decimal)(c.Value * 100);
                            HighestMatch = c.Name;
                        }
                        //Debug.Log(str);
                    }

                    Debug.Log(HighestMatch);
                    switch (HighestMatch)
                    {
                        case "JBL Charge 3":
                            matchedProduct = Product.Speakers;
                            break;
                        case "Sony 1000X M3 Headphones":
                            matchedProduct = Product.Headphones;
                            break;
                        case "Huawei Honor Phone":
                            matchedProduct = Product.Phone;
                            break;
                        default:
                            break;
                    }
                }

                else
                {
                    HighestMatch = "Clarifai request has not been successful.";
                    Debug.Log("Response error details:" + response.Status.ErrorDetails);
                    Debug.Log("Response description: " + response.Status.Description);
                    Debug.Log("Response code: " + response.Status.StatusCode);
                    Debug.Log("Clarifai request has not been successful.");
                }
            });
            Thread.Sleep(300);
            if (matchedProduct != Product.None)
            {
                ShowShopper();
            }
            else
            {
                shopperButton.interactable = false;
            }
            ChangeMatchText();
            showView(true);
            spinning = false;
            loadRing.GetComponent<Image>().CrossFadeAlpha(0f, 1f, true);
        }
    }

    private void ChangeMatchText()
    {
        MatchText.text = HighestMatch;
    }

    private void ShowShopper()
    {
        shopperButton.GetComponent<Image>().CrossFadeAlpha(1, 0.4f, true);
        shopperButton.GetComponent<ShopperManager>().bobbing = true;
        shopperButton.interactable = true;
    }


    private void Update()
    {
        if (spinning)
        {
            loadRing.transform.Rotate(Vector3.back * spinSpeed * Time.deltaTime);
        }     
    }

    /// This method is called in fixed time intervals.
    void FixedUpdate()
    {
        _fixedUpdateTimeDelta = Time.deltaTime;
    }

    /// Takes a screenshot and returns a PNG-encoded byte array.
    private byte[] TakeScreenshot()
    {
        // Create a texture the size of the screen, RGB24 format.
        int width = Screen.width;
        int height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture.
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG.
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);
        return bytes;
    }


    /// HTTPS validation (since the Clarifai API endpoint uses HTTPS).
    /// Source: https://stackoverflow.com/a/33391290/365837
    private bool CertificateValidationCallback(System.Object sender, X509Certificate certificate,
        X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain,
        // look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            foreach (X509ChainStatus t in chain.ChainStatus)
            {
                if (t.Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build((X509Certificate2)certificate);
                if (!chainIsValid)
                {
                    isOk = false;
                    break;
                }
            }
        }
        return isOk;
    }



    public void showView(bool state)
    {
        switch (matchedProduct)
        {
            case Product.Headphones:
                headPhoneView.SetActive(state);
                break;
            case Product.Speakers:
                speakerView.SetActive(state);
                break;
            case Product.Phone:
                phoneView.SetActive(state);
                break;
        }
    }

}


