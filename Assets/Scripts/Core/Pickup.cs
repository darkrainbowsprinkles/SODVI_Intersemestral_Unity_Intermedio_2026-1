using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] bool destroy = true;
    [SerializeField] GameObject pickupEffect;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            if (destroy)
            {
                Destroy(gameObject);
            }
        }
    }

    protected abstract void OnPickup(GameObject player);
}
