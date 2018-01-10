using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBehaviour : MonoBehaviour {
    private InterfaceTP2 interfaceTP2;

    int timer;
    double longMaxContour = 0;
    Boolean isKameha;
    public GameObject prefabBall;
    public GameObject spawner;
    public double seuilmin;

    public AudioClip fxFire1;
    public AudioClip fxFire2;
    public AudioClip fxFire3;

    public GameObject soundManager;

    double lastArea = 0;

    // Use this for initialization
    void Start () {
        interfaceTP2 = GetComponent<InterfaceTP2>();
        timer = 0;
        isKameha = false;
    }

    // Update is called once per frame
    void Update () {
        longMaxContour = interfaceTP2.leftZ;

        if (lastArea != -1 &&  longMaxContour - lastArea > 0.3 && !isKameha) {
            Debug.Log("tmp : " + lastArea);
            Debug.Log("contour : " + longMaxContour);
            Debug.Log("splosh");
            isKameha = true;
            Debug.Log("i" + isKameha);
            // Lancer l'attaque
            BallAttack();
        }
        if (longMaxContour < 2500 && isKameha) {
            isKameha = false;
            Debug.Log("o" + isKameha);
        }

        lastArea = longMaxContour;
    }

    void BallAttack() {

        Instantiate(prefabBall, spawner.transform.position, Quaternion.Euler(0, -90, 0));
        AudioClip[] soundTab = { fxFire1, fxFire2, fxFire3 };
        soundManager.GetComponent<SoundManager>().RandomizeSfx(soundTab);

    }


}
