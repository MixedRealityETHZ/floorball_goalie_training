using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class framescript : MonoBehaviour
{
    public Transform ballTrans;
    public Transform goalTrans;
    public LineRenderer centerLine;         //line for debugging and visualization, currently not visable since diameter = 0, can be set in the object preferences

    public float dist;                      // distance of frame from goal to ball from 0.0 to 1.0

    // Start is called before the first frame update
    void Start()
    {          
        centerLine.positionCount = 2;       //line between 2 points
    }

    // Update is called once per frame
    void Update()

    {   
        //calc Rotation Matrix
        var RotMat = goalTrans.localToWorldMatrix;


        //calc Vector from goal coordinate system to center of the surface
        Vector3 goalCentToSurface = RotMat * new Vector3(0f, 0f, 0.5f);


        //calculate vector from goal center to ball
        Vector3 conVec = ballTrans.position - (goalTrans.position + goalCentToSurface);


        //line for debugging from goal center to ball
        centerLine.SetPosition(0, ballTrans.position);
        centerLine.SetPosition(1, ballTrans.position - conVec);


        //set position of frame
        gameObject.transform.position = goalTrans.position + goalCentToSurface + dist * conVec;


        //set rotation of frame
        var conVecAng = Mathf.Atan2(conVec.x, conVec.z) * Mathf.Rad2Deg;    //rot angle around y orthogonal to conVec
        

        //gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, conVecAng, gameObject.transform.eulerAngles.z);  //frame orthogonal to conVec
        gameObject.transform.rotation = goalTrans.rotation;                                                                               //frame parallel to goal        


        //calculate size of frame
        float frameHeight = (1 - dist) * goalTrans.localScale.y;
        //float frameWidth = Mathf.Cos(conVecAng * Mathf.Deg2Rad) * goalTrans.localScale.x * dist;    //frame orthogonal to conVec, not possible due to distortion
        float frameWidth = (1 - dist) * goalTrans.localScale.x;                                         //frame parallel to goal


        gameObject.transform.localScale = new Vector3(frameWidth ,frameHeight, gameObject.transform.localScale.z);
    }
}
