using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace FPS.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 200f;
        [SerializeField] UnityEvent onDamageTaken;
        public UnityEvent onDie;
        float currentHealth = 0f;

        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }

        public bool IsDead()
        {
            return currentHealth == 0;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead())
            {
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - damage);
            onDamageTaken?.Invoke();

            if (currentHealth == 0)
            {
                HandleDeath();
            }
        }

        void HandleDeath()
        {
            if (TryGetComponent(out Animator animator))
            {
                animator.SetTrigger("die");
            }

            if (TryGetComponent(out NavMeshAgent agent))
            {
                agent.enabled = false;
            }

            GetComponent<Collider>().enabled = false;

            onDie?.Invoke();
        }

        void Start()
        {
            currentHealth = maxHealth;
        }
    }
}
