using UnityEngine;

public class AmmoPickup : Pickup
{
    [SerializeField] AmmoType ammoType;
    [SerializeField] int number;

    protected override void OnPickup(GameObject player)
    {
        player.GetComponent<PlayerController>().AdjustAmmo(ammoType, number);
    }
}
