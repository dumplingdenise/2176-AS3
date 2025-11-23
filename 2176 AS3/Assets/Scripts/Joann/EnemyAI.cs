using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer, whatIsWall;

    [Header("Light Interaction")]
    public LightInteraction safeZoneLight;
    public float lightSafeDistance = 15f;
    public float fleeDistance = 25f;

    [Header("Patrolling")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    // attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public int attackDamage = 1;
    public ParticleSystem punchVFX;

    // states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private bool isFleeing = false;
    private bool hasNoticedPlayer = false;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        if (fleeDistance <= lightSafeDistance)
        {
            Debug.LogWarning("Warning: Flee Distance should be greater than Light Safe Distance on the EnemyAI, or the enemy may get stuck.", this.gameObject);
        }

        // Start patrolling towards the first point
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        // The highest priority is checking if we SHOULD be fleeing.
        if (IsLightAThreat())
        {
            // If the light is a threat and we are not already in the fleeing state, start fleeing.
            if (!isFleeing)
            {
                StartFleeing();
            }
        }

        // Now, execute behavior based on our current state.
        if (isFleeing)
        {
            // If we are in the fleeing state, our only job is to handle that.
            HandleFleeing();
        }
        else
        {
            // If we are not fleeing, execute the normal patrol/chase/attack logic.
            playerInSightRange = CanSeePlayer();
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInSightRange && playerInAttackRange) AttackPlayer();
        }
    }

    // helper function to check if light is a threat
    private bool IsLightAThreat()
    {
        if (safeZoneLight != null && safeZoneLight.light.enabled)
        {
            float distanceToLight = Vector3.Distance(transform.position, safeZoneLight.transform.position);
            return distanceToLight < lightSafeDistance;
        }
        return false;
    }

    // function called to being fleeing state
    private void StartFleeing()
    {
        isFleeing = true;
        hasNoticedPlayer = false; // reset notice player flag when fleeing
        Debug.Log("Light is a threat! Starting to flee.");

        Vector3 directionAwayFromLight = (transform.position - safeZoneLight.transform.position).normalized;
        // The destination is now calculated using the larger fleeDistance.
        Vector3 targetDestination = safeZoneLight.transform.position + directionAwayFromLight * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // function that runs every frame while enemy is fleeing
    private void HandleFleeing()
    {
        // Check if we have reached our destination.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // We've arrived at our safe spot. Now, is the light still a threat?
            if (!IsLightAThreat())
            {
                // The coast is clear! Stop fleeing and go back to normal.
                isFleeing = false;
                Debug.Log("Reached safe spot. Resuming normal behavior.");
            }
            else
            {
                // We reached the spot, but the light is STILL a threat (e.g., it moved closer).
                // Find a new spot to run to.
                Debug.Log("Reached destination, but still not safe. Finding new flee point.");
                StartFleeing();
            }
        }
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
        // reset notice flag when enemy loses sight of player
        if (hasNoticedPlayer)
        {
            hasNoticedPlayer = false;
        }
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
        // play "notice" sound 1st time this state is entered
        if (!hasNoticedPlayer)
        {
            hasNoticedPlayer = true;
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound("EnemyNotice");
            }
        }

        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // make sure enemy does not move
        agent.SetDestination(transform.position);

        // create temporary vector & flatten y-axis so enemy only rotates horizontally to look @ player
        Vector3 lookPosition = player.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        if (!alreadyAttacked)
        {
            if (punchVFX != null)
            {
                // This plays the particle system that is already in the scene, at its correct position.
                punchVFX.Play();
            }

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

        if (safeZoneLight != null)
        {
            // The "danger zone" that triggers the flee
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(safeZoneLight.transform.position, lightSafeDistance);

            // The "target safety zone" where the enemy will run to
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(safeZoneLight.transform.position, fleeDistance);
        }
    }
}
