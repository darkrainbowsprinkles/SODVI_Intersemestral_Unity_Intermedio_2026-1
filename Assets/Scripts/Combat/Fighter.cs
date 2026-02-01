using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace FPS.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] Transform gunContainer;
        [SerializeField] GunSO defaultGunSO;
        [SerializeField] CinemachineCamera firstPersonCamera;
        [SerializeField] Camera gunCamera;
        Gun currentGun;
        int currentGunIndex;
        float timeSinceLastShot = Mathf.Infinity;
        float defaultFieldOfView;
        bool isZooming = false;
        List<GunSO> gunInventory = new();
        Dictionary<AmmoType, int> ammoSlots = new();

        public event Action OnAmmoAdjusted;
        public event Action OnGunEquipped;

        public bool IsZooming()
        {
            return isZooming;
        }

        public GunSO GetCurrentGunSO()
        {
            return gunInventory[currentGunIndex];
        }

        public int GetAmmo(AmmoType ammoType)
        {
            if (!ammoSlots.ContainsKey(ammoType))
            {
                ammoSlots[ammoType] = 0;
            }
            
            return ammoSlots[ammoType];
        }

        public void AdjustAmmo(AmmoType ammoType, int number)
        {
            if (!ammoSlots.ContainsKey(ammoType))
            {
                ammoSlots[ammoType] = 0;
            }

            ammoSlots[ammoType] += number;
            OnAmmoAdjusted?.Invoke();
        }

        public bool CanShoot()
        {
            GunSO currentGunSO = GetCurrentGunSO();

            if (timeSinceLastShot < currentGunSO.GetCooldown())
            {
                return false;
            }

            int ammo = GetAmmo(currentGunSO.GetAmmoType());

            if (ammo <= 0)
            {
                return false;
            }

            return true;
        }

        public void Shoot()
        {
            GunSO currentGunSO = GetCurrentGunSO();
            currentGun.Fire(currentGunSO.GetDamage(), currentGunSO.GetRange());
            AdjustAmmo(currentGunSO.GetAmmoType(), -1);
            timeSinceLastShot = 0f;
        }

        public void ScrollGun(float scrollValue)
        {
            if (gunInventory.Count == 0) 
            {
                return;
            }

            if (scrollValue > 0f)
            {
                currentGunIndex = (currentGunIndex + 1) % gunInventory.Count;
            }
            else if (scrollValue < 0f)
            {
                currentGunIndex = (currentGunIndex - 1 + gunInventory.Count) % gunInventory.Count;
            }

            if (currentGun != null)
            {
                Destroy(currentGun.gameObject);
            }

            currentGun = gunInventory[currentGunIndex].Spawn(gunContainer);
            OnGunEquipped?.Invoke();
        }

        public void EquipGun(GunSO gunSO)
        {
            if (currentGun != null)
            {
                Destroy(currentGun.gameObject);
            }

            if (gunInventory.Contains(gunSO))
            {
                int existingIndex = gunInventory.IndexOf(gunSO);

                if (existingIndex != currentGunIndex)
                {
                    EquipExistingGun(existingIndex);
                }

                return;
            }

            AddGun(gunSO);
        }

        public void ToggleZoom(bool state)
        {
            GunSO currentGunSO = GetCurrentGunSO();

            if (state && currentGunSO.CanZoom())
            {
                firstPersonCamera.Lens.FieldOfView = currentGunSO.GetZoomAmount();
                gunCamera.fieldOfView = currentGunSO.GetZoomAmount();
                isZooming = true;
            }
            else
            {
                firstPersonCamera.Lens.FieldOfView = defaultFieldOfView;
                gunCamera.fieldOfView = defaultFieldOfView;
                isZooming = false;
            }
        }

        void Awake()
        {
            EquipGun(defaultGunSO);
            defaultFieldOfView = firstPersonCamera.Lens.FieldOfView;
        }

        void Update()
        {
            timeSinceLastShot += Time.deltaTime;
        }

        void EquipExistingGun(int index)
        {
            currentGunIndex = index;
            currentGun = gunInventory[currentGunIndex].Spawn(gunContainer);
            OnGunEquipped?.Invoke();
        }

        void AddGun(GunSO gunSO)
        {
            gunInventory.Add(gunSO);
            currentGunIndex = gunInventory.Count - 1;
            currentGun = gunInventory[currentGunIndex].Spawn(gunContainer);
            OnGunEquipped?.Invoke();
        }
    }
}