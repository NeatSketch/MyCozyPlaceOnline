using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public bool controllable;
    public float maxMoveSpeed = 6f;
    public Transform characterVisual;
    public CharacterController characterController;
    public Animator characterAnimator;

    public float LERP_MOVE_TIME = 0.2f;
    public float LERP_MAX_DISTANCE = 3f;

    Vector3 targetPosition;
    Vector3 velocity;

    Coroutine lerpRoutine;

    private void Start()
    {
        oldPosition = transform.position;
    }

    private void Update()
    {
        if (!controllable && lerpRoutine == null)
        {
            Vector3 oldPos = transform.position;
            characterController.Move(velocity);
            UpdateRotation(oldPos, transform.position);
        }

        Vector3 delta = transform.position - oldPosition;
        CurrentVelocity = delta / Time.deltaTime;
        oldPosition = transform.position;
    }

    void UpdateRotation(Vector3 oldPos, Vector3 newPos)
    {
        Vector3 desiredMoveDirection = newPos - oldPos;
        if (desiredMoveDirection != Vector3.zero)
        {
            Quaternion visualRotation = Quaternion.LookRotation(desiredMoveDirection, transform.up);
            characterVisual.localRotation = visualRotation;
        }
    }

    void LerpMove()
    {
        if(lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
        }

        lerpRoutine = StartCoroutine(LerpMove_Routine());
    }

    IEnumerator LerpMove_Routine()
    {
        if (Vector3.Distance(transform.position, targetPosition) < LERP_MAX_DISTANCE)
        {
            Vector3 startPos = transform.position;

            for (float _animTime = 0; _animTime < LERP_MOVE_TIME; _animTime += Time.deltaTime)
            {
                float t = _animTime / LERP_MOVE_TIME;

                Vector3 newPos = Vector3.Lerp(startPos, targetPosition, t);
                UpdateRotation(transform.position, newPos);
                transform.position = newPos;
                yield return null;
            }
        }

        transform.position = targetPosition;
  
        lerpRoutine = null;
    }

    public void TeleportTo(Vector3 newWorldPos)
    {
        transform.position = newWorldPos;
        targetPosition = transform.position;
    }

    public void MoveTo(Vector3 newWorldPos)
    {
        targetPosition = newWorldPos;
        LerpMove();
    }

    public Vector3 CurrentVelocity
    {
        private set;
        get;
    }

    private Vector3 oldPosition;

    public void Move(Vector3 motion)
    {
        Vector3 oldPos = transform.position;

        characterController.Move(motion);

        UpdateRotation(oldPos, transform.position);
    }
}
