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

        webcam = new VideoCapture(0);
        CvInvoke.WaitKey(0); 
    }

    // Update is called once per frame
    void Update() {
        //SEUILLAGE Mme DOUBLE
        //Hsv seuilBas = new Hsv(10, 20, 20);
        //Hsv seuilHaut = new Hsv(30, 255, 255);

        //SEUILLAGE BOUBOULE

        Mat image;
        image = webcam.QueryFrame();
        CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Hsv);
        //CvInvoke.MedianBlur(image, image, 5);
        //CvInvoke.GaussianBlur(image, image, new Size(5,5), 10);
        //CvInvoke.Blur(image, image, new Size(11, 11), new Point(10));

        Image<Hsv, Byte> img = image.ToImage<Hsv, Byte>();

        //Left Bin Image
        //Hsv seuilBas = new Hsv(30, 80, 80);
        Hsv seuilBas = new Hsv(leftHMin, leftSMin, leftVMin);
        //Hsv seuilHaut = new Hsv(120, 255, 255);
        Hsv seuilHaut = new Hsv(leftHMax, leftSMax, leftVMax);
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
            if(i == 0) {
                longMaxContour = CvInvoke.ContourArea(contourObject[i]);
                indexBestContour = 0;
            }
            else {

                if(longMaxContour < CvInvoke.ContourArea(contourObject[i])) {
                    longMaxContour = CvInvoke.ContourArea(contourObject[i]);
                    indexBestContour = i;
                }
            }

        }
        if (indexBestContour > -1) {
            double area = CvInvoke.ContourArea(contourObject[indexBestContour]);
            leftZ = area / (image.Width * image.Height) * zNormalFactor;
        } else {
            leftZ = -1;
        }     

        CvInvoke.Imshow("Mon Image HSV", imageBinLeft);
        if (contourObject.Size > 0)
            CvInvoke.DrawContours(img, contourObject, indexBestContour, colorConst, 2);
        //CvInvoke.Imshow("Mon Image de base", img);

        //Right Bin Image
        //Hsv seuilBas = new Hsv(30, 80, 80);
        seuilBas = new Hsv(rightHMin, rightSMin, rightVMin);
        //Hsv seuilHaut = new Hsv(120, 255, 255);
        seuilHaut = new Hsv(rightHMax, rightSMax, rightVMax);
        Mat imageBinRight = img.InRange(seuilBas, seuilHaut).Mat;

        CvInvoke.Erode(imageBinRight, imageBinRight, structuringElement, new Point(operationSize, operationSize), 3, BorderType.Default, constante);
        CvInvoke.Dilate(imageBinRight, imageBinRight, structuringElement, new Point(operationSize, operationSize), 3, BorderType.Default, constante);
        CvInvoke.FindContours(imageBinRight, contourObject, null, RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);

        for (int i = 0; i < contourObject.Size; i++) {
            if (i == 0) {
                longMaxContour = CvInvoke.ContourArea(contourObject[i]);
                indexBestContour = 0;
            }
            else {

                if (longMaxContour < CvInvoke.ContourArea(contourObject[i])) {
                    longMaxContour = CvInvoke.ContourArea(contourObject[i]);
                    indexBestContour = i;
                }
            }

        }

        if (useStub) {
            SetStubOutput();
        } else {
            SetOutput(image, contourObject, indexBestContour);
        }

        CvInvoke.Imshow("Mon Image 2 HSV", imageBinRight);
        if (contourObject.Size > 0)
        CvInvoke.DrawContours(img, contourObject, indexBestContour, colorConst, 2);
        CvInvoke.Imshow("Mon Image de base", img);
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

    private void SetOutput(Mat image, VectorOfVectorOfPoint contourObject, int indexBestContour) {
        if (indexBestContour > -1) {
            double area = CvInvoke.ContourArea(contourObject[indexBestContour]);
            rightZ = area / (image.Width * image.Height) * zNormalFactor;
        } else {
            rightZ = -1;
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
