using UnityEngine;

public class GunPickup : Pickup
{
    [SerializeField] GunSO gunSO;

    protected override void OnPickup(GameObject player)
    {
        player.GetComponent<PlayerController>().EquipGun(gunSO);
    }
}
