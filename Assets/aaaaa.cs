//using Unity.VisualScripting;
//using UnityEngine;

//this script works for the most part, but the chicken never actually runs away from the player

//using UnityEngine;

//namespace ithappy.Animals_FREE
//{
//    [RequireComponent(typeof(CreatureMover))]
//    public class AIChickenController : MonoBehaviour
//    {
//        private enum State
//        {
//            RunToFlower,
//            ReturnToFlower,
//            FleeFromPlayer,
//            AttackFlower
//        }

//        [Header("References")]
//        [SerializeField] private Transform flowerTarget;
//        [SerializeField] private Transform player;

//        [Header("Behavior Settings")]
//        [SerializeField] private float reachFlowerDistance = 0.7f;
//        [SerializeField] private float fearRadius = 4f;
//        [SerializeField] private float forgetDistance = 12f;
//        [SerializeField] private float attackSpinSpeed = 200f;

//        [Header("Movement Settings")]
//        [SerializeField] private bool useRunSpeed = true;
//        [SerializeField, Range(0.1f, 5f)] private float directionStrength = 1.0f;

//        private CreatureMover mover;

//        private bool hasReachedFlower = false;
//        private State currentState = State.RunToFlower;

//        private Vector2 moveAxis;
//        private Vector3 lookTarget;
//        private bool isRun;
//        private bool isJump;

//        private void Awake()
//        {
//            mover = GetComponent<CreatureMover>();
//        }

//        private void Update()
//        {
//            if (!flowerTarget || !player) return;

//            HandleStateMachine();
//            SendInputToMover();
//        }

//        // =============================
//        private void HandleStateMachine()
//        {
//            // Horizontal distances only
//            Vector3 chickenPos = new Vector3(transform.position.x, 0f, transform.position.z);
//            Vector3 playerPos = new Vector3(player.position.x, 0f, player.position.z);
//            Vector3 flowerPos = new Vector3(flowerTarget.position.x, 0f, flowerTarget.position.z);

//            float distToPlayer = Vector3.Distance(chickenPos, playerPos);
//            float distToFlower = Vector3.Distance(chickenPos, flowerPos);

//            // Flee ALWAYS has priority
//            if (distToPlayer < fearRadius)
//            {
//                currentState = State.FleeFromPlayer;
//            }

//            switch (currentState)
//            {
//                case State.RunToFlower:
//                    RunToFlower(distToFlower);
//                    break;
//                case State.ReturnToFlower:
//                    ReturnToFlower(distToFlower);
//                    break;
//                case State.FleeFromPlayer:
//                    Flee(distToPlayer);
//                    break;
//                case State.AttackFlower:
//                    AttackFlower(distToFlower);
//                    break;
//            }
//        }

//        private void RunToFlower(float flowerDist)
//        {
//            MoveToward(flowerTarget.position, useRunSpeed);

//            if (flowerDist <= reachFlowerDistance)
//            {
//                hasReachedFlower = true;
//                currentState = State.AttackFlower;
//            }
//        }

//        private void ReturnToFlower(float flowerDist)
//        {
//            if (!hasReachedFlower) return;

//            if (flowerDist > forgetDistance)
//            {
//                hasReachedFlower = false;
//                currentState = State.FleeFromPlayer;
//                return;
//            }

//            MoveToward(flowerTarget.position, useRunSpeed);

//            if (flowerDist <= reachFlowerDistance)
//                currentState = State.AttackFlower;
//        }

//        private void Flee(float playerDist)
//        {
//            Vector3 dir = (transform.position - player.position).normalized;
//            MoveDirection(dir, useRunSpeed);

//            if (playerDist > fearRadius)
//            {
//                currentState = hasReachedFlower ? State.ReturnToFlower : State.RunToFlower;
//            }
//        }

//        private void AttackFlower(float flowerDist)
//        {
//            moveAxis = Vector2.zero;

//            Vector3 dir = (flowerTarget.position - transform.position).normalized;
//            lookTarget = transform.position + dir * 4f;

//            transform.Rotate(Vector3.up, attackSpinSpeed * Time.deltaTime);

//            if (flowerDist > reachFlowerDistance * 1.5f)
//                currentState = State.ReturnToFlower;
//        }

//        // =============================
//        private void MoveToward(Vector3 worldTarget, bool running)
//        {
//            Vector3 dir = (worldTarget - transform.position).normalized;
//            MoveDirection(dir, running);
//        }

//        private void MoveDirection(Vector3 dir, bool running)
//        {
//            isRun = running;
//            isJump = false;

//            lookTarget = transform.position + dir * 10f;

//            Vector3 camForward = (lookTarget - transform.position).normalized;
//            Vector3 camRight = Vector3.Cross(Vector3.up, camForward);

//            float f = Vector3.Dot(dir, camForward) * directionStrength;
//            float r = Vector3.Dot(dir, camRight) * directionStrength;

//            moveAxis = new Vector2(r, f);
//            moveAxis = Vector2.ClampMagnitude(moveAxis, 1f);
//        }

//        private void SendInputToMover()
//        {
//            mover.SetInput(in moveAxis, in lookTarget, in isRun, in isJump);
//        }

//        // =============================
//        // DEBUG GIZMOS
//        private void OnDrawGizmosSelected()
//        {
//            if (flowerTarget != null)
//            {
//                Gizmos.color = Color.magenta;
//                Gizmos.DrawWireSphere(flowerTarget.position, reachFlowerDistance);
//                Gizmos.DrawLine(transform.position, flowerTarget.position);
//            }

//            if (player != null)
//            {
//                Gizmos.color = Color.yellow;
//                Vector3 playerFlat = new Vector3(player.position.x, transform.position.y, player.position.z);
//                Gizmos.DrawWireSphere(playerFlat, fearRadius);
//                Gizmos.DrawLine(transform.position, playerFlat);
//            }
//        }
//    }
//}

//any idea why 