using UnityEngine;

namespace FPS.Combat
{
    public class AmmoPickup : Pickup
    {
        [SerializeField] AmmoType ammoType;
        [SerializeField] int number;

        protected override void OnPickup(GameObject player)
        {
            player.GetComponent<Fighter>().AdjustAmmo(ammoType, number);
        }
    }
}
