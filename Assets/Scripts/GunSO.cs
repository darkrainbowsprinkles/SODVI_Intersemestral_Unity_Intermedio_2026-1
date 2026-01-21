using UnityEngine;

[CreateAssetMenu(menuName = "Guns/New Gun")]
public class GunSO : ScriptableObject
{
    [SerializeField] Gun gunPrefab;
    [SerializeField] float damage = 10f;
    [SerializeField] float range = 20f;
    [SerializeField] float cooldown = 0.5f;
    [SerializeField] bool isAutomatic = false;

    public Gun Spawn(Transform gunContainer)
    {
        Gun gunInstance = Instantiate(gunPrefab, gunContainer);
        return gunInstance;
    }

    public bool IsAutomatic()
    {
        return isAutomatic;
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetRange()
    {
        return range;
    }

    public float GetCooldown()
    {
        return cooldown;
    }
}
