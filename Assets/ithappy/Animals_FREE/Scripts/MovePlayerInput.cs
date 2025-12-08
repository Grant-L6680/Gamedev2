using UnityEngine;

namespace ithappy.Animals_FREE
{
    [RequireComponent(typeof(CreatureMover))]
    public class AIChickenController : MonoBehaviour
    {
        private enum State { RunToFlower, ReturnToFlower, FleeFromPlayer, AttackFlower }

        [HideInInspector] public Collider gardenZone;

        [Header("References")]
        [SerializeField] public Transform flowerTarget;
        [SerializeField] public Transform player;

        [Header("Behavior Settings")]
        [SerializeField] private float reachFlowerDistance = 0.7f;
        [SerializeField] private float fearRadius = 1f;
        [SerializeField] private float forgetDistance = 12f;
        [SerializeField] private float attackSpinSpeed = 200f;

        [Header("Movement Settings")]
        [SerializeField] private bool useRunSpeed = true;

        private CreatureMover mover;
        private bool hasReachedFlower = false;
        private Vector2 moveAxis;
        private Vector3 lookTarget;
        private bool isRun;
        private bool isJump;

        // Flags to ensure AI doesn’t run until initialized
        private bool isInitialized = false;

        // PUBLIC METHODS TO SET TARGETS
        public void SetPlayer(Transform playerTransform) => player = playerTransform;
        public void SetFlower(Transform flowerTransform) => flowerTarget = flowerTransform;

        private void Awake()
        {
            mover = GetComponent<CreatureMover>();
        }

        private void Update()
        {
            // Auto-find missing targets dynamically if not assigned
            if (!player)
                player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (!flowerTarget)
                flowerTarget = GameObject.FindGameObjectWithTag("Flower")?.transform;

            // Only start AI when both targets exist
            if (!player || !flowerTarget)
                return;

            if (!isInitialized)
                isInitialized = true;

            // Run AI
            EvaluateStateMachine();
            SendInputToMover();
        }

        // =======================================================================
        // STATE MACHINE
        // =======================================================================
        private void EvaluateStateMachine()
        {
            moveAxis = Vector2.zero;
            isRun = false;
            isJump = false;

            Vector2 chicken2D = new Vector2(transform.position.x, transform.position.z);

            float distToPlayer = float.MaxValue;
            float distToFlower = float.MaxValue;

            if (player) distToPlayer = Vector2.Distance(chicken2D, new Vector2(player.position.x, player.position.z));
            if (flowerTarget) distToFlower = Vector2.Distance(chicken2D, new Vector2(flowerTarget.position.x, flowerTarget.position.z));

            // Priority 1: Flee player
            if (player != null && distToPlayer <= fearRadius)
            {
                Flee(player.position);
                return;
            }

            // Priority 2: Attack flower
            if (flowerTarget != null && distToFlower <= reachFlowerDistance)
            {
                hasReachedFlower = true;
                AttackFlower(flowerTarget.position);
                return;
            }

            // Priority 3: Return to flower if previously reached
            if (hasReachedFlower && flowerTarget != null)
            {
                if (distToFlower > forgetDistance)
                {
                    hasReachedFlower = false;
                    RunToFlower(flowerTarget.position);
                    return;
                }

                ReturnToFlower(flowerTarget.position);
                return;
            }

            // Priority 4: Run to flower if it exists
            if (flowerTarget != null)
            {
                RunToFlower(flowerTarget.position);
            }
        }

        // =======================================================================
        // BEHAVIOR FUNCTIONS
        // =======================================================================
        private void RunToFlower(Vector3 flowerPos) => MoveToward(flowerPos, true);
        private void ReturnToFlower(Vector3 flowerPos) => MoveToward(flowerPos, true);

        private void Flee(Vector3 playerPos)
        {
            Vector3 fleeDir = (transform.position - playerPos).normalized;
            Vector3 randomOffset = new Vector3(Random.Range(-0.25f, 0.25f), 0, Random.Range(-0.25f, 0.25f));
            fleeDir = (fleeDir + randomOffset).normalized;

            Vector3 localDir = transform.InverseTransformDirection(fleeDir);
            moveAxis = new Vector2(localDir.x, localDir.z);
            moveAxis = Vector2.ClampMagnitude(moveAxis, 1f);

            lookTarget = transform.position + fleeDir * 10f;
            isRun = true;

            Debug.DrawRay(transform.position, fleeDir * 5f, Color.red);
        }

        private void AttackFlower(Vector3 flowerPos)
        {
            Vector3 dir = (flowerPos - transform.position).normalized;
            moveAxis = Vector2.zero;
            lookTarget = transform.position + dir * 4f;
            transform.Rotate(Vector3.up, attackSpinSpeed * Time.deltaTime);
        }

        private void MoveToward(Vector3 target, bool running)
        {
            Vector3 dir = target - transform.position;
            dir.y = 0f;
            dir.Normalize();

            Vector3 localDir = transform.InverseTransformDirection(dir);
            moveAxis = new Vector2(localDir.x, localDir.z);
            moveAxis = Vector2.ClampMagnitude(moveAxis, 1f);

            lookTarget = transform.position + dir * 10f;
            isRun = running;
        }

        private void SendInputToMover()
        {
            mover.SetInput(in moveAxis, in lookTarget, in isRun, in isJump);
        }

        private void OnDrawGizmosSelected()
        {
            if (flowerTarget != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(flowerTarget.position, reachFlowerDistance);
                Gizmos.DrawLine(transform.position, flowerTarget.position);
            }

            if (player != null)
            {
                Gizmos.color = Color.yellow;
                Vector3 playerFlat = new Vector3(player.position.x, transform.position.y, player.position.z);
                Gizmos.DrawWireSphere(playerFlat, fearRadius);
                Gizmos.DrawLine(transform.position, playerFlat);
            }
        }
    }
}
