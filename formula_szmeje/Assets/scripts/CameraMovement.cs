using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    private CameraPosition cameraPosition = CameraPosition.Front;
    private PlayerInput playerInput;
    private Transform transform;
    private Camera camera;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        transform = GetComponent<Transform>();
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        if(cameraPosition == CameraPosition.Front)
        {
            if (playerInput.actions["Back"].triggered)
            {
                cameraPosition = CameraPosition.Back;
                transform.Rotate(new Vector3(0, 180, 0));
                transform.localPosition = new Vector3(0, 0.65f, -0.4f);
            }
            else if (playerInput.actions["Right"].triggered)
            {
                cameraPosition = CameraPosition.Right;
                transform.Rotate(new Vector3(0, 90, 0));
                camera.fieldOfView = 85;
            }
            else if (playerInput.actions["Left"].triggered)
            {
                cameraPosition = CameraPosition.Left;
                transform.Rotate(new Vector3(0, -90, 0));
                camera.fieldOfView = 85;
            }
        }
        else if(cameraPosition == CameraPosition.Back && !playerInput.actions["Back"].IsPressed())
        {
            cameraPosition = CameraPosition.Front;
            transform.Rotate(new Vector3(0, -180, 0));
            transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        else if (cameraPosition == CameraPosition.Right && !playerInput.actions["Right"].IsPressed())
        {
            cameraPosition = CameraPosition.Front;
            transform.Rotate(new Vector3(0, -90, 0));
            camera.fieldOfView = 78;
        }
        else if (cameraPosition == CameraPosition.Left && !playerInput.actions["Left"].IsPressed())
        {
            cameraPosition = CameraPosition.Front;
            transform.Rotate(new Vector3(0, 90, 0));
            camera.fieldOfView = 78;
        }
    }
    private enum CameraPosition
    {
        Front,
        Right,
        Back,
        Left
    }
}
