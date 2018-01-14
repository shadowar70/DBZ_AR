using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public class InterfaceTP2 : MonoBehaviour {

    VideoCapture webcam;
   // int timer;
    double longMaxContour = 0;

    //Classifier
    private CascadeClassifier frontFaceClassifier;

    //Calibration
    public float leftHMin = 0;
    public float leftHMax = 255;
    public float leftSMin = 0;
    public float leftSMax = 255;
    public float leftVMin = 0;
    public float leftVMax = 255;

    public float rightHMin = 0;
    public float rightHMax = 255;
    public float rightSMin = 0;
    public float rightSMax = 255;
    public float rightVMin = 0;
    public float rightVMax = 255;

    //outputs
    public double leftX;
    public double leftY;
    public double leftZ;

    public double rightX;
    public double rightY;
    public double rightZ;

    public double headX;
    public double headY;
    public double headZ;

    public bool useStub = false;

    public double zNormalFactor = 10;

    // Use this for initialization
    void Start() {
        frontFaceClassifier = new CascadeClassifier("Assets\\haarcascades\\haarcascade_frontalface_default.xml");
        webcam = new VideoCapture(0);
        CvInvoke.WaitKey(0); 
    }

    // Update is called once per frame
    void Update() {
        ////SEUILLAGE BOUBOULE
        Mat image;
        image = webcam.QueryFrame();
        CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Hsv);
        //CvInvoke.MedianBlur(image, image, 5);
        //CvInvoke.GaussianBlur(image, image, new Size(5,5), 10);
        //CvInvoke.Blur(image, image, new Size(11, 11), new Point(10));

        Image<Hsv, Byte> img = image.ToImage<Hsv, Byte>();

        //Detect left ball by color
        Hsv seuilBas = new Hsv(leftHMin, leftSMin, leftVMin);
        Hsv seuilHaut = new Hsv(leftHMax, leftSMax, leftVMax);
        DetectBall(image, img, seuilBas, seuilHaut, "Left");

        //Detect right ball by color
        seuilBas = new Hsv(rightHMin, rightSMin, rightVMin);
        seuilHaut = new Hsv(rightHMax, rightSMax, rightVMax);
        DetectBall(image, img, seuilBas, seuilHaut, "Right");

        //Detect face with cascad classifier
        DetectFace();

        CvInvoke.Imshow("Mon Image de base HSV", img);
    }

    private void DetectFace() {
        Debug.Log("TODO");
    }

    private void DetectBall(Mat image, Image<Hsv, byte> img, Hsv seuilBas, Hsv seuilHaut, String suffix) {
        //Left Bin Image
        Mat imageBinLeft = img.InRange(seuilBas, seuilHaut).Mat;

        int operationSize = 1;
        Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(2 * operationSize + 1, 2 * operationSize + 1), new Point(operationSize, operationSize));
        MCvScalar constante = new MCvScalar(10);
        MCvScalar colorConst = new MCvScalar(100, 255, 100);
        VectorOfVectorOfPoint contourObject = new VectorOfVectorOfPoint();
        int indexBestContour = -1;

        CvInvoke.Erode(imageBinLeft, imageBinLeft, structuringElement, new Point(operationSize, operationSize), 3, BorderType.Default, constante);
        CvInvoke.Dilate(imageBinLeft, imageBinLeft, structuringElement, new Point(operationSize, operationSize), 3, BorderType.Default, constante);
        CvInvoke.FindContours(imageBinLeft, contourObject, null, RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);

        for (int i = 0; i < contourObject.Size; i++) {
            if (i == 0) {
                longMaxContour = CvInvoke.ContourArea(contourObject[i]);
                indexBestContour = 0;
            } else {

                if (longMaxContour < CvInvoke.ContourArea(contourObject[i])) {
                    longMaxContour = CvInvoke.ContourArea(contourObject[i]);
                    indexBestContour = i;
                }
            }

        }
        if (indexBestContour > -1) {
            double area = CvInvoke.ContourArea(contourObject[indexBestContour]);
            if (suffix == "Left") {
                leftZ = area / (image.Width * image.Height) * zNormalFactor;
            } else if (suffix == "Right") {
                rightZ = area / (image.Width * image.Height) * zNormalFactor;
            } else {
                throw new NotImplementedException();
            }
        } else {
            if (suffix == "Left") {
                leftZ = -1;
            } else if (suffix == "Right") {
                rightZ = -1;
            } else {
                throw new NotImplementedException();
            }
        }
            CvInvoke.Imshow("Mon Image" + suffix, imageBinLeft);
        if (contourObject.Size > 0) {
            CvInvoke.DrawContours(img, contourObject, indexBestContour, colorConst, 2);
        }
    }

    private void SetStubOutput() {
        leftZ = 0;
        rightZ = 0;
	headZ = 0;
        headX = 0;

        // Right kamea
        if (Input.GetKey(KeyCode.Keypad1)) {
            leftZ = 1;
        }

        // Right kamea
        if (Input.GetKey(KeyCode.Keypad3)) {
            rightZ = 1;
        }

	// Head pos
	if (Input.GetKey(KeyCode.Z)) {
            headZ = 1;
        }
	if (Input.GetKey(KeyCode.S)) {
            headZ = -1;
        }
	if (Input.GetKey(KeyCode.Q)) {
            headX = -1;
        }
	if (Input.GetKey(KeyCode.D)) {
            headX = 1;
        }
    }

    private void OnDestroy() {
        CvInvoke.DestroyAllWindows();
    }

    void DrawLine(VectorOfPoint biggestContour, Image<Hsv, Byte> image) {
        //CENTROID
        Hsv couleurPoint = new Hsv(179, 100, 50);
        MCvMoments moments = CvInvoke.Moments(biggestContour);
        int cx = (int)(moments.M10 / moments.M00);
        int cy = (int)(moments.M01 / moments.M00);
        Point[] centroid = { new Point(cx, cy), new Point(cx, cy) };

        image.Draw(centroid, couleurPoint, 10);

        

    }
    //=======================BOITE A IDEES===============================
    

}
