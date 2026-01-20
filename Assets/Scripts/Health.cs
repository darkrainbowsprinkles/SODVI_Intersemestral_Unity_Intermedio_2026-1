using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 200f;
    float currentHealth = 0f;

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (currentHealth == 0)
        {
            Destroy(gameObject);
        }

        print(currentHealth);
    }

    void Start()
    {
        currentHealth = maxHealth;
    }
}
