using UnityEngine;

[CreateAssetMenu(menuName = "Guns/New Gun")]
public class GunSO : ScriptableObject
{
    [SerializeField] AmmoType ammoType;
    [SerializeField] Sprite gunIcon;
    [SerializeField] Texture2D crosshair;
    [SerializeField] Texture2D scope;
    [SerializeField] Gun gunPrefab;
    [SerializeField] float damage = 10f;
    [SerializeField] float range = 20f;
    [SerializeField] float cooldown = 0.5f;
    [SerializeField] bool isAutomatic = false;
    [SerializeField] bool canZoom = false;
    [SerializeField] float zoomAmount = 10f;

    public Gun Spawn(Transform gunContainer)
    {
        Gun gunInstance = Instantiate(gunPrefab, gunContainer);
        return gunInstance;
    }

    public Texture2D GetScope()
    {
        return scope;
    }

    public float GetZoomAmount()
    {
        return zoomAmount;
    }

    public bool CanZoom()
    {
        return canZoom;
    }

    public Texture2D GetCrosshair()
    {
        return crosshair;
    }

    public Sprite GetGunIcon()
    {
        return gunIcon;
    }

    public AmmoType GetAmmoType()
    {
        return ammoType;
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
