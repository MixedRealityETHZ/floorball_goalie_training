using OpenCVForUnity.CoreModule;
using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMoveScript : MonoBehaviour
{   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        if(VideoPanelApp.cameraToWorldMatrix.Length > 0)
        {
            Matrix4x4 mat = ConvertFloatArrayToMatrix4x4(VideoPanelApp.cameraToWorldMatrix);
            Vector3 p = mat.MultiplyPoint(ARucoOpenCV.CodePosition);

            gameObject.GetComponent<Transform>().position = p;
        }
        else
        {
            gameObject.GetComponent<Transform>().position = ARucoOpenCV.CodePosition;
        }

        

    }

    public static Matrix4x4 ConvertFloatArrayToMatrix4x4(float[] matrixAsArray)
    {
        //There is probably a better way to be doing this but System.Numerics.Matrix4x4 is not available
        //in Unity and we do not include UnityEngine in the plugin.
        Matrix4x4 m = new Matrix4x4();

        m.m00 = matrixAsArray[0];
        m.m01 = matrixAsArray[1];
        m.m02 = matrixAsArray[2];
        m.m03 = matrixAsArray[3];
        m.m10 = matrixAsArray[4];
        m.m11 = matrixAsArray[5];
        m.m12 = matrixAsArray[6];
        m.m13 = matrixAsArray[7];
        m.m20 = matrixAsArray[8];
        m.m21 = matrixAsArray[9];
        m.m22 = matrixAsArray[10];
        m.m23 = matrixAsArray[11];
        m.m30 = matrixAsArray[12];
        m.m31 = matrixAsArray[13];
        m.m32 = matrixAsArray[14];
        m.m33 = matrixAsArray[15];

        return m;
    }
}
