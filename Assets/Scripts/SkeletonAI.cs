using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonAI : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private Animator animator;
    private float nextIdleTime; // Next time for the skeleton to idle
    private float nextChangeTargetTime; // Next time for the skeleton to change target
    private bool isIdle = true; // Flag to determine if the skeleton is currently idling
    private bool isAggressive; // Flag to determine if the skeleton is aggressive
    private bool isAttacking; // Flag to determine if the skeleton is currently attacking
    private float attackStartTime; // Time when the attack starts

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null)
        {
            Debug.LogWarning("Target is not set for skeleton AI!");
            return;
        }

        SetRandomIdleTime();
        SetRandomChangeTargetTime();
    }

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not set for skeleton AI!");
            return;
        }

        if (Time.time >= nextIdleTime)
        {
            if (!isIdle)
            {
                SetIdleBehavior();
                SetRandomIdleTime();
            }
        }

        if (Time.time >= nextChangeTargetTime)
        {
            SetRandomChangeTargetTime();
            SetNewRandomDestination();
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > agent.stoppingDistance + 2f) // Adjusting the distance to make it slightly further from the player
        {
            if (!isAggressive)
            {
                SetAggressiveBehavior();
            }
            RotateTowardsTarget(); // Rotate towards the target before moving
            agent.destination = target.position;
        }
        else
        {
            if (isAggressive && !isAttacking)
            {
                SetAttackBehavior();
            }
            agent.isStopped = true; // Stop AI movement
        }

        // Transition between idle and run animations
        float movementThreshold = 0.1f; // Adjust this threshold as needed
        animator.SetBool("run", agent.velocity.magnitude > movementThreshold); // Activate the "run" animation when the agent's velocity exceeds the threshold
    }

    // Set the skeleton to idle and stop AI behavior
    private void SetIdleBehavior()
    {
        isIdle = true;
        isAggressive = false;
        agent.isStopped = true; // Stop AI movement
    }

    // Set the skeleton to aggressive and resume AI behavior
    private void SetAggressiveBehavior()
    {
        isIdle = false;
        isAggressive = true;
        isAttacking = false;
        agent.isStopped = false; // Resume AI movement
    }

    // Set the skeleton to attack behavior
    private void SetAttackBehavior()
    {
        isIdle = false; // Set idle to false to immediately stop idling
        isAttacking = true;
        attackStartTime = Time.time;
        animator.SetTrigger("attack"); // Trigger the "attack" animation
    }

    // Set a random time for the next idle
    private void SetRandomIdleTime()
    {
        nextIdleTime = Time.time + Random.Range(2f, 5f); // Random idle time between 2 to 5 seconds
    }

    // Set a random time for the next target change
    private void SetRandomChangeTargetTime()
    {
        nextChangeTargetTime = Time.time + Random.Range(5f, 10f); // Random time between 5 to 10 seconds
    }

    // Set a new random destination for the skeleton
    private void SetNewRandomDestination()
    {
        Vector3 randomPosition = Random.insideUnitSphere * 10f; // Random position within 10 units of the skeleton
        randomPosition.y = 0f; // Make sure the destination is on the same level as the skeleton
        agent.SetDestination(randomPosition); // Set the new destination
    }

    // Rotate towards the target
    private void RotateTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Adjust rotation speed here
    }

    // Method to be called when the attack animation is finished
    public void OnAttackAnimationComplete()
    {
        isAttacking = false;
    }
}
