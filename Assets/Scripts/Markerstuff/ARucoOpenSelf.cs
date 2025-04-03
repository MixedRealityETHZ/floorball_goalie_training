using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.ArucoModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;


namespace OpenCVForUnityExample
{
    public class ARucoOpenSelf : MonoBehaviour
    {

        public pictureeee pictureeee;
        public Texture2D texture;

        DetectorParameters detectorParameters;
        Dictionary dictionary;
        Mat ids = new Mat();
        List<Mat> corners = new List<Mat>();
        List<Mat> rejectedCorners = new List<Mat>();


        //Mat matt;



        // Start is called before the first frame update
        void Start()
        {
            detectorParameters = DetectorParameters.create();                                       // Create default parameres for detection
            dictionary = Aruco.getPredefinedDictionary(Aruco.DICT_6X6_250);

            
            StartCoroutine(loop());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator loop()
        {

            while (true)
            {
                if (pictureeee.image)
                {
                    //get marker data
                    Mat mat = new Mat(pictureeee.image.height, pictureeee.image.width, CvType.CV_8UC3);
                    Utils.texture2DToMat(pictureeee.image, mat);

                    //Mat mat = OpenCvSharp.Unity.TextureToMat(pictureeee.image);

                    Mat grayMat = new Mat();
                    Imgproc.cvtColor(mat, grayMat, Imgproc.COLOR_BGR2GRAY);
                    //Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);



                    //CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
                    Aruco.detectMarkers(grayMat, dictionary, corners, ids, detectorParameters, rejectedCorners);

                    if (ids.cols() > 0)
                    {
                        Debug.Log("neu: " + ids.get(0, 0)[0]);
                        string print = " ";
                        for (int i = 0; i < 4; i++)
                        {
                            print = print + corners[0].get(0, i)[0].ToString() + " " + corners[0].get(0, i)[1].ToString() + " ";
                        }
                        Debug.Log(print);
                    }
                    else
                    {
                        Debug.Log("neu: nix gefunden");
                    }

                    //Aruco.estimatePoseSingleMarkers(corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);



                    //calc position
                    //OpenCVForUnity.ArucoModule.Aruco.estimatePoseSingleMarkers(corners, markerLength, cameraMatrix, distortionCoefficients, rvec, tvec, objPoints);













                }
                else
                {
                    Debug.Log("no Image yet");
                }


                yield return new WaitForSeconds(5f);
            }
        }
    }
}
