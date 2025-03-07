using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehaviour : MonoBehaviour
{    
    NavMeshAgent _agent;
    Vector3 _currentTargetPos;
    Animator _animator;
    float _defaultSpeed;
    [SerializeField] float talkingtime, talkingMaxTime = 5f;
    NPCTalkingController _talkingController;
    int numberOfNPCsToTalk;
    public bool isWaitingMat = false;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _talkingController = GetComponent<NPCTalkingController>();
        _defaultSpeed = _agent.speed;
        numberOfNPCsToTalk = Random.Range(1, NPCManager.instance.GetTotalStationaryNPCCount());        
    }
    private void OnEnable()
    {
        FindNewTarget();
    }
    
    private void Update()
    {
        if (isWaitingMat)
            if (NPCManager.instance.AreThereAnyNPCsAvailable())
            {
                _currentTargetPos = NPCManager.instance.GetRandomStationaryNPCTalkingPoint(_talkingController);
                isWaitingMat = false;
            }
        if (_currentTargetPos == Vector3.zero) return;

        float distance = Vector3.Distance(transform.position, _currentTargetPos);
        float remainingDistance = _agent.remainingDistance;

        if (_agent.pathPending || remainingDistance > 0.1f || !_agent.hasPath) // Hedefe ulaþmadýysa
        {
            _agent.speed = _defaultSpeed;
            _animator.SetBool("Walking", true);
            _agent.SetDestination(_currentTargetPos);
        }
        else if (_agent.hasPath && remainingDistance <= 0.1f) // Hedefe ulaþtýysa
        {
            _animator.SetBool("Walking", false);
            _agent.speed = 0f;
            _agent.ResetPath(); // Hareketi durdur

            if (_currentTargetPos == SpawnManager.instance.GetSpawnPoint()) // Spawn noktasýna ulaþtýysa
            {
                gameObject.SetActive(false);
                SpawnManager.instance.SpawnNewNpc(1);
                Destroy(gameObject, 0.5f);
                return;
            }

            

            if (!_talkingController.IsTalking())
                NPCManager.instance.MatchingNPCDialog(this,_talkingController.GetGuidID());

            if (talkingtime <= talkingMaxTime)
                talkingtime += Time.deltaTime;
            else
            {
                _currentTargetPos = Vector3.zero;
                talkingtime = 0f;
                numberOfNPCsToTalk--;
                NPCManager.instance.EndDialog(this, _talkingController.GetGuidID());
            }
        }
    }


    public void FindNewTarget() => StartCoroutine(IEGuidIdWaiting());
    IEnumerator IEGuidIdWaiting()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(_talkingController.GetGuidID()));
        if (numberOfNPCsToTalk > 0)
        {
            _currentTargetPos = NPCManager.instance.GetRandomStationaryNPCTalkingPoint(_talkingController);
        }
        else
        {
            _currentTargetPos = SpawnManager.instance.GetSpawnPoint();
        }
    }
    public void AgentOtoRotateActive(bool _active)
    {
        _agent.updateRotation = _active;
        _agent.updateUpAxis = _active;
    }
}