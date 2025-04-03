using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UtilsModule;
using OpenCVForUnity.ImgprocModule;
using UnityEngine.UI;
using System.Diagnostics;

public class BallTracker : MonoBehaviour
{
    //Variable for switching Tracking On and Off
    public static bool BallTrackingEnabled = true;
    public static bool ConstrainBallY = true;

    public GameObject Goal;

    //get image
    public VideoPanelApp videoPanelApp;

    //texture sharing (for debugging)
    public static bool have_tex = false;
    public static Texture2D tex;
    public GameObject ball;
    public static int num_contours;
    public static string state = "";

    //result
    public static float x, y, z;

    //variable declarations for the computer vision stuff
    Mat img_mat;
    Mat img_mat_static = new Mat();
    Mat blurred_img = new Mat();
    Mat hsv_img = new Mat();
    Mat img_mask = new Mat();
    Mat eroded = new Mat();
    Mat dilated = new Mat();
    Mat to_contour = new Mat();
    Scalar lowerBound = new Scalar(40, 60, 50);
    Scalar upperBound = new Scalar(75, 255, 255);
    List<MatOfPoint> cntr = new List<MatOfPoint>();
    Mat hierarchy = new Mat();
    float[] radius = new float[1];
    Point center = new Point();
    Vector3 p;
    Texture2D imageTMP;
    double maxArea = 0;
    MatOfPoint argmax = null;
    Vector3 vec_pos = new Vector3(0, 0, 0);

    bool first = true;
    int skip = 4;

    void Start()
    {
        //StartCoroutine(init_tex());
        img_mat = new Mat(504 / skip, 896 / skip, CvType.CV_8UC3);
        imageTMP = new Texture2D(896 / skip, 504 / skip);
        StartCoroutine(tracking());
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator tracking()
    {
        while (true)
        {
            if (BallTrackingEnabled)
            {
                UnityEngine.Debug.Log("while");
                if (first && videoPanelApp._latestImageBytes.Length > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                    first = false;
                }
                if (videoPanelApp._latestImageBytes.Length > 0 && !first && VideoPanelApp.image != null)
                {
                    state = "balltracking started ";
                    UnityEngine.Debug.Log("balltracking");
                    downsample(VideoPanelApp.image, imageTMP, skip);
                    Utils.texture2DToMat(imageTMP, img_mat);
                    state += "texture to mat";
                    UnityEngine.Debug.Log("texturetomat works");
                    //blur
                    Imgproc.GaussianBlur(img_mat, blurred_img, new Size(3, 3), 2, 2);
                    //change to hsv space
                    Imgproc.cvtColor(blurred_img, hsv_img, Imgproc.COLOR_RGB2HSV);
                    UnityEngine.Debug.Log("hsv");
                    //mask
                    Core.inRange(hsv_img, lowerBound, upperBound, img_mask);
                    UnityEngine.Debug.Log("mask");
                    //erosion and dilation to get rid of noise (and have fewer contours)
                    Mat erodeElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(2, 2));
                    Mat dilateElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(2, 2));
                    //Imgproc.erode(img_mask, eroded, erodeElement);
                    Imgproc.dilate(img_mask, dilated, dilateElement);
                    state += " processed";
                    //tex = imageTMP;
                    tex = new Texture2D(dilated.cols(), dilated.rows(), TextureFormat.RGBA32, false);
                    Utils.matToTexture2D(dilated, tex);
                    have_tex = true;
                    //UnityEngine.Debug.Log("mat to texture");
                    //state += " tex ";
                    //find contours
                    cntr.Clear();
                    Imgproc.findContours(dilated, cntr, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
                    UnityEngine.Debug.Log("found contours");
                    UnityEngine.Debug.Log("there are so many:");
                    num_contours = cntr.Count;
                    state += " contours " + num_contours.ToString();
                    UnityEngine.Debug.Log(cntr.Count);
                    //look for biggest contour
                    bool exists = (cntr.Count != 0);
                    //found ball
                    if (exists)
                    {
                        maxArea = 0;
                        argmax = null;
                        foreach (MatOfPoint contour in cntr)
                        {
                            //UnityEngine.Debug.Log("here");
                            double area = Imgproc.contourArea(contour);
                            if (area > maxArea)
                            {
                                argmax = contour;
                                maxArea = area;
                            }
                        }
                        state += "areas ";
                        UnityEngine.Debug.Log("areas");
                        Imgproc.minEnclosingCircle(new MatOfPoint2f(argmax.toArray()), center, radius);
                        UnityEngine.Debug.Log("circle");
                        //state += " areas";

                        //unproject
                        z = (0.036f * (850f / (float)skip)) / radius[0];
                        y = -z * ((float)center.y - (504f / 2f) / (float)skip) / (850f / (float)skip);
                        x = z * ((float)center.x - (896f / 2f) / (float)skip) / (850f / (float)skip);
                        state += " x: " + x.ToString() + ", y: " + y.ToString() + ", z: " + z.ToString();

                        UnityEngine.Debug.Log("center is:");
                        UnityEngine.Debug.Log(center);
                        UnityEngine.Debug.Log("radius is:");
                        UnityEngine.Debug.Log(radius[0]);

                        //do transform
                        if (VideoPanelApp.cameraToWorldMatrix is not null)
                        {
                            UnityEngine.Debug.Log("video panel array looks like ");
                            //UnityEngine.Debug.Log(VideoPanelApp.cameraToWorldMatrix.Length);
                            //for (int i = 0; i < VideoPanelApp.cameraToWorldMatrix.Length; i++)
                            //{
                            //    UnityEngine.Debug.Log("element _ is");
                            //    UnityEngine.Debug.Log(i);
                            //    UnityEngine.Debug.Log(VideoPanelApp.cameraToWorldMatrix[i]);
                            //}

                            Matrix4x4 mat = ConvertFloatArrayToMatrix4x4(VideoPanelApp.cameraToWorldMatrix);
                            UnityEngine.Debug.Log("convert float array to matrix");
                            p = mat.MultiplyPoint(new Vector3(x, y, -z));
                            x = p[0];
                            y = p[1];
                            z = p[2];

                            if (ConstrainBallY)
                            {
                                Vector3 cameraposition = new Vector3(mat.m03, mat.m13, mat.m23);
                                Vector3 connection = p - cameraposition;
                                if (connection.y != 0)
                                {
                                    float j = ((Goal.transform.position.y - 0.575f) - p.y) / connection.y;
                                    Vector3 constrainedPos = p + j * connection;
                                    vec_pos = constrainedPos;
                                }
                                
                            }
                            else
                            {
                                vec_pos = p;
                            }
                        }

                        //apply
                        //vec_pos.x = x;
                        //vec_pos.y = y;
                        //vec_pos.z = z;
                        ball.transform.position = vec_pos;

                        state += " ball";
                        UnityEngine.Debug.Log("world points are");
                        UnityEngine.Debug.Log(new Vector3(x, y, z));
                        //}
                    }
                }
            }
            yield return new WaitForSeconds(0.0001f);
        }
    }


