using FPS.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPS.UI
{
    public class GunUI : MonoBehaviour
    {
        [SerializeField] TMP_Text ammoText;
        [SerializeField] Image gunIconImage;
        [SerializeField] Image ammoIconImage;
        [SerializeField] RawImage crosshairImage;
        [SerializeField] RawImage scopeImage;
        [SerializeField] AmmoIcon[] ammoIcons;
        Fighter fighter;

        [System.Serializable]
        class AmmoIcon
        {
            public AmmoType ammoType;
            public Sprite ammoIcon;
        }

        void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            fighter = player.GetComponent<Fighter>();
        }

        void Start()
        {
            OnAmmoAdjusted();
            OnGunEquipped();
        }

        void Update()
        {
            GunSO currentGunSO = fighter.GetCurrentGunSO();

            if (currentGunSO == null)
            {
                return;
            }

            if (currentGunSO.GetScope() == null)
            {
                scopeImage.enabled = false;
                crosshairImage.enabled = true;
                return;
            }

            scopeImage.enabled = fighter.IsZooming();
            crosshairImage.enabled = !fighter.IsZooming();
        }

        void OnEnable()
        {
            fighter.OnAmmoAdjusted += OnAmmoAdjusted;
            fighter.OnGunEquipped += OnGunEquipped;
        }

        void OnDisable()
        {
            fighter.OnAmmoAdjusted -= OnAmmoAdjusted;
            fighter.OnGunEquipped -= OnGunEquipped;
        }

        void OnAmmoAdjusted()
        {
            GunSO currentGunSO = fighter.GetCurrentGunSO();
            int currentAmmo = fighter.GetAmmo(currentGunSO.GetAmmoType());
            ammoText.text = currentAmmo.ToString();
        }

        void OnGunEquipped()
        {
            GunSO currentGunSO = fighter.GetCurrentGunSO();
            gunIconImage.sprite = currentGunSO.GetGunIcon();
            ammoIconImage.sprite = GetAmmoIcon(currentGunSO.GetAmmoType());
            crosshairImage.texture = currentGunSO.GetCrosshair();
            OnAmmoAdjusted();
        }

        Sprite GetAmmoIcon(AmmoType ammoType)
        {
            foreach (AmmoIcon ammoIcon in ammoIcons)
            {
                if (ammoIcon.ammoType == ammoType)
                {
                    return ammoIcon.ammoIcon;
                }
            }

            return null;
        }
    }
}