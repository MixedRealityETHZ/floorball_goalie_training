using Microsoft.MixedReality.Toolkit.UI;
using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandMenu : MonoBehaviour
{

    //inputs
    public Renderer VirtualBallRenderer;
    public Renderer GoalRenderer;

    public ManipulationHandler GoalManipulation;

    public TextMeshPro ARucoText;
    public TextMeshPro ManualGoalAlignmentText;
    public TextMeshPro BallTrackingText;
    public TextMeshPro VirtualObjectsText;
    public TextMeshPro ConstrainBallYText;







    // Start is called before the first frame update
    void Start()
    {
        if (ARucoOpenCV.CheckForARucoMarkers == true){ ARucoText.color = Color.green;}      
        if (BallTracker.BallTrackingEnabled == true){BallTrackingText.color = Color.green;}      
        if (ProjectionSpawner.LineRenderingEnabled == true){VirtualObjectsText.color = Color.green;}
        if (BallTracker.ConstrainBallY == true) { ConstrainBallYText.color = Color.green; }
        if (GoalManipulation.enabled == true){ManualGoalAlignmentText.color = Color.green;}
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ARuco()
    {
        if (ARucoOpenCV.CheckForARucoMarkers == true)
        {
            ARucoOpenCV.CheckForARucoMarkers = false;
            ARucoText.color = Color.white;
            GoalRenderer.enabled = false;


        }
        else{
            ARucoOpenCV.CheckForARucoMarkers = true;
            ARucoText.color = Color.green;
            GoalRenderer.enabled = true;

            GoalManipulation.enabled = false;
            ManualGoalAlignmentText.color = Color.white;
        }
    }

    public void ManualGoalAlignment()
    {
        if(GoalManipulation.enabled == true)
        {
            GoalManipulation.enabled = false;
            GoalRenderer.enabled = false;
            ManualGoalAlignmentText.color = Color.white;
        }
        else
        {
            GoalManipulation.enabled = true;
            GoalRenderer.enabled = true;
            ManualGoalAlignmentText.color = Color.green;

            ARucoOpenCV.CheckForARucoMarkers = false;
            ARucoText.color = Color.white;

        }
    }

    public void Balltracking()
    {
        if(BallTracker.BallTrackingEnabled == true)
        {
            BallTracker.BallTrackingEnabled = false;
            BallTrackingText.color = Color.white;
        }
        else
        {
            BallTracker.BallTrackingEnabled = true;
            BallTrackingText.color = Color.green;
        }
    }

    public void VirtualObjects()
    {
        if (ProjectionSpawner.LineRenderingEnabled == true)
        {
            ProjectionSpawner.LineRenderingEnabled = false;
            VirtualBallRenderer.enabled = false;
            VirtualObjectsText.color = Color.white;
        }
        else
        {
            ProjectionSpawner.LineRenderingEnabled = true;
            VirtualBallRenderer.enabled = true;
            VirtualObjectsText.color = Color.green;
        }
    }

    public void ConstrainBallY()
    {
        if (BallTracker.ConstrainBallY)
        {
            BallTracker.ConstrainBallY = false;
            ConstrainBallYText.color = Color.white;
        }
        else
        {
            BallTracker.ConstrainBallY = true;
            ConstrainBallYText.color = Color.green;
        }
    }


}
