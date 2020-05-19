using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
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
    public const float IdleLightBlinkTime = 2f;

    public GameObject deathCamera;
    public GameObject head;

    public float movementSpeedMultiplier = 1f;
    public float sirenDistance = 200f;
    public bool enableRandomMovementSpeeds = true;

    public bool isFollowingPlayer = true;
    public int playerDetectionDistance = 200;
    public MeshFilter mapPlane;

    public List<SirenAudio> sirenAudios = new List<SirenAudio>();
    public AudioClip noiseClip;

    private Light _light;
    private AudioSource _sirenAudio;
    private AudioSource _footstepAudio;
    private AudioSource _noiseAudio;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private NoiseGenerator _posNoiseGen;
    private NoiseGenerator _speedNoiseGen;

    private bool _navDestSet = false;

    private Player _player;
    private float _idleLightTimer = 0;

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

        _noiseAudio = gameObject.AddComponent<AudioSource>();
        _noiseAudio.volume = 0;
        _noiseAudio.clip = noiseClip;
        _noiseAudio.loop = true;

        _player = FindObjectOfType<Player>();

        SirenHeadAnimEventReceiver.OnFootstep += OnFootstep;
        SirenHeadAnimEventReceiver.OnPlayerEaten += OnPlayerEaten;

        sirenAudios.Sort((x, y) => x.MinTriggerDistance.CompareTo(y.MinTriggerDistance));
        
        // Whenever the player has picked up a wheel, make sirenhead go to them ;P
        Player.OnWheelFound += delegate(Vector3 pos) { 
            _navMeshAgent.ResetPath();
            _navMeshAgent.SetDestination(pos);
            _navDestSet = true;
        };
    }

    private void OnPlayerEaten()
    {
        GameManager.OnPlayerDied();
    }

    private void OnFootstep()
    {
        _footstepAudio.Play();
    }

    private void Update()
    {
        // Enable sirenhead once player has found a wheel or sirenhead as been awakened
        if (_player.wheelsFound > 0 || Vector3.Distance(_player.transform.position, transform.position) <= playerDetectionDistance || _navMeshAgent.hasPath)
        {
            _animator.enabled = true;
            
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
        else
        {
            _animator.enabled = false;
            if (_idleLightTimer > IdleLightBlinkTime)
            {
                BlinkLight();
                Invoke("BlinkLight", IdleLightBlinkTime / 2f);
                _idleLightTimer = 0;
            }

            _idleLightTimer += Time.deltaTime;
            _player.StopShake();
        }
    }

    private void OnFollowPlayer()
    {
//        transform.position += transform.forward * movementSpeed * Time.deltaTime;
//        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));

        // Slow down siren head to give player a chance to escape
        // Also, based on # of wheels player has collected, if they have a lot, they better a void sirenhead as much as possible
        // Otherwise they're dead for sure
        movementSpeedMultiplier = _player.wheelsFound + 1;

        _player.StartShake();
        _navMeshAgent.SetDestination(_player.transform.position);
        
        if(!_noiseAudio.isPlaying)
            _noiseAudio.Play();
        _noiseAudio.volume = Mathf.Lerp(0, 0.15f, 1f - Mathf.InverseLerp(0, playerDetectionDistance,
            Vector3.Distance(transform.position, _player.transform.position)));
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
//            other.gameObject.transform.parent = GameObject.FindWithTag("SirenHeadHand").transform;
//            // These offsets were just found by editing player pos in unity while in SirenHead's hands
//            other.gameObject.transform.localPosition = new Vector3(-0.0049f, -0.0338f, 0.0108f);
//            other.gameObject.transform.LookAt(head.transform);
//            
//            other.gameObject.GetComponent<FPSController>().enabled = false;
//            other.gameObject.GetComponent<Player>().StopShake();
            deathCamera.SetActive(true);
            other.gameObject.SetActive(false);
            _noiseAudio.Stop();
            _animator.SetTrigger("Eat");
            Destroy(GetComponent<BoxCollider>());
            Destroy(_navMeshAgent);

            // Enable light so player can see sirenhead
            BlinkLight();
        }
    }

    private void BlinkLight()
    {
        _light.enabled = !_light.enabled;
    }
}