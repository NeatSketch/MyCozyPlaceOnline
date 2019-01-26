using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    static CharacterMovement instance;

    public Character character;
    
    public static bool CanControl { get; set; }

    Camera mainCam;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (CanControl)
        {
            HandleControlling();
        }
    }

    void HandleControlling()
    {
        if (character)
        {
            Vector2 input = MobileInputController.GetInputAxes();

            if (input.magnitude > 0.001f)
            {
                Vector3 forward = mainCam.transform.forward;
                Vector3 right = mainCam.transform.right;

                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                Vector3 desiredMoveDirection = forward * input.y + right * input.x;

                desiredMoveDirection.Normalize();

                Vector3 move = desiredMoveDirection * character.maxMoveSpeed * Time.deltaTime;

                character.Move(move);
            }
        }
    }

    public static void SetCharacter(Character target)
    {
        if(instance.character != null)
        {
            instance.character.Controllable = false;
        }

        instance.character = target;
        instance.character.Controllable = true;
    }
}