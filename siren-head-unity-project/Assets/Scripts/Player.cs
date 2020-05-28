using System;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public AudioClip initialAudio;
    public AudioClip[] wheelFoundVoiceClips;

    public AudioClip[] footstepAudioClips;
    public AudioClip mapAudioClip;

    public GameObject map;

    public InfoText startInfoText;
    
    private AudioSource _voiceAudio;
    private AudioSource _footstepAudio;
    private AudioSource _mapAudio;
    public int wheelsFound = 0;
    private int _footstepAudioIndex = 0;

    private int _textState = 0;

    public static event Action<Vector3> OnWheelFound;

    private void Start()
    {
        _voiceAudio = GetComponentInChildren<AudioSource>();
        
        _footstepAudio = gameObject.AddComponent<AudioSource>();
        _footstepAudio.volume = 0.3f;

        _mapAudio = gameObject.AddComponent<AudioSource>();
        _mapAudio.volume = 0.7f;
        _mapAudio.clip = mapAudioClip;

        _voiceAudio.clip = initialAudio;
        _voiceAudio.Play();

        FPSController.OnFootstep += OnFootstep;
        InfoText.OnFinished += OnInfoTextFinished;

        GameInput.OnDoubleTapEvent += OnDoubleTap;
    }

    private void OnDoubleTap(GestureRecognizer gestureRecognizer)
    {
        // Show/hide map
        if (gestureRecognizer.State == GestureRecognizerState.Ended)
        {
            map.SetActive(!map.activeSelf);
            _mapAudio.Play();
        }
    }

    private void OnInfoTextFinished(int id, InfoText infoText)
    {
        if (id == 0 && _textState == 0)
        {
            infoText.msgText = "Hold Down To Move And Orient The Camera";
            infoText.Reset();
            _textState++;
        }
        else if (id == 0 && _textState == 1)
        {
            infoText.msgText = "Remember To Get Back To The Car Once You've Found The Tires!";
            infoText.Reset();
            _textState++;
        }
        else if (id == 0)
        {
            Destroy(infoText.gameObject);
        }
    }

    void PlayObjectFoundVoiceLine()
    {
        _voiceAudio.clip = wheelFoundVoiceClips[wheelsFound - 1];
        _voiceAudio.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PickupableObject>() != null)
        {
            wheelsFound++;
            PlayObjectFoundVoiceLine();
            Destroy(other.gameObject);
            
            OnWheelFound(transform.position);
        }
    }

    private void OnFootstep()
    {
        _footstepAudioIndex++;
        if (_footstepAudioIndex == footstepAudioClips.Length)
            _footstepAudioIndex = 0;

        _footstepAudio.clip = footstepAudioClips[_footstepAudioIndex];
        _footstepAudio.Play();
    }
}