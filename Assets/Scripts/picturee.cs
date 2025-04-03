using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows;
//using UnityEngine.XR.WSA.WebCam; for older unity version
using UnityEngine.Windows.WebCam;
//using System.Runtime.Hosting;
using System.IO;
using System.Security.Principal;

public class picturee : MonoBehaviour
{

    
    WebCamTexture webcam;
    public Texture2D image;
    GameObject quad;
    Renderer quadRenderer;
    bool done;
    int i = 0;
    Texture2D webcamImage;

    // Use this for initialization
    void Start()
    {
        webcam = new WebCamTexture();
        webcam.Play();
        webcamImage = new Texture2D(webcam.width, webcam.height);
        //Debug.LogFormat("webcam:  {0} {1}x{2}", webcam.deviceName, webcam.width, webcam.height);
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        quadRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        quad.transform.parent = this.transform;
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
        //StartCoroutine(ExampleCoroutine());
    }
    public Texture2D TakePhoto()
    {
        //Debug.Log("Take Photo");
        webcamImage.SetPixels(webcam.GetPixels());
        webcamImage.Apply();
        quadRenderer.material.mainTexture = webcamImage;
        return webcamImage;
    }

    async void Update()
    {
        if (i % 1 == 0)
        {
            image = TakePhoto();
            
        }
        i++;
        //byte[] im = image.EncodeToPNG();
        //UnityEngine.Debug.Log(image);
        //var camerarollfolder = Windows.Storage.KnownFolders.CameraRoll.Path;
        //UnityEngine.Debug.Log(camerarollfolder);
        //StorageFolder CameraFolder = Windows.Storage.KnownFolders.CameraRoll;
        //System.IO.File.WriteAllBytes(Application.dataPath+"/Resources/image.png", im);
    }


}