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
    private int MIN_FACE_SIZE = 180;
    private int MAX_FACE_SIZE = 300;

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

    public float faceAreaMin;
    public float faceAreaMid;
    public float faceAreaMax;
    public float headThreshold;

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
        CvInvoke.Flip(image, image, FlipType.Horizontal);
        CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Hsv);

        CvInvoke.MedianBlur(image, image, 5);
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
        DetectFace(image);

        CvInvoke.Imshow("Mon Image de base HSV", img);
    }

    private void DetectFace(Mat image) {
        Rectangle[] frontFaces = frontFaceClassifier.DetectMultiScale(image, 1.1, 5, new Size(MIN_FACE_SIZE, MIN_FACE_SIZE), new Size(MAX_FACE_SIZE, MAX_FACE_SIZE));

        int indBestFace = -1;
        if (frontFaces.Length > 0) {
            indBestFace = 0;
            float bestArea = frontFaces[0].Height * frontFaces[0].Width;
            for (int i = 1; i < frontFaces.Length; i++) {
                float curArea = frontFaces[i].Height * frontFaces[i].Width;
                if (curArea > bestArea) {
                    bestArea = curArea;
                    indBestFace = i;
                }
            }
        }

        if (indBestFace > -1) {
            // X Y
            float w = frontFaces[indBestFace].Width;
            float h = frontFaces[indBestFace].Height;
            float x = (float) frontFaces[indBestFace].Left + (float) frontFaces[indBestFace].Width / 2;
            float y = (float) frontFaces[indBestFace].Top + (float) frontFaces[indBestFace].Height / 2;
            CvInvoke.Circle(image, new Point((int) x, (int) y), 5, new MCvScalar(100, 255, 100), 2);
            headX = Mathf.Clamp((((float) x / ((float)image.Width)) - 0.5f) * 5f, -1f, 1);

            // Z
            float area =  w * h;
            CvInvoke.Rectangle(image, frontFaces[indBestFace], new MCvScalar(100, 255, 100), 2);

            if (area > faceAreaMid + headThreshold) {
                headZ = Mathf.Min(1, (area - faceAreaMid) / (faceAreaMax - faceAreaMid));
            } else if (area < faceAreaMid - headThreshold) {
                headZ = Mathf.Max(-1,  -(area - faceAreaMid) / (faceAreaMin - faceAreaMid));
            } else {
                headZ = 0;
            }
        } else {
            headX = 0;
            headY = 0;
            headZ = 0;
        }

        CvInvoke.Imshow("Mon Image de base", image);
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
