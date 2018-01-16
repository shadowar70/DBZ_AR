using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreenHit : MonoBehaviour {

    [SerializeField]
    private AnimationCurve _OpacityCurve;
    [SerializeField]
    private float _AnimationTime = 1.0f;
    private float _Timer = 0f;
    private Image _Img;

    // Use this for initialization
    void Start() {

        _Img = GetComponent<Image>();

    }

    void OnEnable() {
        _Timer = 0f;
    }

    // Update is called once per frame
    void Update() {

        _Timer += Time.deltaTime;
        if (_Timer > _AnimationTime) {

            this.gameObject.SetActive(false);
        }
        _Img.color = new Color(1, 0, 0, _OpacityCurve.Evaluate(_Timer / _AnimationTime));


    }
}

