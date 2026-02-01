using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float updateDelay;

    private Transform _player;

    private bool _hasPosition;
    private Vector3 _lastPlayerPosition;
    private float _lastPositionUpdate;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!_player) return;

        float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
        if (distanceToPlayer > 10.0f)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }
        else
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        }

        float distanceFromLastSetPosition = Vector3.Distance(_player.position, _lastPlayerPosition);

        if (!_hasPosition ||
            distanceFromLastSetPosition > 10.0f ||
            distanceToPlayer < 10.0f ||
            Time.time - _lastPositionUpdate > updateDelay
        )
        {
            _hasPosition = true;
            _lastPlayerPosition = _player.position;
            _lastPositionUpdate = Time.time;

            agent.SetDestination(_player.position);
        }
    }
}
