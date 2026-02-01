using UnityEngine;

namespace FPS.Combat
{
    public class GunPickup : Pickup
    {
        [SerializeField] GunSO gunSO;

        protected override void OnPickup(GameObject player)
        {
            player.GetComponent<Fighter>().EquipGun(gunSO);
        }
    }
}
