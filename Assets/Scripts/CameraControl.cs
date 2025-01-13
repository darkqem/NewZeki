using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotateSpeed = 10.0f, speedWASD = 10.0f, zoomSpeed = 1000.0f, sensitivity = 0.1f;
    public float mouse_sens = 1f;
    
    private float _mult = 1f;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        _mult = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;

        transform.Translate(new Vector3(horizontal, vertical, 0) * Time.deltaTime * _mult * speedWASD, Space.World);

        transform.position += transform.forward * zoomSpeed * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel");

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -6f, 4f),
            Mathf.Clamp(transform.position.y, 7f, 12f),
            Mathf.Clamp(transform.position.z, 0f, 7f));

        if (Input.GetKey(KeyCode.Mouse1))
        {
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 movement = new Vector3(mouseX * -sensitivity, mouseY * -sensitivity, 0);
        
        transform.position += movement;
    }

    
}
