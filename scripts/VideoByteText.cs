using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using OpenCVForUnityExample;

public class VideoByteText : MonoBehaviour
{

    public TextMeshPro textDisplay;
    public VideoPanelApp VideoPanelApp;
    public ARucoOpenCV ARucoOpenCV;

    string VideoPanel = "VideoPanel notset";
    public static string running = "not connected";
    public static string MatrixCtW = "not connected";
    public static string CodePosition = "not connected";
    
    // Start is called before the first frame update
    void Start()
    {
        textDisplay = gameObject.GetComponent<TextMeshPro>();
    }


    // Update is called once per frame
    void Update()
    {   
        if (VideoPanelApp._latestImageBytes.Length > 0)
        {
            VideoPanel = "first byte: " + VideoPanelApp._latestImageBytes[0].ToString();
        }
        else
        {
            VideoPanel = "length of bytes is: " + VideoPanelApp._latestImageBytes.Length.ToString();
        }


        //textDisplay.text = VideoPanel + "\nMarkerID: " + ARucoOpenCV.markerID.ToString() + "\npictureeee: " + running + "\nMatrix: " + MatrixCtW + "\n CodePosition: " + CodePosition;
        textDisplay.text = VideoPanel + "\nnum contours: " + BallTracker.num_contours.ToString()+"\ncenter: " + BallTracker.x.ToString()+" "+ BallTracker.y.ToString() + " " + BallTracker.z.ToString() + "\n state: " + BallTracker.state + "\nMatrix: " + MatrixCtW;


    }
}
