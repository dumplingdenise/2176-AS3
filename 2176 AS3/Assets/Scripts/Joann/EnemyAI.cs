using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer, whatIsWall;

    [Header("Light Interaction")]
    public List<LightInteraction> safeZoneLights = new List<LightInteraction>();
    public float lightSafeDistance = 15f;
    public float fleeDistance = 25f;
    private LightInteraction currentThreateningLight;

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
        LightInteraction closestThreat = FindClosestThreateningLight();

        if (closestThreat != null)
        {
            // If we are not fleeing, or if the new threat is closer than our old one, start fleeing.
            if (!isFleeing || closestThreat != currentThreateningLight)
            {
                StartFleeing(closestThreat);
            }
        }

        if (isFleeing)
        {
            HandleFleeing();
        }
        else
        {
            // Normal AI behavior
            playerInSightRange = CanSeePlayer();
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInSightRange && playerInAttackRange) AttackPlayer();
        }
    }

    private LightInteraction FindClosestThreateningLight()
    {
        LightInteraction closestLight = null;
        float minDistance = float.MaxValue;

        foreach (var light in safeZoneLights)
        {
            // Check if the light is valid and currently turned on
            if (light != null && light.light.enabled)
            {
                float distance = Vector3.Distance(transform.position, light.transform.position);

                // If this light is within the safe distance AND is closer than any other we've found
                if (distance < lightSafeDistance && distance < minDistance)
                {
                    minDistance = distance;
                    closestLight = light;
                }
            }
        }
        return closestLight; // This will be null if no lights are active and close enough
    }

    private void StartFleeing(LightInteraction threat)
    {
        isFleeing = true;
        hasNoticedPlayer = false;
        currentThreateningLight = threat; // Remember the current threat
        agent.stoppingDistance = 0f;

        Debug.Log($"Light '{threat.name}' is a threat! Starting to flee.");
        Vector3 directionAwayFromLight = (transform.position - threat.transform.position).normalized;
        Vector3 targetDestination = threat.transform.position + directionAwayFromLight * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void HandleFleeing()
    {
        // Check if our current threat is still valid and active
        bool isThreatStillActive = currentThreateningLight != null &&
                                   currentThreateningLight.light.enabled &&
                                   Vector3.Distance(transform.position, currentThreateningLight.transform.position) < lightSafeDistance;

        // If we've reached our destination OR our threat is no longer active, stop fleeing
        if (!agent.pathPending && agent.remainingDistance < 0.5f || !isThreatStillActive)
        {
            isFleeing = false;
            currentThreateningLight = null;
            Debug.Log("Threat is gone or safe spot reached. Resuming normal behavior.");
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

        if (safeZoneLights.Count > 0)
        {
            foreach (var light in safeZoneLights)
            {
                if (light != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(light.transform.position, lightSafeDistance);
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(light.transform.position, fleeDistance);
                }
            }
        }
    }
}
