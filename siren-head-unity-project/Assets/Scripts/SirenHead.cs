using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class SirenHead : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float sirenDistance = 200f;
    public float lightBlinkDelay = 1f;

    public bool isFollowingPlayer = true;

    private Light _light;
    private AudioSource _sirenAudio;
    private AudioSource _footstepAudio;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    
    private Player _player;
    private float _lightTimer = 0;

    private void Start()
    {
        _light = GetComponentInChildren<Light>();
        _light.enabled = false;
        
        _sirenAudio = GetComponents<AudioSource>()[0];
        _footstepAudio = GetComponents<AudioSource>()[1];
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        _player = FindObjectOfType<Player>();

        _lightTimer = lightBlinkDelay;

        SirenHeadAnimEventReceiver.OnFootstep += OnFootstep;
    }
    
    private void OnFootstep()
    {
        _footstepAudio.Play();
    }

    private void Update()
    {
        if(isFollowingPlayer)
            FollowPlayer();
        else
        {
            // Teleport somewhere, and then after some time or important event, teleport near the player randomly
            // As soon as out of player's sight, do stuff here and maybe right away appear behind the player ;) 
        }
    }

    private void FollowPlayer()
    {
        
//        transform.position += transform.forward * movementSpeed * Time.deltaTime;
//        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));

        _navMeshAgent.SetDestination(_player.transform.position);

        if (_lightTimer >= lightBlinkDelay)
        {
            BlinkLight();
            // Make sure light gets turned off
            Invoke("BlinkLight", lightBlinkDelay/2f);
            
            if(Vector3.Distance(transform.position, _player.transform.position) < sirenDistance)
                PlaySiren();

            _lightTimer = 0;
        }

        _lightTimer += Time.deltaTime;
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
            _animator.SetTrigger("Eat");
            Destroy(GetComponent<BoxCollider>());
        }
    }

    private void BlinkLight()
    {
        _light.enabled = !_light.enabled;
    }

    private void PlaySiren()
    {
        _sirenAudio.Play();
    }
}