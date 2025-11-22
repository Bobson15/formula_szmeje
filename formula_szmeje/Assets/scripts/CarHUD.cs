using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarHUD : MonoBehaviour
{
    public Rigidbody carRb;
    public TMP_Text speedText;
    public Transform cameraTransform;


    void Update()
    {
        if (carRb == null) return;

        float speedKmh = carRb.velocity.magnitude * 3.6f;
        speedText.text = Mathf.RoundToInt(speedKmh) + " km/h";
    }
}
