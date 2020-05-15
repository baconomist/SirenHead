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

    private float _stepTimer = -1;
    private float stepStartY;
    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        stepStartY = transform.position.y;
    }

    void FixedUpdate()
    {
        _camera.transform.Rotate(Vector3.up, GameInput.GetCameraRotationX() * xRotSensitivity);

        if (GameInput.GetForward() > 0.5f)
        {
            float stepPercent = Mathf.InverseLerp(0, stepLength, _stepTimer);
            float curveDerivative =
                (stepBounceCurve.Evaluate(stepPercent + 0.01f) - stepBounceCurve.Evaluate(stepPercent)) / 0.01f;
            float moveSpeed = (GameInput.IsSprinting() ? sprintSpeed : movementSpeed);

            // Make character move with step, otherwise continuous movement
            if (linkStepsToMovement)
                transform.position +=
                    transform.forward * Time.deltaTime * moveSpeed * stepMovementCurve.Evaluate(stepPercent);
            else
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            
            // Decide whether step is increasing or decreasing y
            Vector3 stepUp = transform.up * Time.deltaTime * stepBounceCurve.Evaluate(stepPercent) * stepBounceMultiplier;
            if (curveDerivative > 0)
                transform.position += stepUp;
            else
                transform.position -= stepUp;

            // Restart step if finished
            if (stepPercent >= 1)
            {
                _stepTimer = 0;
                transform.position = new Vector3(transform.position.x, stepStartY, transform.position.z);
            }

            _stepTimer += Time.deltaTime * (moveSpeed / movementSpeed);
        }
        else
        {
            _stepTimer = 0;
        }
    }
}