using System;
using DigitalRubyShared;
using SimpleInputNamespace;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FingersScript))]
public class GameInput : MonoBehaviour
{
    public const float ActionTimeout = 0.250f;
    public bool mobileControls = true;
    public float mobileLookSensitivity = 0.05f;
    public Slider slider;

    public Joystick moveJoystick;

    private static GameInput _instance;

    public static GameInput Instance
    {
        get
        {
            if (_instance is null)
                throw new Exception("Please add a GameInput instance to your scene.");
            return _instance;
        }
    }

    private FingersScript _fingersScript;

    private EndAction<PanGestureRecognizer> _panAction;
    private EndAction<LongPressGestureRecognizer> _longPressAction;

    private GestureTouch _lookTouch;

    private bool _runningLookTouch = false;
    private Vector2 _touchDelta;

    public void OnMobileSensitivityChanged()
    {
        float min = 0.01f;
        float max = 0.1f;

        mobileLookSensitivity = Mathf.Lerp(min, max, slider.value / 100f);
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _fingersScript = GetComponent<FingersScript>();
        _fingersScript.TreatMousePointerAsFinger = mobileControls;

        LongPressGestureRecognizer lookGestureRecognizer = new LongPressGestureRecognizer();
        lookGestureRecognizer.MinimumDurationSeconds = 0.1f;
        lookGestureRecognizer.ThresholdUnits = 5.0f;
        lookGestureRecognizer.StateUpdated += OnLookLongPress;

        _fingersScript.AddGesture(lookGestureRecognizer);

        //        Cursor.lockState = CursorLockMode.Locked;

        _longPressAction = new EndAction<LongPressGestureRecognizer>(lookGestureRecognizer, -999999f);
    }

    private void Update()
    {
        _longPressAction.Update();
        _touchDelta = new Vector2(Mathf.Lerp(0, _touchDelta.x, 1 - Time.deltaTime * 10f),
            Mathf.Lerp(0, _touchDelta.y, 1 - Time.deltaTime * 10f));
    }

    private void OnLookLongPress(GestureRecognizer gestureRecognizer)
    {
        _longPressAction.Reset();

        if (gestureRecognizer.State == GestureRecognizerState.Began && gestureRecognizer.FocusX > Screen.width / 2f)
            _runningLookTouch = true;
        else if (gestureRecognizer.State == GestureRecognizerState.Ended)
            _runningLookTouch = false;
        else if (gestureRecognizer.State == GestureRecognizerState.Began && gestureRecognizer.FocusX < Screen.width / 2f)
        {
            gestureRecognizer.Reset();
        }

        if (gestureRecognizer.State == GestureRecognizerState.Executing && _runningLookTouch)
            _touchDelta = new Vector2(Mathf.Lerp(_touchDelta.x, gestureRecognizer.DeltaX, Time.deltaTime * 100f),
                Mathf.Lerp(_touchDelta.y, gestureRecognizer.DeltaY, Time.deltaTime * 100f));
    }

    public static float GetCameraRotationX()
    {
        if (Instance.mobileControls)
        {
            return Instance._touchDelta.x * _instance.mobileLookSensitivity;
        }

        return Input.GetAxis("Mouse X");
    }

    public static float GetCameraRotationY()
    {
        if (Instance.mobileControls)
        {
            return -Instance._touchDelta.y * _instance.mobileLookSensitivity;
        }

        return -Input.GetAxis("Mouse Y");
    }

    public static float GetForward()
    {
        if (Instance.mobileControls)
        {
            return Instance.moveJoystick.yAxis.value;
        }

        return Input.GetAxis("Vertical");
    }

    public static float GetRight()
    {
        if (Instance.mobileControls)
        {
            return Instance.moveJoystick.xAxis.value;
        }

        return Input.GetAxis("Horizontal");
        ;
    }

    public static bool IsSprinting()
    {
        // Always sprint on mobile?
        return true;
    }

    class EndAction<T>
    {
        public float StartTime;
        public readonly T Data;
        public bool Running;

        public EndAction(T data, float startTime = float.NaN)
        {
            Data = data;
            if (float.IsNaN(startTime))
                StartTime = Time.time;
            Running = true;
        }

        public void Update()
        {
            if (Time.time - StartTime >= ActionTimeout)
                Running = false;
        }

        public void Reset()
        {
            Running = true;
            StartTime = Time.time;
        }
    }
}