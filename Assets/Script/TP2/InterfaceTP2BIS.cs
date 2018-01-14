using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceTP2BIS : MonoBehaviour {

    VideoCapture webcam;
    bool isOpen;
    public MemoryStream memstream = new MemoryStream();
    public RawImage RIWebcam;
    public RawImage RIFeedback;
    Texture2D textureCam;

    private CascadeClassifier frontFaceClassifier;
    private CascadeClassifier frontEyesClassifier;
    private string _absolutePathClassifier = "\\haarcascade_eye.xml";
    private Rectangle[] frontFaces;
    private Rectangle[] frontEyes;
    private int MIN_FACE_SIZE = 50;
    private int MAX_FACE_SIZE = 200;

    // Use this for initialization
    void Start() {
        frontFaceClassifier = new CascadeClassifier("Assets\\haarcascades\\haarcascade_frontalface_default.xml");
        frontEyesClassifier = new CascadeClassifier("Assets\\haarcascades\\haarcascade_eye.xml");
        webcam = new VideoCapture(0);
        RIWebcam.rectTransform.sizeDelta = new Vector2(webcam.Width/2, webcam.Height/2);
        RIFeedback.rectTransform.sizeDelta = new Vector2(webcam.Width/2, webcam.Height/2);
        textureCam = new Texture2D(webcam.Width, webcam.Height);
        isOpen = webcam.IsOpened;
        webcam.ImageGrabbed += new EventHandler(_handleWebcamQueryFrame);
        CvInvoke.WaitKey(0);
       
    }

    // Update is called once per frame
    void Update() {

        if (isOpen) {
            webcam.Grab();
            //SEUILLAGE Mme DOUBLE
            //Hsv seuilBas = new Hsv(10, 20, 20);
            //Hsv seuilHaut = new Hsv(30, 255, 255);



            ////SEUILLAGE BOUBOULE
            //Hsv seuilBas = new Hsv(30, 80, 80);
            //Hsv seuilHaut = new Hsv(120, 255, 255);
            //int operationSize = 1;
            //Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(2 * operationSize + 1, 2 * operationSize + 1), new Point(operationSize, operationSize));
            //MCvScalar constante = new MCvScalar(10);
            //MCvScalar colorConst = new MCvScalar(100, 255, 100);
            //VectorOfVectorOfPoint contourObject = new VectorOfVectorOfPoint();
            //int indexBestContour = -1;

            //Mat image;
            //image = webcam.QueryFrame();
            //CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Hsv);
            //CvInvoke.MedianBlur(image, image, 5);
            ////CvInvoke.GaussianBlur(image, image, new Size(5,5), 10);
            ////CvInvoke.Blur(image, image, new Size(11, 11), new Point(10));

            //Image<Hsv, Byte> img = image.ToImage<Hsv, Byte>();
            //Mat imageModifi = img.InRange(seuilBas, seuilHaut).Mat;

            //CvInvoke.Erode(imageModifi, imageModifi, structuringElement, new Point(operationSize, operationSize), 3, BorderType.Default, constante);
            //CvInvoke.Dilate(imageModifi, imageModifi, structuringElement, new Point(operationSize, operationSize), 3, BorderType.Default, constante);
            //CvInvoke.FindContours(imageModifi, contourObject, null, RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);


            //CvInvoke.Imshow("Mon Image HSV", imageModifi);
            //CvInvoke.DrawContours(img, contourObject, indexBestContour, colorConst, 2);
            //CvInvoke.Imshow("Mon Image de base", img);



            //CvInvoke.Resize(image, image, new Size(1280, 720));

            //writer.Write(image);
        }
        else {
            Debug.Log("caca");
        }

    }

    private void _handleWebcamQueryFrame(object sender, EventArgs e) {
        Mat image = new Mat();
        MCvScalar colorConst = new MCvScalar(100, 255, 100);
        MCvScalar colorConst2 = new MCvScalar(80, 255, 80);
        if (webcam.IsOpened) {
            
            webcam.Retrieve(image);
            
            //CvInvoke.CvtColor(image, image, ColorConversion.Bgra2Gray);
            frontFaces = frontFaceClassifier.DetectMultiScale(image, 1.1, 5, new Size(MIN_FACE_SIZE, MIN_FACE_SIZE), new Size(MAX_FACE_SIZE, MAX_FACE_SIZE));
            frontEyes = frontEyesClassifier.DetectMultiScale(image, 1.1, 5, new Size(MIN_FACE_SIZE-40, MIN_FACE_SIZE-40), new Size(MAX_FACE_SIZE, MAX_FACE_SIZE));
            Debug.Log("Nb Faces:" + frontFaces.Length);
            if (frontFaces.Length > 0) {
                for (int i = 0; i < frontFaces.Length; i++) {
                    CvInvoke.Rectangle(image, frontFaces[i], colorConst, 2);
                }
            }
            if (frontEyes.Length > 0) {
                for (int i = 0; i < frontEyes.Length; i++) {
                    CvInvoke.Rectangle(image, frontEyes[i], colorConst2, 1);
                }
            }
            CvInvoke.Flip(image, image, FlipType.Horizontal);
            CvInvoke.Imshow("Mon Image de base", image);

            RIWebcam.texture = ImageToTexture(image);
            CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Hsv);
            RIFeedback.texture = ImageToTexture(image);
        }
        if (image.IsEmpty) {
            return;
        }

        GC.Collect();

    }

    Texture ImageToTexture(Mat matImage) {
        memstream.Flush();
        memstream.Close();
        memstream.Dispose();
        memstream = new MemoryStream();
        matImage.Bitmap.Save(memstream, matImage.Bitmap.RawFormat);
        textureCam.LoadImage(memstream.ToArray());
        return textureCam;
    }

    private void OnDestroy() {
        CvInvoke.DestroyAllWindows();
    }


}
