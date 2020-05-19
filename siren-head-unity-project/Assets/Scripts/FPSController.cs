using System;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float sprintSpeed = 4f;
    public float movementSpeed = 2f;
    public float stepBounceMultiplier = 1f;
    public AnimationCurve stepBounceCurve = AnimationCurve.Linear(0, 0, 0.5f, 1);
    public AnimationCurve stepMovementCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float stepLength = 1f;
    public bool linkStepsToMovement = false;
    public float xRotSensitivity = 1f;
    public float yRotSensitivity = 1f;

    public Camera camera;

    public bool enabled = true;

    private float _stepTimer = -1;
    private float _stepStartY;
    private float _prevDerivative = 0;

    public static event Action OnFootstep;

    void Start()
    {
        _stepStartY = transform.position.y;
    }

    void FixedUpdate()
    {
        if (enabled)
        {
            transform.Rotate(Vector3.up, GameInput.GetCameraRotationX() * xRotSensitivity);
            camera.transform.Rotate(Vector3.right, GameInput.GetCameraRotationY() * yRotSensitivity);

            if (Mathf.Abs(GameInput.GetForward()) > 0.5f)
            {
                float stepPercent = Mathf.InverseLerp(0, stepLength, _stepTimer);
                float curveDerivative =
                    (stepBounceCurve.Evaluate(stepPercent + 0.01f) - stepBounceCurve.Evaluate(stepPercent)) / 0.01f;
                float moveSpeed = (GameInput.IsSprinting() ? sprintSpeed : movementSpeed) * GameInput.GetForward();

                // Make character move with step, otherwise continuous movement
                if (linkStepsToMovement)
                    transform.position +=
                        transform.forward * Time.deltaTime * moveSpeed * stepMovementCurve.Evaluate(stepPercent);
                else
                    transform.position += transform.forward * Time.deltaTime * moveSpeed;

                // Decide whether step is increasing or decreasing y
                Vector3 stepUp = transform.up * Time.deltaTime * stepBounceCurve.Evaluate(stepPercent) *
                                 stepBounceMultiplier;
                if (curveDerivative > 0)
                    transform.position += stepUp;
                else
                    transform.position -= stepUp;

                if (_prevDerivative > 0 && curveDerivative < 0)
                    OnFootstep();

                // Restart step if finished
                if (stepPercent >= 1)
                {
                    _stepTimer = 0;
                    transform.position = new Vector3(transform.position.x, _stepStartY, transform.position.z);
                }

                _stepTimer += Time.deltaTime * (Mathf.Abs(moveSpeed) / movementSpeed);
                
                _prevDerivative = curveDerivative;
            }
            else
            {
                _stepTimer = 0;
            }
        }
    }
}