using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public bool canControl = true;
    public CharacterController characterController;
    public float maxMoveSpeed = 6f;    

    [Header("Visual")]
    public Transform characterVisual;
    public Animator characterAnimator;

    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

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
        Vector2 input = MobileInputController.GetInputAxes();       

        if (input != Vector2.zero)
        {
            Vector3 forward = mainCam.transform.forward;
            Vector3 right = mainCam.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * input.y + right * input.x;
            characterController.Move(desiredMoveDirection * maxMoveSpeed * Time.deltaTime);

            Quaternion visualRotation = Quaternion.LookRotation(desiredMoveDirection);

            characterVisual.localRotation = visualRotation;
        }
    }

    private void HandleNetworkPlayer()
    {
        
    }
}