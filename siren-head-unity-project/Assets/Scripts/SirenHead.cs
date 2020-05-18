using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public struct SirenAudio
{
    public AudioClip AudioClip;
    public int MinTriggerDistance;
}

public class SirenHead : MonoBehaviour
{
    public const float LightBlinkTime = 1f;
    public const float DefaultMovementSpeed = 10f;

    public float movementSpeedMultiplier = 1f;
    public float sirenDistance = 200f;
    public bool enableRandomMovementSpeeds = true;

    public bool isFollowingPlayer = true;
    public int playerDetectionDistance = 200;
    public MeshFilter mapPlane;

    public List<SirenAudio> sirenAudios = new List<SirenAudio>();

    private Light _light;
    private AudioSource _sirenAudio;
    private AudioSource _footstepAudio;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private NoiseGenerator _posNoiseGen;
    private NoiseGenerator _speedNoiseGen;

    private bool _navDestSet = false;

    private Player _player;

    private void Start()
    {
        _light = GetComponentInChildren<Light>();
        _light.enabled = false;

        _sirenAudio = GetComponents<AudioSource>()[0];
        _footstepAudio = GetComponents<AudioSource>()[1];
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        _posNoiseGen = gameObject.AddComponent<NoiseGenerator>();
        _posNoiseGen.width = 100;
        _posNoiseGen.height = 100;
        _posNoiseGen.scale = 100;
        _posNoiseGen.offset = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));

        _speedNoiseGen = gameObject.AddComponent<NoiseGenerator>();
        _speedNoiseGen.width = 100;
        _speedNoiseGen.height = 100;
        _speedNoiseGen.scale = 100;
        _speedNoiseGen.offset = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));

        _player = FindObjectOfType<Player>();

        SirenHeadAnimEventReceiver.OnFootstep += OnFootstep;

        sirenAudios.Sort((x, y) => x.MinTriggerDistance.CompareTo(y.MinTriggerDistance));
    }

    private void OnFootstep()
    {
        _footstepAudio.Play();
    }

    private void Update()
    {
        if (_navMeshAgent != null)
        {
            _navMeshAgent.speed = movementSpeedMultiplier * DefaultMovementSpeed;
            _animator.speed = movementSpeedMultiplier;

            bool wasFollowingPlayer = isFollowingPlayer;
            isFollowingPlayer = Vector3.Distance(_player.transform.position, transform.position) <
                                playerDetectionDistance;

            // DON"T reset if was following player and now isn't, that way sirenhead goes to last known player pos
            if (!wasFollowingPlayer && isFollowingPlayer)
                _navMeshAgent.ResetPath();

            if (isFollowingPlayer)
                OnFollowPlayer();
            else
                OnAmbientMovement();

            _posNoiseGen.offset.x += Time.deltaTime;
            _speedNoiseGen.offset.x += Time.deltaTime;
        }


        // Manage siren audio based on distance from player
        AudioClip audioClip = null;
        foreach (SirenAudio sirenAudio in sirenAudios)
        {
            if (sirenAudio.MinTriggerDistance > Vector3.Distance(transform.position, _player.transform.position))
            {
                audioClip = sirenAudio.AudioClip;
                break;
            }
        }

        if (_sirenAudio.clip != audioClip)
        {
            _sirenAudio.clip = audioClip;
        }

        if (!_sirenAudio.isPlaying && _sirenAudio.clip != null)
        {
            BlinkLight();
            // Make sure light gets turned off
            Invoke("BlinkLight", LightBlinkTime);
            _sirenAudio.Play();
        }
    }

    private void OnFollowPlayer()
    {
//        transform.position += transform.forward * movementSpeed * Time.deltaTime;
//        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));

        // Slow down siren head to give player a chance to escape
        movementSpeedMultiplier = 1;

        _player.StartShake();
        _navMeshAgent.SetDestination(_player.transform.position);
    }

    private void OnAmbientMovement()
    {
        _player.StopShake();

        // Teleport somewhere, and then after some time or important event, teleport near the player randomly
        // As soon as out of player's sight, do stuff here and maybe right away appear behind the player ;)
        if (!_navDestSet || Vector3.Distance(_navMeshAgent.destination, transform.position) < 100)
        {
            float x = Mathf.Lerp(
                mapPlane.transform.position.x -
                mapPlane.mesh.bounds.extents.x * Mathf.Abs(mapPlane.transform.localScale.x),
                mapPlane.transform.position.x +
                mapPlane.mesh.bounds.extents.x * Mathf.Abs(mapPlane.transform.localScale.x),
                _posNoiseGen.GetNoiseAt(new Vector2(0, 0)));
            float z = Mathf.Lerp(
                mapPlane.transform.position.z -
                mapPlane.mesh.bounds.extents.z * Mathf.Abs(mapPlane.transform.localScale.z),
                mapPlane.transform.position.z +
                mapPlane.mesh.bounds.extents.z * Mathf.Abs(mapPlane.transform.localScale.z),
                _posNoiseGen.GetNoiseAt(new Vector2(50, 50)));

            _navMeshAgent.SetDestination(new Vector3(x, 0, z));

            if (enableRandomMovementSpeeds)
                movementSpeedMultiplier = _speedNoiseGen.GetNoiseAt(new Vector2(0, 0)) * 10;

            _navDestSet = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            other.gameObject.transform.parent = GameObject.FindWithTag("SirenHeadHand").transform;
            // These offsets were just found by editing player pos in unity while in SirenHead's hands
            other.gameObject.transform.localPosition = new Vector3(0.001099201f, -0.03159722f, -0.001200514f);
            other.gameObject.transform.rotation = Quaternion.Euler(58.97f, 46.323f, -174.344f);
            other.gameObject.transform.LookAt(transform);
            other.gameObject.GetComponent<FPSController>().enabled = false;
            _animator.SetTrigger("Eat");
            Destroy(GetComponent<BoxCollider>());
            Destroy(_navMeshAgent);
        }
    }

    private void BlinkLight()
    {
        _light.enabled = !_light.enabled;
    }
}