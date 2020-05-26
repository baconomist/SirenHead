using System;
using DigitalRubyShared;
using SimpleInputNamespace;
using UnityEngine;

[RequireComponent(typeof(FingersScript))]
public class GameInput : MonoBehaviour
{
    public const float ActionTimeout = 0.250f;
    public bool mockMobile = true;

    public static event Action<LongPressGestureRecognizer> OnLongPressEvent;
    public static event Action<PanGestureRecognizer> OnPanEvent;
    public static event Action<TapGestureRecognizer> OnDoubleTapEvent;

    public Joystick moveJoystick;
    public Joystick lookJoystick;

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

    private void Start()
    {
        _fingersScript = GetComponent<FingersScript>();
        _fingersScript.TreatMousePointerAsFinger = mockMobile;

        TapGestureRecognizer doubleTapGestureRecognizer = new TapGestureRecognizer();
        doubleTapGestureRecognizer.NumberOfTapsRequired = 2;
        doubleTapGestureRecognizer.StateUpdated += OnDoubleTap;

        LongPressGestureRecognizer longPressGestureRecognizer = new LongPressGestureRecognizer();
        longPressGestureRecognizer.MinimumDurationSeconds = 0.1f;
        longPressGestureRecognizer.StateUpdated += OnLongPress;

        PanGestureRecognizer panGestureRecognizer = new PanGestureRecognizer();
        panGestureRecognizer.StateUpdated += OnPan;
        
        _fingersScript.AddGesture(longPressGestureRecognizer);
        _fingersScript.AddGesture(panGestureRecognizer);
        _fingersScript.AddGesture(doubleTapGestureRecognizer);

        //        Cursor.lockState = CursorLockMode.Locked;

        _panAction = new EndAction<PanGestureRecognizer>(panGestureRecognizer, -999999f);
        _longPressAction = new EndAction<LongPressGestureRecognizer>(longPressGestureRecognizer, -999999f);
        _tapAction = new EndAction<TapGestureRecognizer>(doubleTapGestureRecognizer, -999999f);
    }

    private void Update()
    {
        _panAction.Update();
        _longPressAction.Update();
        _tapAction.Update();
    }

    private void OnLongPress(GestureRecognizer gestureRecognizer)
    {
        _longPressAction.Reset();

        OnLongPressEvent?.Invoke(gestureRecognizer as LongPressGestureRecognizer);
    }

    private void OnPan(GestureRecognizer gestureRecognizer)
    {
        _panAction.Reset();

        OnPanEvent?.Invoke(gestureRecognizer as PanGestureRecognizer);
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
            return Instance.lookJoystick.xAxis.value;
        }

        return Input.GetAxis("Mouse X");
    }

    public static float GetCameraRotationY()
    {
        if ((Input.touchSupported || Instance.mockMobile))
        {
            return -Instance.lookJoystick.yAxis.value;
        }

        return Input.GetAxis("Mouse Y");
    }

    public static float GetForward()
    {
        return Instance.moveJoystick.yAxis.value;
    }

    public static float GetRight()
    {
        return Instance.moveJoystick.xAxis.value;
    }

    public static bool IsSprinting()
    {
        // Always sprint on mobile?
        return true;
    }

    class EndAction <T>
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