using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour
{
    [SerializeField] TMP_Text ammoText;
    [SerializeField] Image gunIconImage;
    PlayerController playerController;

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
        GunSO currentGun = playerController.GetCurrentGun();
        int currentAmmo = playerController.GetAmmo(currentGun.GetAmmoType());
        ammoText.text = currentAmmo.ToString();
    }

    void OnGunEquipped()
    {
        GunSO currentGun = playerController.GetCurrentGun();
        gunIconImage.sprite = currentGun.GetGunIcon();
    }

}
