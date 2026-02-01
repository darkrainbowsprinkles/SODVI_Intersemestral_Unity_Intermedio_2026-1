using UnityEngine;
using UnityEngine.AI;

namespace FPS.Movement
{
    public class Mover : MonoBehaviour
    {   
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float rotationSpeed = 10f;
        CharacterController controller;
        NavMeshAgent agent;
        Animator animator;
        float verticalVelocity;

        public bool IsGrounded()
        {
            return controller.isGrounded;
        }

        public void Stop()
        {
            agent.isStopped = true;
        }

        public void Jump(float jumpForce)
        {
            if (!controller.isGrounded)
            {
                return;
            }

            verticalVelocity += jumpForce;
        }

        public void LookAt(GameObject target)
        {
            Vector3 lookDirection = target.transform.position - transform.position;
            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            float speed = maxSpeed * Mathf.Clamp01(speedFraction);

            if (CompareTag("Player"))
            {
                Vector3 gravity = Vector3.up * verticalVelocity;
                Vector3 motion = destination * speed;
                controller.Move((gravity + motion) * Time.deltaTime);
            }
            else
            {
                agent.isStopped = false;
                agent.speed = speed;
                agent.destination = destination;
            }
        }

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            CalculateVerticalVelocity();
            UpdateBlendTree();
        }

        void CalculateVerticalVelocity()
        {
            if (controller == null)
            {
                return;
            }

            if (controller.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
        }

        void UpdateBlendTree()
        {
            if (animator == null)
            {
                return;
            }

            float localVelocity = transform.InverseTransformDirection(agent.velocity).magnitude;
            animator.SetFloat("movementSpeed", localVelocity, 0.1f, Time.deltaTime);
        }
    }
}