using System;
using DigitalRubyShared;
using SimpleInputNamespace;
using UnityEngine;

[RequireComponent(typeof(FingersScript))]
public class GameInput : MonoBehaviour
{
    public const float ActionTimeout = 0.250f;
    public bool mockMobile = true;
    public float lookThreshold = 10f;
    public float mobileLookSensitivity = 0.05f;

    public static event Action<LongPressGestureRecognizer> OnLongPressEvent;
    public static event Action<PanGestureRecognizer> OnPanEvent;
    public static event Action<TapGestureRecognizer> OnDoubleTapEvent;

    public Joystick moveJoystick;

    private static GameInput _instance;

    public static GameInput Instance
    {
        get
        {
            if (_instance is null)
                _instance = FindObjectOfType<GameInput>();
            if (_instance is null)
                throw new Exception("Please add a GameInput instance to your scene.");
            return _instance;
        }
    }

    private FingersScript _fingersScript;

    private EndAction<PanGestureRecognizer> _panAction;
    private EndAction<LongPressGestureRecognizer> _longPressAction;
    private EndAction<TapGestureRecognizer> _tapAction;

    private bool _runningLookTouch = false;
    private Vector2 _touchDelta;

    private void Start()
    {
        _instance = this;

        _fingersScript = GetComponent<FingersScript>();
        _fingersScript.TreatMousePointerAsFinger = mockMobile;

        TapGestureRecognizer doubleTapGestureRecognizer = new TapGestureRecognizer();
        doubleTapGestureRecognizer.NumberOfTapsRequired = 2;
        doubleTapGestureRecognizer.StateUpdated += OnDoubleTap;

        LongPressGestureRecognizer longPressGestureRecognizer = new LongPressGestureRecognizer();
        longPressGestureRecognizer.MinimumDurationSeconds = 0.1f;
        longPressGestureRecognizer.ThresholdUnits = 5.0f;
        longPressGestureRecognizer.StateUpdated += OnLongPress;

        _fingersScript.AddGesture(longPressGestureRecognizer);
        _fingersScript.AddGesture(doubleTapGestureRecognizer);

        //        Cursor.lockState = CursorLockMode.Locked;

        _longPressAction = new EndAction<LongPressGestureRecognizer>(longPressGestureRecognizer, -999999f);
        _tapAction = new EndAction<TapGestureRecognizer>(doubleTapGestureRecognizer, -999999f);
    }

    private void Update()
    {
        _longPressAction.Update();
        _tapAction.Update();
        _touchDelta = new Vector2(Mathf.Lerp(0, _touchDelta.x, 1 - Time.deltaTime * 10f),
            Mathf.Lerp(0, _touchDelta.y, 1 - Time.deltaTime * 10f));
    }

    private void OnLongPress(GestureRecognizer gestureRecognizer)
    {
        _longPressAction.Reset();

        OnLongPressEvent?.Invoke(gestureRecognizer as LongPressGestureRecognizer);
        
        if (gestureRecognizer.State == GestureRecognizerState.Began && gestureRecognizer.FocusX > Screen.width / 2f)
            _runningLookTouch = true;
        else if (gestureRecognizer.State == GestureRecognizerState.Ended)
            _runningLookTouch = false;

        if (gestureRecognizer.State == GestureRecognizerState.Executing && _runningLookTouch)
            _touchDelta = new Vector2(Mathf.Lerp(_touchDelta.x, gestureRecognizer.DeltaX, Time.deltaTime * 100f),
                Mathf.Lerp(_touchDelta.y, gestureRecognizer.DeltaY, Time.deltaTime * 100f));
    }

    private void OnDoubleTap(GestureRecognizer gestureRecognizer)
    {
        _tapAction.Reset();

        OnDoubleTapEvent?.Invoke(gestureRecognizer as TapGestureRecognizer);
    }

    public static float GetCameraRotationX()
    {
        if ((Input.touchSupported || Instance.mockMobile))
        {
            return Instance._touchDelta.x * _instance.mobileLookSensitivity;
        }

        return Input.GetAxis("Mouse X");
    }

    public static float GetCameraRotationY()
    {
        if ((Input.touchSupported || Instance.mockMobile))
        {
            return -Instance._touchDelta.y * _instance.mobileLookSensitivity;
        }

        return Input.GetAxis("Mouse Y");
    }

    public static float GetForward()
    {
        if ((Input.touchSupported || Instance.mockMobile))
        {
            return Instance.moveJoystick.yAxis.value;
        }

        return 0;
    }

    public static float GetRight()
    {
        if ((Input.touchSupported || Instance.mockMobile))
        {
            return Instance.moveJoystick.xAxis.value;
        }

        return 0;
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