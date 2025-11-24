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
    public float lightSafeDistance = 15f; // how close a light needs to be to be considered a threat
    public float fleeDistance = 25f; // how far the enemy tries to run from a threatening light
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
    private bool isFleeing = false; // state flag to override all other behaviours
    private bool hasNoticedPlayer = false; // flag to play "notice" sound only once

    private void Awake()
    {
        // find player & get navmesh component at start
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        // initial warning to prevent ai from getting stuck
        if (fleeDistance <= lightSafeDistance)
        {
            Debug.LogWarning("Warning: Flee Distance should be greater than Light Safe Distance on the EnemyAI, or the enemy may get stuck.");
        }

        // start the patrol cycle immediately on game start
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        // check for light threats first - fleeing is the highest priority state
        LightInteraction closestThreat = FindClosestThreateningLight();

        if (closestThreat != null)
        {
            // enter flee state if a new threat is found
            if (!isFleeing || closestThreat != currentThreateningLight)
            {
                StartFleeing(closestThreat);
            }
        }

        // main state machine logic
        if (isFleeing)
        {
            HandleFleeing();
        }
        else
        {
            // normal ai behaviour when not fleeing
            playerInSightRange = CanSeePlayer();
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInSightRange && playerInAttackRange) AttackPlayer();
        }
    }

    // iterates through all safe zone lights to find closest active one
    private LightInteraction FindClosestThreateningLight()
    {
        LightInteraction closestLight = null;
        float minDistance = float.MaxValue;

        foreach (var light in safeZoneLights)
        {
            // only considers lights that are currently turned on
            if (light != null && light.light.enabled)
            {
                float distance = Vector3.Distance(transform.position, light.transform.position);

                // checks if this light is both a threat & closest one found so far
                if (distance < lightSafeDistance && distance < minDistance)
                {
                    minDistance = distance;
                    closestLight = light;
                }
            }
        }
        return closestLight; // returns null if no lights are active/close enough
    }

    private void StartFleeing(LightInteraction threat)
    {
        isFleeing = true;
        hasNoticedPlayer = false;  // reset player notice when fleeing
        currentThreateningLight = threat; // remember current threat
        agent.stoppingDistance = 0f;

        // calculate a target point directly away from the light source
        Debug.Log($"Light '{threat.name}' is a threat! Starting to flee.");
        Vector3 directionAwayFromLight = (transform.position - threat.transform.position).normalized;
        Vector3 targetDestination = threat.transform.position + directionAwayFromLight * fleeDistance;

        // find closest valid point on navmesh to calculated target
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // manages ai's behavior while in fleeing state
    private void HandleFleeing()
    {
        // checks if current threat is still active & close enough to be a danger
        bool isThreatStillActive = currentThreateningLight != null &&
                                   currentThreateningLight.light.enabled &&
                                   Vector3.Distance(transform.position, currentThreateningLight.transform.position) < lightSafeDistance;

        // stops fleeing once destination is reached / threat is gone
        if (!agent.pathPending && agent.remainingDistance < 0.5f || !isThreatStillActive)
        {
            isFleeing = false;
            currentThreateningLight = null;
            Debug.Log("Threat is gone or safe spot reached. Resuming normal behavior.");
        }
    }

    // more advanced visibility check than a simple sphere
    private bool CanSeePlayer()
    {
        // cheap sphere check first for performance
        if (Physics.CheckSphere(transform.position, sightRange, whatIsPlayer))
        {
            // if player is in range, do an expensive raycast to check for line of sight
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // if raycast does not hit a wall, enemy can see player
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, whatIsWall))
            {
                return true;
            }
        }

        // if either sphere check / raycast check fails = enemy cannot see player
        return false;
    }

    private void Patrolling()
    {
        // reset notice flag when enemy loses sight of player
        if (hasNoticedPlayer)
        {
            hasNoticedPlayer = false;
        }

        // moves to next patrol point only after reaching current one
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }
    void GoToNextPatrolPoint()
    {
        // safety check to prevent errors if no patrol points are assigned
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("Patrol points array is empty! Assign points in the Inspector.", this.gameObject);
            return;
        }

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        Debug.Log("Enemy heading to patrol point: " + patrolPoints[currentPatrolIndex].name);

        // use modulo operator to cycle through patrol points array
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void ChasePlayer()
    {
        // play "notice" sound only the first time enemy enters this state
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
        // stop enemy from moving to attack
        agent.SetDestination(transform.position);

        // create temporary vector & flatten y-axis so enemy only rotates horizontally to look @ player
        Vector3 lookPosition = player.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        // attack cooldown logic
        if (!alreadyAttacked)
        {
            if (punchVFX != null)
            {
                // This plays the particle system that is already in the scene, at its correct position.
                punchVFX.Play();
            }

            // get player health component & apply damage
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            // call ResetAttack after a delay to enable next attack
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    // helper function used by invoke to reset the attack cooldown
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // draw wire spheres in the editor to visualize ai ranges for easier debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // draw the light interaction ranges for every assigned light
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
