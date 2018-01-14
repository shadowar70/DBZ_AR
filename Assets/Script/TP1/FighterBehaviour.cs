using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBehaviour : MonoBehaviour {
    private InterfaceTP2 interfaceTP2;

    int timer;
    double longMaxContourLeft = 0;
    double longMaxContourRight = 0;
    double MoveSide = 0;
    double MoveFront = 0;
    Boolean isKameha;
    public int speedDash = 200;
    public GameObject prefabBall;
    public GameObject spawnerLeft;
    public GameObject spawnerRight;
    public double seuilmin;

    public AudioClip fxFire1;
    public AudioClip fxFire2;
    public AudioClip fxFire3;

    public GameObject soundManager;
    public GameObject target;

    double lastAreaLeft = 0;
    double lastAreaRight = 0;

    float fireLeftKameha = 0;
    float fireRightKameha = 0;

    // Use this for initialization
    void Start () {
        interfaceTP2 = GetComponent<InterfaceTP2>();
        timer = 0;
        isKameha = false;
    }

    // Update is called once per frame
    void Update () {
        longMaxContourLeft = interfaceTP2.leftZ;
        longMaxContourRight = interfaceTP2.rightZ;
        MoveSide = interfaceTP2.headX;
        MoveFront = interfaceTP2.headZ;

        //MoveWithKey();
        Dash((float)MoveSide, (float)MoveFront);

        if (((lastAreaLeft != -1 && longMaxContourLeft - lastAreaLeft > 0.3) || fireLeftKameha == 1) && !isKameha) {
            isKameha = true;
            //Debug.Log("tmp : " + lastArea);
            //Debug.Log("contour : " + longMaxContour);
            //Debug.Log("splosh");
            //Debug.Log("i" + isKameha);

            // Lancer l'attaque
            BallAttack(1);
        }
        if (longMaxContourLeft < 2500 && isKameha) {
            isKameha = false;
            //Debug.Log("o" + isKameha);
        }

        if (((lastAreaRight != -1 && longMaxContourRight - lastAreaRight > 0.3) || fireRightKameha == 1) && !isKameha) {
            isKameha = true;
            //Debug.Log("tmp : " + lastArea);
            //Debug.Log("contour : " + longMaxContour);
            //Debug.Log("splosh");
            //Debug.Log("i" + isKameha);

            // Lancer l'attaque
            BallAttack(-1);
        }
        if (longMaxContourRight < 2500 && isKameha) {
            isKameha = false;
            //Debug.Log("o" + isKameha);
        }

        lastAreaRight = longMaxContourRight;
        lastAreaLeft = longMaxContourLeft;
    }

    void BallAttack(int fireLeft) {
        GameObject fireBall;
        if(fireLeft == 1) {
            fireBall = Instantiate(prefabBall, spawnerLeft.transform.position, Quaternion.Euler(0, 0, 0));
        }
        else {
            fireBall = Instantiate(prefabBall, spawnerRight.transform.position, Quaternion.Euler(0, 0, 0));
        }
        fireBall.transform.LookAt(new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z));
        AudioClip[] soundTab = { fxFire1, fxFire2, fxFire3 };
        soundManager.GetComponent<SoundManager>().RandomizeSfx(soundTab);

    }

    void Dash(float moveHeadX, float moveHeadZ){

        if(moveHeadX < -0.2) {
            transform.RotateAround(target.transform.position, Vector3.up, -speedDash * Time.deltaTime* moveHeadX);
        }


        if (moveHeadX > 0.2) {
            transform.RotateAround(target.transform.position, Vector3.up, -speedDash * Time.deltaTime* moveHeadX);
        }

        if (moveHeadZ < -0.1) {
            transform.Translate(Vector3.forward * speedDash*2 * Time.deltaTime* moveHeadZ);
        }


        if (moveHeadZ > 0.1) {
            transform.Translate(Vector3.forward * speedDash*2 * Time.deltaTime* moveHeadZ);
        }
    }

    private void MoveWithKey() {
        float moveHeadX = 0;
        float moveHeadZ = 0;
        fireLeftKameha = 0;
        fireRightKameha = 0;

        // Right kamea
        if (Input.GetKey(KeyCode.Keypad1)) {
            fireLeftKameha = 1;
        }

        // Right kamea
        if (Input.GetKey(KeyCode.Keypad3)) {
            fireRightKameha = 1;
        }

        // Head pos
        if (Input.GetKey(KeyCode.Z)) {
            moveHeadZ = 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveHeadZ = -1;
        }
        if (Input.GetKey(KeyCode.Q)) {
            moveHeadX = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveHeadX = 1;
        }

        Dash(moveHeadX, moveHeadZ);
    }
}
