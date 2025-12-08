using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class ChickenWanderStandalone : MonoBehaviour
{
    [Header("Wander Settings")]
    public float wanderRadius = 3f;
    public float moveSpeed = 1f;
    public float rotationSpeed = 150f; // degrees per second
    public float targetReachedThreshold = 0.1f;
    public float newTargetInterval = 2f;

    [Header("Idle Settings")]
    public float idleDurationMin = 0.5f;
    public float idleDurationMax = 1.5f;

    private CharacterController controller;
    private Animator animator;

    private Vector3 wanderTarget;
    private float timer;
    private Vector3 moveDirection = Vector3.zero;

    private bool isIdle = false;
    private float idleTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        PickNewTarget();
        timer = 0f;
    }

    void Update()
    {
        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime); // idle anim

            if (idleTimer <= 0f)
            {
                isIdle = false;
                PickNewTarget();
                timer = 0f;
            }
            return; // skip movement while idle
        }

        timer += Time.deltaTime;

        Vector3 currentPos = transform.position;
        Vector3 toTarget = wanderTarget - currentPos;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float distance = toTargetXZ.magnitude;

        if (distance < targetReachedThreshold || timer > newTargetInterval)
        {
            isIdle = true;
            idleTimer = Random.Range(idleDurationMin, idleDurationMax);
            animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
            timer = 0f;
            return;
        }

        // Move toward target
        Vector3 targetDir = toTargetXZ.normalized;

        // Rotate smoothly toward direction
        if (targetDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move forward
        moveDirection = transform.forward * moveSpeed;

        // Gravity
        if (!controller.isGrounded)
        {
            moveDirection += Physics.gravity;
        }

        controller.Move(moveDirection * Time.deltaTime);

        // Animate walking
        float speedPercent = moveSpeed / 3f; // adjust 3f to match your max speed
        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    }

    void PickNewTarget()
    {
        const int maxAttempts = 20;
        float checkDistance = wanderRadius;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Random direction (unit circle on XZ plane)
            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(randomDir2D.x, 0, randomDir2D.y);

            // Check if the direction is clear
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            if (!Physics.Raycast(origin, randomDirection, checkDistance))
            {
                // No obstacle — set wander target
                wanderTarget = transform.position + randomDirection * wanderRadius;
                return;
            }
        }

        // Fallback: just rotate in place
        wanderTarget = transform.position;
    }


    bool IsValidTarget(Vector3 candidate)
    {
        Vector3 direction = candidate - transform.position;
        float distance = direction.magnitude;

        // Obstacle check
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction.normalized, distance))
        {
            return false;
        }

        return true;
    }
}
