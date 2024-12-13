using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public Transform Camera;

    public float X_Smoothing = 1.5f;
    public float Y_Smoothing = 1.5f;

    public float X_Sensitivity = 0.1f;
    public float Y_Sensitivity = 0.1f;

    Vector2 mouseDelta;
    Vector2 mouseAbsolute, smoothMouse;

    Vector2 camTargetDirection;

    // Start is called before the first frame update
    void Start()
    {
        camTargetDirection = Camera.localRotation.eulerAngles;
        mouseAbsolute.x = transform.localRotation.eulerAngles.y;
        mouseAbsolute.y = Camera.localRotation.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        Quaternion camTargetOrientation = Quaternion.Euler(camTargetDirection);

        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(X_Sensitivity * X_Smoothing, Y_Sensitivity * Y_Smoothing));

        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / X_Smoothing);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / Y_Smoothing);

        mouseAbsolute += smoothMouse;

        mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -90f, 90f);

        Quaternion xRotation = Quaternion.AngleAxis(-mouseAbsolute.y, camTargetOrientation * Vector3.right);
        Camera.localRotation = xRotation;

        Quaternion yRotation = Quaternion.AngleAxis(mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
        transform.localRotation = yRotation;
    }

    public void GetLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

}
