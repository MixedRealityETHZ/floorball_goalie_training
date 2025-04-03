using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.ArucoModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine.Windows.WebCam;
using System;
using Unity.VisualScripting;

namespace OpenCVForUnityExample
{
    /// <summary>
    /// ArUco Example
    /// An example of marker-based AR view and camera pose estimation using the aruco (ArUco Marker Detection) module.
    /// Referring to https://github.com/opencv/opencv_contrib/blob/master/modules/aruco/samples/detect_markers.cpp.
    /// http://docs.opencv.org/3.1.0/d5/dae/tutorial_aruco_detection.html
    /// </summary>
    /// 


    public class ARucoOpenCV : MonoBehaviour
    {


        public VideoPanelApp VideoPanelApp;
        //public pictureeee pictureeee;

        public int markerID = 77;



        //important stuff
        public static Vector3 PositionLeft = new Vector3(0f, 1f, 0f);
        public static Vector3 PositionRight = new Vector3(0f, 1f, 0f);
        public static Vector3 GoalCenter = new Vector3(0f, 1f, 0f);
        public static float GoalAngle = 0;
        public static bool GoalFound = false;
        public static bool RightFound = false;
        public static bool LeftFound = false;
        public static bool CheckForARucoMarkers = false;

        public static bool newPositionFound = false;

        public Transform GoalTrans;

        //public Transform RCube;
        //public Transform LCube;

        /// <summary>
        /// The image texture.
        /// </summary>
        Texture2D imgTexture;

        public static Vector3 CodePosition = new Vector3(0f, 1f, 0f);

        [Space(10)]

        /// <summary>
        /// The dictionary identifier.
        /// </summary>
        public ArUcoDictionary dictionaryId = ArUcoDictionary.DICT_6X6_250;

        /// <summary>
        /// The dictionary id dropdown.
        /// </summary>
        public Dropdown dictionaryIdDropdown;

        /// <summary>
        /// Determines if shows rejected corners.
        /// </summary>
        public bool showRejectedCorners = false;

        /// <summary>
        /// The shows rejected corners toggle.
        /// </summary>
        public Toggle showRejectedCornersToggle;

        /// <summary>
        /// Determines if applied the pose estimation.
        /// </summary>
        public bool applyEstimationPose = true;

        /// <summary>
        /// The length of the markers' side. Normally, unit is meters.
        /// </summary>
        public float markerLength = 0.17f;

        /// <summary>
        /// The AR game object.
        /// </summary>
        public GameObject arGameObject;

        /// <summary>
        /// The AR camera.
        /// </summary>
        public Camera arCamera;

        [Space(10)]

        /// <summary>
        /// Determines if request the AR camera moving.
        /// </summary>
        public bool shouldMoveARCamera = false;

        /// <summary>
        /// The rgb mat.
        /// </summary>
        Mat rgbMat;

        /// <summary>
        /// The texture.
        /// </summary>
        public Texture2D texture;


        //Texture2D projectImage;
        //GameObject quad2;
        //Renderer quadRenderer2;
        //public Shader shader1;

        int updateCounter = 0;
        bool stillRunning = false;


        // Use this for initialization
        void Start()
        {



            StartCoroutine(startWait());

        }


        // Update is called once per frame
        void Update()
        {
            //if(pictureeee.image == true)
            //{
            //    Debug.Log("3hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhtrue");
            //}
            //else
            //{
            //    Debug.Log("3hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhfalse");
            //}


            updateCounter++;
            if (updateCounter == 5)
            {
                stillRunning = false;
            }
            if (updateCounter == 200)
            {
                if (stillRunning == false)
                {
                    Debug.Log("3!!!!!!!!!!!!!!!!!!!!!!!!!!! before restart initiated");
                    StartCoroutine(startCode());
                    Debug.Log("3!!!!!!!!!!!!!!!!!!!!!!!!!!! after restart initiated");
                }
                updateCounter = 0;
            }
        }



        IEnumerator startWait()
        {
            stillRunning = true;
            yield return new WaitForSeconds(3);
            StartCoroutine(startCode());
        }

