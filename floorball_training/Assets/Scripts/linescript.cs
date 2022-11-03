using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR.Input;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;

public class linescript : MonoBehaviour
{
    public LineRenderer lineTL;     //top left
    public LineRenderer lineTR;         
    public LineRenderer lineBL;
    public LineRenderer lineBR;     //bottom right
    public Transform ballTrans;
    public Transform goalTrans;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {   
        //lines only between 2 points
        lineTL.positionCount = 2;
        lineTR.positionCount = 2;
        lineBL.positionCount = 2;
        lineBR.positionCount = 2;

        

        //Debug.Log("output: " + goalTrans.localToWorldMatrix.);
    }

    // Update is called once per frame
    void Update()
    {
        //var degY = goalTrans.rotation[1];

        var RotMat = goalTrans.localToWorldMatrix;


        //Debug.Log(RotMat[0,0] +" "+ RotMat[0,1] + " " + RotMat[0,2] + " " + RotMat[0,3] + " " + RotMat[1,0] + " " + RotMat[1,1] + " " + RotMat[1,2] + " " + RotMat[1,3] + " " + RotMat[2,0] + " " + RotMat[2,1] + " " + RotMat[2,2] + " " + RotMat[2,3] + " " + RotMat[3,0] + " " + RotMat[3,1] + " " + RotMat[3,2] + " " + RotMat[3,3]);
        //Debug.Log(RotMat.m00 + RotMat.m01 + RotMat.m02 + RotMat.m03 + RotMat.m10 + RotMat.m11 + RotMat.m12 + RotMat.m13 + RotMat.m20 + RotMat.m21 + RotMat.m22 + RotMat.m23 + RotMat.m30 + RotMat.m31 + RotMat.m32 + RotMat.m33);

        //position of goal corners in worldframe
        Vector3 TL = RotMat * new Vector4(0.5f, 0.5f, 0.5f, 1f);        //Top Left
        Vector3 TR = RotMat * new Vector4(-0.5f, 0.5f, 0.5f, 1f);       //Top Right
        Vector3 BL = RotMat * new Vector4(0.5f, -0.5f, 0.5f, 1f);       //Bottom Left
        Vector3 BR = RotMat * new Vector4(-0.5f, -0.5f, 0.5f, 1f);      //Bottom Right

        //set lines between goal corners and ball
        lineTL.SetPosition(0, ballTrans.position);
        lineTL.SetPosition(1, TL);

        lineTR.SetPosition(0, ballTrans.position);
        lineTR.SetPosition(1, TR);

        lineBL.SetPosition(0, ballTrans.position);
        lineBL.SetPosition(1, BL);

        lineBR.SetPosition(0, ballTrans.position);
        lineBR.SetPosition(1, BR);
    }
}
