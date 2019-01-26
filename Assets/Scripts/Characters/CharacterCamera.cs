using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    public enum Mode
    {
        Target,
        Free
    }

    public Mode cameraMode;
    public CharacterController targetCharacter;
    public Transform cameraItself;

    public float cameraAngle = 45f;
    public float cameraRotateSpeed = 10f;
    public float cameraMoveSpeed = 10f;
    public float cameraDistance = 2f;

    void LateUpdate()
    {
        bool rotateCamLeft = Input.GetButton("RotateCameraLeft")
            || MobileInputController.GetButton("RotateCameraLeft");
        bool rotateCamRight = Input.GetButton("RotateCameraRight")
            || MobileInputController.GetButton("RotateCameraRight");

        float camRotation = transform.rotation.eulerAngles.y;

        if (rotateCamLeft)
        {
            camRotation += cameraRotateSpeed * Time.deltaTime;
        }

        if (rotateCamRight)
        {
            camRotation -= cameraRotateSpeed * Time.deltaTime;
        }

        Quaternion camRot = Quaternion.Euler(cameraAngle, camRotation, 0);
        Vector3 camPos = new Vector3(0, 0, -cameraDistance);

        Vector3 targetPos = transform.position;

        if(cameraMode == Mode.Target)
        {
            if (targetCharacter)
            {
                targetPos = targetCharacter.transform.position;
            }
        }
        else
        {
            Vector2 input = MobileInputController.GetInputAxes();

            Vector3 forward = cameraItself.transform.forward;
            Vector3 right = cameraItself.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * input.y + right * input.x;
            Vector3 move = desiredMoveDirection * cameraMoveSpeed * Time.deltaTime;

            targetPos += move;
        }
        
        transform.position = targetPos;
        transform.localRotation = camRot;

        cameraItself.transform.localPosition = camPos;
    }
}
