using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour
{
    [SerializeField] TMP_Text ammoText;
    [SerializeField] Image gunIconImage;
    [SerializeField] Image ammoIconImage;
    [SerializeField] RawImage crosshairImage;
    [SerializeField] RawImage scopeImage;
    [SerializeField] AmmoIcon[] ammoIcons;
    PlayerController playerController;

    [System.Serializable]
    class AmmoIcon
    {
        public AmmoType ammoType;
        public Sprite ammoIcon;
    }

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void Start()
    {
        OnAmmoAdjusted();
        OnGunEquipped();
    }

    void Update()
    {
        GunSO currentGunSO = playerController.GetCurrentGunSO();

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

        scopeImage.enabled = playerController.IsZooming();
        crosshairImage.enabled = !playerController.IsZooming();
    }

    void OnEnable()
    {
        playerController.OnAmmoAdjusted += OnAmmoAdjusted;
        playerController.OnGunEquipped += OnGunEquipped;
    }

    void OnDisable()
    {
        playerController.OnAmmoAdjusted -= OnAmmoAdjusted;
        playerController.OnGunEquipped -= OnGunEquipped;
    }

    void OnAmmoAdjusted()
    {
        GunSO currentGunSO = playerController.GetCurrentGunSO();
        int currentAmmo = playerController.GetAmmo(currentGunSO.GetAmmoType());
        ammoText.text = currentAmmo.ToString();
    }

    void OnGunEquipped()
    {
        GunSO currentGunSO = playerController.GetCurrentGunSO();
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
