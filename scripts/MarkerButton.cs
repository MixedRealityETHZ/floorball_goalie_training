using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnityExample;

public class MarkerButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void click()
    {
        if (ARucoOpenCV.CheckForARucoMarkers == true)
        {
            ARucoOpenCV.CheckForARucoMarkers = false;
        }
        else
        {
            ARucoOpenCV.CheckForARucoMarkers = true;
        }


    }
}
