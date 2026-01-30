using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] bool destroy = true;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);

            if (destroy)
            {
                Destroy(gameObject);
            }
        }
    }

    protected abstract void OnPickup(GameObject player);
}
