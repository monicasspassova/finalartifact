using UnityEngine;
using UnityEngine.AI;

public class GooseController : MonoBehaviour
{
    public Transform player;
    public float wanderRadius = 20f;
    public float wanderInterval = 3f;
    public float chaseSpeed = 8f;
    public float wanderSpeed = 3f;
    public float chaseRange = 15f;
    public float chaseDuration = 5f;

    private NavMeshAgent agent;
    private bool isChasing = false;
    private float wanderTimer;
    private float chaseTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = wanderSpeed;
        wanderTimer = wanderInterval;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Start chasing
        if (distanceToPlayer < chaseRange && !isChasing)
        {
            StartChasing();
        }

        if (isChasing)
        {
            agent.SetDestination(player.position);
            agent.speed = chaseSpeed;

            chaseTimer -= Time.deltaTime;
            if (chaseTimer <= 0)
            {
                StopChasing();
            }
        }
        else
        {
            agent.speed = wanderSpeed;
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
                agent.SetDestination(newPos);
                wanderTimer = wanderInterval;
            }
        }
    }

    public void StartChasing()
    {
        isChasing = true;
        chaseTimer = chaseDuration;
        agent.speed = chaseSpeed;
    }

    public void StopChasing()
    {
        isChasing = false;
        agent.speed = wanderSpeed;
        wanderTimer = wanderInterval;
    }

    static Vector3 RandomNavSphere(Vector3 origin, float radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius + origin;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, radius, 1);
        return hit.position;
    }
}