using System;
using DigitalRubyShared;
using UnityEngine;

[RequireComponent(typeof(FingersScript))]
public class GameInput : MonoBehaviour
{
    public const float ActionTimeout = 0.250f;
    public bool mockMobile = true;
    public float mobileSensitivity = 0.05f;

    public static event Action<SwipeGestureRecognizer> OnSwipeEvent;
    public static event Action<LongPressGestureRecognizer> OnLongPressEvent;
    public static event Action<PanGestureRecognizer> OnPanEvent;
    public static event Action<TapGestureRecognizer> OnDoubleTapEvent;

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

    private EndAction _panAction;
    private EndAction _longPressAction;
    private EndAction _tapAction;

    private void Start()
    {
        _fingersScript = GetComponent<FingersScript>();
        _fingersScript.TreatMousePointerAsFinger = mockMobile;

        SwipeGestureRecognizer swipeGestureRecognizer = new SwipeGestureRecognizer();
        swipeGestureRecognizer.StateUpdated += OnSwipe;
        swipeGestureRecognizer.FailOnDirectionChange = false;
        swipeGestureRecognizer.EndMode = SwipeGestureRecognizerEndMode.EndWhenTouchEnds;

        TapGestureRecognizer doubleTapGestureRecognizer = new TapGestureRecognizer();
        doubleTapGestureRecognizer.NumberOfTapsRequired = 2;
        doubleTapGestureRecognizer.StateUpdated += OnDoubleTap;

        LongPressGestureRecognizer longPressGestureRecognizer = new LongPressGestureRecognizer();
        longPressGestureRecognizer.MinimumDurationSeconds = 0.1f;
        longPressGestureRecognizer.StateUpdated += OnLongPress;

        PanGestureRecognizer panGestureRecognizer = new PanGestureRecognizer();
        panGestureRecognizer.StateUpdated += OnPan;

        _fingersScript.AddGesture(swipeGestureRecognizer);
        _fingersScript.AddGesture(longPressGestureRecognizer);
        _fingersScript.AddGesture(panGestureRecognizer);
        _fingersScript.AddGesture(doubleTapGestureRecognizer);

        //        Cursor.lockState = CursorLockMode.Locked;

        _panAction = new EndAction(panGestureRecognizer, -999999f);
        _longPressAction = new EndAction(longPressGestureRecognizer, -999999f);
        _tapAction = new EndAction(doubleTapGestureRecognizer, -999999f);
    }

    private void Update()
    {
        _panAction.Update();
        _longPressAction.Update();
        _tapAction.Update();
    }

    private void OnSwipe(GestureRecognizer gestureRecognizer)
    {
        OnSwipeEvent?.Invoke(gestureRecognizer as SwipeGestureRecognizer);
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
            if (Instance._panAction.Running && ((PanGestureRecognizer) Instance._panAction.Data).State ==
                GestureRecognizerState.Executing)
                return ((PanGestureRecognizer) Instance._panAction.Data).DeltaX * Instance.mobileSensitivity;
            return 0;
        }

        return Input.GetAxis("Mouse X");
    }

    public static float GetCameraRotationY()
    {
        if ((Input.touchSupported || Instance.mockMobile))
        {
            if (Instance._panAction.Running && ((PanGestureRecognizer) Instance._panAction.Data).State ==
                GestureRecognizerState.Executing)
                return ((PanGestureRecognizer) Instance._panAction.Data).DeltaY * Instance.mobileSensitivity;
            return 0;
        }

        return Input.GetAxis("Mouse Y");
    }

    public static float GetForward()
    {
        if (Input.touchSupported || Instance.mockMobile)
        {
            if (Instance._panAction.Running &&
                ((GestureRecognizer) Instance._panAction.Data).State == GestureRecognizerState.Executing)
                return 1;
            return 0;
        }

        return Input.GetAxis("Vertical");
    }

    public static bool IsSprinting()
    {
        // Always sprint on mobile?
        if (Input.touchSupported || Instance.mockMobile)
            return true;
        return Input.GetKey(KeyCode.LeftShift);
    }

    class EndAction
    {
        public float StartTime;
        public readonly object Data;
        public bool Running;

        public EndAction(object data = null, float startTime = float.NaN)
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