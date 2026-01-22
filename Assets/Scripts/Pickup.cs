using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);
        }
    }

    protected abstract void OnPickup(GameObject player);
}
