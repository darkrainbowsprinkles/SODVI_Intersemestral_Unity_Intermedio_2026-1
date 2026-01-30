using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 200f;
    public UnityEvent onDie;
    Animator animator;
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

        if (currentHealth == 0)
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("die");
        }

        GetComponent<Collider>().enabled = false;

        onDie?.Invoke();
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }
}
