using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public class TestInterface : MonoBehaviour {

    VideoCapture webcam;
    VideoWriter writer;

	// Use this for initialization
	void Start () {
        //Si on a une webcam
        //webcam = new VideoCapture(0);
        webcam = new VideoCapture("D:/Jothenin_gamagora2017/source/repos/InterfaceProject/Assets/Ressources/Trailer.mp4");
        writer = new VideoWriter("D:/Jothenin_gamagora2017/source/repos/InterfaceProject/Assets/Ressources/Trailer_copie.mp4", 30, new Size(1920, 1080), true);
        //string windowName = "FrameWindow";
        //CvInvoke.NamedWindow(windowName);
        CvInvoke.WaitKey(0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Mat image;
        image = webcam.QueryFrame();
        CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Gray);
        CvInvoke.Imshow("Mon Image", image);
        CvInvoke.Resize(image, image, new Size(1920, 1080));
        CvInvoke.Flip(image, image, FlipType.Horizontal);
        writer.Write(image);

	}

    private void OnDestroy() {
        CvInvoke.DestroyAllWindows();
    }
}
