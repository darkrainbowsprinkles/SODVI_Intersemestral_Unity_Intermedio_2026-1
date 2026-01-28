using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 200f;
    Animator animator;
    float currentHealth = 0f;

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
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
            if (animator != null)
            {
                animator.SetTrigger("die");
            }

            GetComponent<Collider>().enabled = false;
        }

        print(currentHealth);
    }

    public bool IsDead()
    {
        return currentHealth == 0;
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
