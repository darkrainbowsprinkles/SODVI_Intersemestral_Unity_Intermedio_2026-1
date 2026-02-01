using FPS.Core;
using FPS.Movement;
using UnityEngine;
using UnityEngine.Events;

namespace FPS.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] float chaseSpeedFraction = 1f;
        [SerializeField] float chaseRange = 10f;
        [SerializeField] float attackRange = 2f;
        [SerializeField] float hitRange = 3f;
        [SerializeField] float attackDamage = 30f;
        [SerializeField] UnityEvent onHit;
        Mover mover;
        Health health;
        Animator animator;
        GameObject player;

        void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();
            player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            if (health.IsDead())
            {
                return;
            }

            if (PlayerInRange(attackRange))
            {
                AttackBehavior();
            }
            else if (PlayerInRange(chaseRange))
            {
                ChaseBehavior();
            }
            else
            {
                mover.Stop();
            }
        }

        void ChaseBehavior()
        {
            mover.MoveTo(player.transform.position, chaseSpeedFraction);
            animator.ResetTrigger("attack");
        }

        void AttackBehavior()
        {
            mover.Stop();
            animator.SetTrigger("attack");
            mover.LookAt(player);
        }

        bool PlayerInRange(float range)
        {
            return Vector3.Distance(transform.position, player.transform.position) <= range;
        }

        // Called in Unity Events
        void Hit()
        {
            if (PlayerInRange(hitRange))
            {
                player.GetComponent<Health>().TakeDamage(attackDamage);
                onHit?.Invoke();
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }
}
