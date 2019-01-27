using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    static CharacterCamera instance;

    public enum Mode
    {
        Character,
        Free
    }

    public Mode cameraMode;
    public Character targetCharacter;
    public Transform cameraItself;

    public float cameraSwitchAnimTime = 1f;
    public float characterCameraAngle = 45f;
    public float editCameraAngle = 60f;
    public float cameraRotateSpeed = 10f;
    public float cameraMoveSpeed = 10f;
    public float cameraDistance = 2f;

    float cameraAngle;
    Coroutine animRoutine;


    private void Awake()
    {
        instance = this;
        cameraAngle = characterCameraAngle;
    }

    void LerAngleAnimated(float from, float to)
    {
        if(animRoutine != null)
        {
            StopCoroutine(animRoutine);
        }

        animRoutine = StartCoroutine(LerpAngleAnimated_Routine(from, to));
    }

    IEnumerator LerpAngleAnimated_Routine(float angleFrom, float angleTo)
    {
        Debug.Log("a");
        for (float _animTime = 0; _animTime < cameraSwitchAnimTime; _animTime += Time.deltaTime)
        {
            float t = _animTime / cameraSwitchAnimTime;

            cameraAngle = Mathf.Lerp(angleFrom, angleTo, t);

            yield return null;
        }

        cameraAngle = angleTo;

        animRoutine = null;
        Debug.Log("b");

    }

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

        if(cameraMode == Mode.Character)
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

    public static void SetTarget(Character character)
    {
        instance.targetCharacter = character;
        instance.cameraMode = Mode.Character;
    }

    public static void ToEditMode()
    {
        //SetTarget(instance.transform.position);
        instance.LerAngleAnimated(instance.cameraAngle, instance.editCameraAngle);
    }

    public static void ToCharacterMode()
    {
       // SetTarget(instance.targetCharacter);
        instance.LerAngleAnimated(instance.cameraAngle, instance.characterCameraAngle);
    }

    public static void SetTarget(Vector3 position)
    {
        //instance.targetCharacter = null;
        instance.cameraMode = Mode.Free;
        instance.transform.position = position;
    }
}
