using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] float range = 10f;
    [SerializeField] float damage = 100f;

    public void Fire()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        
        if (Physics.Raycast(cameraPosition, cameraForward, out RaycastHit hit, range))
        {
            Health health = hit.transform.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