    public static Matrix4x4 ConvertFloatArrayToMatrix4x4(float[] matrixAsArray)
    {

        // need to invert the transform (maybe):

        //// inv(T) = R'|-R'*t
        //float[] inverted = new float[16];

        //// R component
        //inverted[0] = matrixAsArray[0];
        //inverted[1] = matrixAsArray[4];
        //inverted[2] = matrixAsArray[8];
        //inverted[4] = matrixAsArray[1];
        //inverted[5] = matrixAsArray[5];
        //inverted[6] = matrixAsArray[9];
        //inverted[8] = matrixAsArray[2];
        //inverted[9] = matrixAsArray[6];
        //inverted[10] = matrixAsArray[10];

        //// t component = -R'*t
        //inverted[3] = (-matrixAsArray[0]) * matrixAsArray[3] + (-matrixAsArray[4]) * matrixAsArray[7] + (-matrixAsArray[8]) * matrixAsArray[11];
        //inverted[7] = (-matrixAsArray[1]) * matrixAsArray[3] + (-matrixAsArray[5]) * matrixAsArray[7] + (-matrixAsArray[9]) * matrixAsArray[11];
        //inverted[11] = (-matrixAsArray[2]) * matrixAsArray[3] + (-matrixAsArray[6]) * matrixAsArray[7] + (-matrixAsArray[10]) * matrixAsArray[11];

        //// last row
        //inverted[12] = 0;
        //inverted[13] = 0;
        //inverted[14] = 0;
        //inverted[15] = 1;

        //There is probably a better way to be doing this but System.Numerics.Matrix4x4 is not available 
        //in Unity and we do not include UnityEngine in the plugin.
        Matrix4x4 m = new Matrix4x4();

        // *** replace matrixAsArray w/ inverted ***
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

    public static void FlipTextureVertically(Texture2D original)
    {
        var originalPixels = original.GetPixels();

        var newPixels = new Color[originalPixels.Length];

        var width = original.width;
        var rows = original.height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < rows; y++)
            {
                newPixels[x + y * width] = originalPixels[x + (rows - y - 1) * width];
            }
        }

        original.SetPixels(newPixels);
        original.Apply();
    }
    public static void downsample(Texture2D input, Texture2D output, int skip)
    {
        var originalPixels = input.GetPixels();
        var newPixels = new Color[output.width * output.height];
        UnityEngine.Debug.Log("variable stuff");
        for (var x = 0; x < output.height; x += 1)
        {
            for (var y = 0; y < output.width; y += 1)
            {
                newPixels[y + x * output.width] = originalPixels[x * skip * input.width + skip * y];
                //UnityEngine.Debug.Log(x + y * output.height);
            }
        }
        UnityEngine.Debug.Log("set");
        output.SetPixels(newPixels);
        output.Apply();
        UnityEngine.Debug.Log("applied");
    }
}
