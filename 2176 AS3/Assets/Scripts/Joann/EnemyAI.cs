using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer, whatIsWall;

    // patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Patrolling")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    // attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public int attackDamage = 1;

    // states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        // Start patrolling towards the first point
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        playerInSightRange = CanSeePlayer();

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private bool CanSeePlayer()
    {
        // First, check if the player is even within the sight radius sphere.
        if (Physics.CheckSphere(transform.position, sightRange, whatIsPlayer))
        {
            // If they are, then perform a line-of-sight check.
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Shoot a ray from the enemy towards the player.
            // If the ray hits a wall before it hits the player, then the enemy can't see the player.
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, whatIsWall))
            {
                // The raycast did NOT hit a wall, so the enemy can see the player.
                return true;
            }
        }

        // If either the sphere check or the raycast check fails, the enemy cannot see the player.
        return false;
    }

    private void Patrolling()
    {
        // If the agent is not busy and has reached its destination...
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // ...go to the next point.
            GoToNextPatrolPoint();
        }
    }
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("Patrol points array is empty! Assign points in the Inspector.", this.gameObject);
            return;
        }

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        Debug.Log("Enemy heading to patrol point: " + patrolPoints[currentPatrolIndex].name);

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // make sure enemy does not move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
