using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AudioClip initialAudio;
    public AudioClip[] wheelFoundVoiceClips;
    
    private Animation _animation;
    private AudioSource _voiceAudio;
    private int _wheelsFound = 0;

    private void Start()
    {
        _animation = GetComponentInChildren<Animation>();
        _voiceAudio = GetComponentInChildren<AudioSource>();
        
        _voiceAudio.clip = initialAudio;
        _voiceAudio.Play();
    }

    public void StartShake()
    {
        if(!_animation.isPlaying)
            _animation.Play();
    }

    public void StopShake()
    {
        if(_animation.isPlaying)
            _animation.Stop();
    }

    void PlayObjectFoundVoiceLine()
    {
        _voiceAudio.clip = wheelFoundVoiceClips[_wheelsFound - 1];
        _voiceAudio.Play();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PickupableObject>() != null)
        {
            _wheelsFound++;
            PlayObjectFoundVoiceLine();
            Destroy(other.gameObject);
        }
    }
}