        IEnumerator startCode()
        {
            Debug.Log("3!!!!!!!!!!!!!!!!!ARuco code started1");
            //projectImage = new Texture2D(picturee.image.width, picturee.image.height);
            ////Debug.LogFormat("webcam:  {0} {1}x{2}", webcam.deviceName, webcam.width, webcam.height);
            //quad2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
            //quadRenderer2 = quad2.GetComponent<Renderer>() as Renderer;
            //quadRenderer2.material = new Material(shader1); // Shader.Find("Custom/Unlit/UnlitTexture")
            //quad2.transform.parent = this.transform;
            //quad2.transform.localPosition = new Vector3(2.0f, 0.0f, 3.0f);

            bool flaggg = true;
            while (flaggg)
            {
                Debug.Log("1!!!!!!!!!!!!!!!!!ARuco code while");
                stillRunning = true;
                if (VideoPanelApp._latestImageBytes.Length > 0)
                {
                    flaggg = false;

                }
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("3!!!!!!!!!!!!!!!!!ARuco code started2");

            yield return new WaitForSeconds(0.5f);

            imgTexture = VideoPanelApp.image;
            FlipTextureVertically(imgTexture);
            Debug.Log("3!!!!!!!!!!!!!!!!!ARuco code started3");
            rgbMat = new Mat(imgTexture.height, imgTexture.width, CvType.CV_8UC3);
            Debug.Log("3!!!!!!!!!!!!!!!!!ARuco code started4");
            texture = new Texture2D(rgbMat.cols(), rgbMat.rows(), TextureFormat.RGBA32, false);
            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            //dictionaryIdDropdown.value = (int)dictionaryId;
            //showRejectedCornersToggle.isOn = showRejectedCorners;

            Debug.Log("3!!!!!!!!!!!!!!!!!ARuco code started5");
            StartCoroutine(loop());
        }

        IEnumerator loop()
        {
            //important stuff
            while (CheckForARucoMarkers)
            {

                Debug.Log("3!!!!!!!!!!!!!!!!!ARuco loop start");
                stillRunning = true;
                Debug.Log("3!!!!!!!!!!!!!!!!!ARuco loop");
                yield return new WaitForSeconds(0.1f);
                DetectMarkers();

            }
        }

        private void DetectMarkers()
        {
            imgTexture = VideoPanelApp.image;
            FlipTextureVertically(imgTexture);

            Utils.texture2DToMat(imgTexture, rgbMat);
            Debug.Log("imgMat dst ToString " + rgbMat.ToString());

            gameObject.transform.localScale = new Vector3(imgTexture.width / 200, imgTexture.height / 200, 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = rgbMat.width();
            float height = rgbMat.height();

            float imageSizeScale = 1.0f;
            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
                imageSizeScale = (float)Screen.height / (float)Screen.width;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }


            // set camera parameters.
            int max_d = (int)Mathf.Max(width, height);
            double fx = max_d;
            double fy = max_d;
            double cx = width / 2.0f;
            double cy = height / 2.0f;
            Mat camMatrix = new Mat(3, 3, CvType.CV_64FC1);
            camMatrix.put(0, 0, fx);
            camMatrix.put(0, 1, 0);
            camMatrix.put(0, 2, cx);
            camMatrix.put(1, 0, 0);
            camMatrix.put(1, 1, fy);
            camMatrix.put(1, 2, cy);
            camMatrix.put(2, 0, 0);
            camMatrix.put(2, 1, 0);
            camMatrix.put(2, 2, 1.0f);
            Debug.Log("camMatrix " + camMatrix.dump());


            MatOfDouble distCoeffs = new MatOfDouble(0, 0, 0, 0);
            Debug.Log("distCoeffs " + distCoeffs.dump());


            // calibration camera matrix values.
            Size imageSize = new Size(width * imageSizeScale, height * imageSizeScale);
            double apertureWidth = 0;
            double apertureHeight = 0;
            double[] fovx = new double[1];
            double[] fovy = new double[1];
            double[] focalLength = new double[1];
            Point principalPoint = new Point(0, 0);
            double[] aspectratio = new double[1];

            Calib3d.calibrationMatrixValues(camMatrix, imageSize, apertureWidth, apertureHeight, fovx, fovy, focalLength, principalPoint, aspectratio);

            Debug.Log("imageSize " + imageSize.ToString());
            Debug.Log("apertureWidth " + apertureWidth);
            Debug.Log("apertureHeight " + apertureHeight);
            Debug.Log("fovx " + fovx[0]);
            Debug.Log("fovy " + fovy[0]);
            Debug.Log("focalLength " + focalLength[0]);
            Debug.Log("principalPoint " + principalPoint.ToString());
            Debug.Log("aspectratio " + aspectratio[0]);

            //VideoByteText.CamMatrix = " imageSize " + imageSize.ToString() + " apertureWidth " + apertureWidth + " apertureHeight " + apertureHeight + " fovx " + fovx[0] + " fovy " + fovy[0] + " focalLength " + focalLength[0] + " principalPoint " + principalPoint.ToString() + " aspectratio " + aspectratio[0];


            // To convert the difference of the FOV value of the OpenCV and Unity. 
            double fovXScale = (2.0 * Mathf.Atan((float)(imageSize.width / (2.0 * fx)))) / (Mathf.Atan2((float)cx, (float)fx) + Mathf.Atan2((float)(imageSize.width - cx), (float)fx));
            double fovYScale = (2.0 * Mathf.Atan((float)(imageSize.height / (2.0 * fy)))) / (Mathf.Atan2((float)cy, (float)fy) + Mathf.Atan2((float)(imageSize.height - cy), (float)fy));

            Debug.Log("fovXScale " + fovXScale);
            Debug.Log("fovYScale " + fovYScale);


            // Adjust Unity Camera FOV https://github.com/opencv/opencv/commit/8ed1945ccd52501f5ab22bdec6aa1f91f1e2cfd4
            //if (widthScale < heightScale)
            //{
            //    arCamera.fieldOfView = (float)(fovx[0] * fovXScale);
            //}
            //else
            //{
            //    arCamera.fieldOfView = (float)(fovy[0] * fovYScale);
            //}
            // Display objects near the camera.
            //arCamera.nearClipPlane = 0.01f;



            Mat ids = new Mat();
            List<Mat> corners = new List<Mat>();
            List<Mat> rejectedCorners = new List<Mat>();
            Mat rvecs = new Mat();
            Mat tvecs = new Mat();
            Mat rotMat = new Mat(3, 3, CvType.CV_64FC1);

            DetectorParameters detectorParams = DetectorParameters.create();
            Dictionary dictionary = Aruco.getPredefinedDictionary((int)dictionaryId);

            // undistort image.
            Calib3d.undistort(rgbMat, rgbMat, camMatrix, distCoeffs);
            // detect markers.
            Aruco.detectMarkers(rgbMat, dictionary, corners, ids, detectorParams, rejectedCorners);


            Debug.Log("3!!!!!!!!!!!!!!!!!ARuco nix gefunden");
            // if at least one marker detected
            //VideoByteText.Markers = "ID size111: " + ids.size();
            if (ids.total() > 0)
            {
                Debug.Log("was gefunden3!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!: " + ids.get(0, 0)[0]);
                Aruco.drawDetectedMarkers(rgbMat, corners, ids, new Scalar(0, 255, 0));
                markerID = (int)ids.get(0, 0)[0];


                //VideoByteText.Markers = "ID size: " + ids.size();
                //for (int i = 0; i < ids.rows(); i++)
                //{
                //    for (int j = 0; j < ids.cols(); j++)
                //    {
                //        VideoByteText.Markers += ">>> i: " + i + " j: " + j + " id: " + (int)ids.get(i, j)[0];
                //    }
                //}





                // estimate pose.
                if (applyEstimationPose)
                {
                    Aruco.estimatePoseSingleMarkers(corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);

                    Debug.Log("groesse der tvecs3!!!!!!!!!!!!!!!!!!!!: " + tvecs.size());
                    Debug.Log("tvec 0,0: " + tvecs.get(0, 0)[0] + " " + tvecs.get(0, 0)[1] + " " + tvecs.get(0, 0)[2]);

                    CodePosition = new Vector3((float)tvecs.get(0, 0)[0], -(float)tvecs.get(0, 0)[1], (float)tvecs.get(0, 0)[2]);
                    //VideoByteText.CodePosition = CodePosition.ToString();


                    //VideoByteText.Goal = " ";

                    //VideoByteText.Markers += "  >!!!>  Position Size: " + tvecs.size();
                    for (int i = 0; i < ids.rows(); i++)
                    {
                        for (int j = 0; j < ids.cols(); j++)
                        {
                            //VideoByteText.Markers += ">>> i: " + i + " j: " + j + " id: " + (float)tvecs.get(i, j)[0] + ", " + (-(float)tvecs.get(i, j)[1]) + ", " + (-(float)tvecs.get(i, j)[2]);
                            Matrix4x4 mat = ConvertFloatArrayToMatrix4x4(VideoPanelApp.cameraToWorldMatrix);


                            if ((int)ids.get(i, j)[0] == 16)
                            {
                                PositionLeft = mat.MultiplyPoint(new Vector3((float)tvecs.get(i, j)[0], -(float)tvecs.get(i, j)[1], -(float)tvecs.get(i, j)[2]));
                                LeftFound = true;
                                //LCube.position = PositionLeft;
                            }
                            if ((int)ids.get(i, j)[0] == 17)
                            {
                                PositionRight = mat.MultiplyPoint(new Vector3((float)tvecs.get(i, j)[0], -(float)tvecs.get(i, j)[1], -(float)tvecs.get(i, j)[2]));
                                RightFound = true;
                                //RCube.position = PositionRight;
                            }

                        }
                    }

                    Vector3 ConnectionVec = new Vector3(0f, 0f, 0f);
                    if (RightFound == true && LeftFound == true)
                    {
                        ConnectionVec = PositionRight - PositionLeft;

                        Vector3 AccountForGoalHight = new Vector3(0f, -0.575f, 0f);
                        Vector3 AccoruntForGoalDepth = 0.05f * Vector3.Cross(ConnectionVec, new Vector3(0f, -1f, 0f)).normalized;

                        GoalCenter = PositionLeft + 0.5f * ConnectionVec + AccountForGoalHight;
                        GoalAngle = (float)(Math.Atan2(-ConnectionVec[2], ConnectionVec[0]) * 180f / Math.PI);

                        GoalTrans.position = GoalCenter;
                        GoalTrans.eulerAngles = new Vector3(0f, GoalAngle, 0f);
                        //Goal
                        //GoalTrans.eulerAngles.y = GoalAngle;

                        GoalFound = true;
                        //VideoByteText.Goal += " Goal Found!!" + " AccoruntForGoalDepth: " + AccoruntForGoalDepth;
                    }

                    //VideoByteText.Goal += "PositionLeft: " + PositionLeft + "PositionRight: " + PositionRight + "GoalCenter: " + GoalCenter + "GoalAngle: " + GoalAngle + "ConnectionVec: " + ConnectionVec[0] + " " + ConnectionVec[1] + " " + ConnectionVec[2];







                    for (int i = 0; i < ids.total(); i++)
                    {


                        //using (Mat rvec = new Mat(rvecs, new OpenCVForUnity.CoreModule.Rect(0, i, 1, 1)))
                        //using (Mat tvec = new Mat(tvecs, new OpenCVForUnity.CoreModule.Rect(0, i, 1, 1)))
                        //{

                        //    // In this example we are processing with RGB color image, so Axis-color correspondences are X: blue, Y: green, Z: red. (Usually X: red, Y: green, Z: blue)
                        //    Calib3d.drawFrameAxes(rgbMat, camMatrix, distCoeffs, rvec, tvec, markerLength * 0.5f);
                        //}

                        //// This example can display the ARObject on only first detected marker.
                        //if (i == 0)
                        //{

                        //    // Get translation vector
                        //    double[] tvecArr = tvecs.get(i, 0);

                        //    // Get rotation vector
                        //    double[] rvecArr = rvecs.get(i, 0);
                        //    Mat rvec = new Mat(3, 1, CvType.CV_64FC1);
                        //    rvec.put(0, 0, rvecArr);

                        //    // Convert rotation vector to rotation matrix.
                        //    Calib3d.Rodrigues(rvec, rotMat);
                        //    double[] rotMatArr = new double[rotMat.total()];
                        //    rotMat.get(0, 0, rotMatArr);

                        //    // Convert OpenCV camera extrinsic parameters to Unity Matrix4x4.
                        //    Matrix4x4 transformationM = new Matrix4x4(); // from OpenCV
                        //    transformationM.SetRow(0, new Vector4((float)rotMatArr[0], (float)rotMatArr[1], (float)rotMatArr[2], (float)tvecArr[0]));
                        //    transformationM.SetRow(1, new Vector4((float)rotMatArr[3], (float)rotMatArr[4], (float)rotMatArr[5], (float)tvecArr[1]));
                        //    transformationM.SetRow(2, new Vector4((float)rotMatArr[6], (float)rotMatArr[7], (float)rotMatArr[8], (float)tvecArr[2]));
                        //    transformationM.SetRow(3, new Vector4(0, 0, 0, 1));
                        //    Debug.Log("transformationM " + transformationM.ToString());

                        //    Matrix4x4 invertYM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
                        //    Debug.Log("invertYM " + invertYM.ToString());

                        //    // right-handed coordinates system (OpenCV) to left-handed one (Unity)
                        //    // https://stackoverflow.com/questions/30234945/change-handedness-of-a-row-major-4x4-transformation-matrix
                        //    Matrix4x4 ARM = invertYM * transformationM * invertYM;

                        //    //if (shouldMoveARCamera)
                        //    //{

                        //    //    ARM = arGameObject.transform.localToWorldMatrix * ARM.inverse;

                        //    //    Debug.Log("ARM " + ARM.ToString());

                        //    //    ARUtils.SetTransformFromMatrix(arCamera.transform, ref ARM);

                        //    //}
                        //    //else
                        //    //{

                        //    //    ARM = arCamera.transform.localToWorldMatrix * ARM;

                        //    //    Debug.Log("ARM " + ARM.ToString());

                        //    //    ARUtils.SetTransformFromMatrix(arGameObject.transform, ref ARM);
                        //    //}
                        //}
                    }
                }
            }

            ////////////if (showRejectedCorners && rejectedCorners.Count > 0)
            ////////////    Aruco.drawDetectedMarkers(rgbMat, rejectedCorners, new Mat(), new Scalar(255, 0, 0));

            ////////////Utils.matToTexture2D(rgbMat, texture);

            //projectImage = texture;
            //quadRenderer2.material.mainTexture = projectImage;
        }

        private void ResetObjectTransform()
        {
            // reset AR object transform.
            Matrix4x4 i = Matrix4x4.identity;
            ARUtils.SetTransformFromMatrix(arCamera.transform, ref i);
            ARUtils.SetTransformFromMatrix(arGameObject.transform, ref i);
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            if (rgbMat != null)
                rgbMat.Dispose();
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("OpenCVForUnityExample");
        }

        /// <summary>
        /// Raises the dictionary id dropdown value changed event.
        /// </summary>
        public void OnDictionaryIdDropdownValueChanged(int result)
        {
            if ((int)dictionaryId != result)
            {
                dictionaryId = (ArUcoDictionary)result;

                ResetObjectTransform();

                DetectMarkers();
            }
        }

        /// <summary>
        /// Raises the show rejected corners toggle value changed event.
        /// </summary>
        public void OnShowRejectedCornersToggleValueChanged()
        {
            if (showRejectedCorners != showRejectedCornersToggle.isOn)
            {
                showRejectedCorners = showRejectedCornersToggle.isOn;

                ResetObjectTransform();

                DetectMarkers();
            }
        }

        public enum ArUcoDictionary
        {
            DICT_4X4_50 = Aruco.DICT_4X4_50,
            DICT_4X4_100 = Aruco.DICT_4X4_100,
            DICT_4X4_250 = Aruco.DICT_4X4_250,
            DICT_4X4_1000 = Aruco.DICT_4X4_1000,
            DICT_5X5_50 = Aruco.DICT_5X5_50,
            DICT_5X5_100 = Aruco.DICT_5X5_100,
            DICT_5X5_250 = Aruco.DICT_5X5_250,
            DICT_5X5_1000 = Aruco.DICT_5X5_1000,
            DICT_6X6_50 = Aruco.DICT_6X6_50,
            DICT_6X6_100 = Aruco.DICT_6X6_100,
            DICT_6X6_250 = Aruco.DICT_6X6_250,
            DICT_6X6_1000 = Aruco.DICT_6X6_1000,
            DICT_7X7_50 = Aruco.DICT_7X7_50,
            DICT_7X7_100 = Aruco.DICT_7X7_100,
            DICT_7X7_250 = Aruco.DICT_7X7_250,
            DICT_7X7_1000 = Aruco.DICT_7X7_1000,
            DICT_ARUCO_ORIGINAL = Aruco.DICT_ARUCO_ORIGINAL,
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
            m.m13 = matrixAsArray[7];       // -0.1f
            m.m20 = matrixAsArray[8];
            m.m21 = matrixAsArray[9];
            m.m22 = matrixAsArray[10];
            m.m23 = matrixAsArray[11];      // + 0.1f
            m.m30 = matrixAsArray[12];
            m.m31 = matrixAsArray[13];
            m.m32 = matrixAsArray[14];
            m.m33 = matrixAsArray[15];

            return m;
        }
    }
}