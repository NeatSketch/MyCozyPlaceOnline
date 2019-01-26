using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public bool canControl = true;
    public CharacterController characterController;
    public float maxMoveSpeed = 6f;

    [Header("Camera")]
    public Transform cameraParent;
    public Transform cameraItself;
    public float cameraAngle = 45f;
    public float cameraRotateSpeed = 10f;
    public float cameraDistance = 2f;

    [Header("Visual")]
    public Transform characterVisual;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canControl)
        {
            HandleControlling();
        }
        else
        {
            HandleNetworkPlayer();
        }
    }   

    void HandleControlling()
    {
        bool rotateCamLeft = Input.GetKey(KeyCode.Q);
        bool rotateCamRight = Input.GetKey(KeyCode.E);

        float moveAxisX = Input.GetAxis("Horizontal");
        float moveAxisY = Input.GetAxis("Vertical");

        float camRotation = cameraParent.transform.rotation.eulerAngles.y;
        
        if(rotateCamLeft)
        {
            camRotation += cameraRotateSpeed * Time.deltaTime; 
        }

        if (rotateCamRight)
        {
            camRotation -= cameraRotateSpeed * Time.deltaTime;
        }

        Quaternion camRot = Quaternion.Euler(cameraAngle, camRotation, 0);
        Vector3 camPos = new Vector3(0, 0, -cameraDistance);

        cameraParent.transform.localRotation = camRot;
        cameraItself.transform.localPosition = camPos;

        if (Mathf.Abs(moveAxisX) > 0.1f || Mathf.Abs(moveAxisY) > 0.1f)
        {
            Vector3 forward = cameraItself.forward;
            Vector3 right = cameraItself.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * moveAxisY + right * moveAxisX;
            characterController.Move(desiredMoveDirection * maxMoveSpeed * Time.deltaTime);

            
            Quaternion visualRotation = Quaternion.LookRotation(desiredMoveDirection);

            characterVisual.localRotation = visualRotation;
        }
    }

    private void HandleNetworkPlayer()
    {
        throw new NotImplementedException();
    }
}