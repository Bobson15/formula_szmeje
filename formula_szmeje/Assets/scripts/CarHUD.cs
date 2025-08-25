using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarHUD : MonoBehaviour
{
    public Rigidbody carRb;
    public TMP_Text speedText;
    public Transform cameraTransform;

    [Header("Shake Settings")]
    public float shakeIntensity = 0.05f;
    public float shakeSpeed = 20f;

    private Vector3 initialCamPos;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        initialCamPos = cameraTransform.localPosition;
    }

    void Update()
    {
        if (carRb == null) return;

        float speedKmh = carRb.velocity.magnitude * 3.6f;
        speedText.text = Mathf.RoundToInt(speedKmh) + " km/h";

        float normalizedSpeed = Mathf.Clamp01(speedKmh / 300f);

        float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity * normalizedSpeed;
        float offsetY = Mathf.Cos(Time.time * shakeSpeed * 1.2f) * shakeIntensity * normalizedSpeed;

        cameraTransform.localPosition = initialCamPos + new Vector3(offsetX, offsetY, 0);
    }
}
