using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float sprintMultiplier = 2f;
    [SerializeField] Transform gunContainer;
    [SerializeField] GunSO defaultGunSO;
    [SerializeField] AmmoSlot[] ammoSlots;
    PlayerInput playerInput;
    CharacterController controller;
    Gun currentGun;
    GunSO currentGunSO;
    float timeSinceLastShot = Mathf.Infinity;
    Dictionary<AmmoType, int> ammoLookup;

    public void EquipGun(GunSO gunSO)
    {
        if (currentGun != null)
        {
            Destroy(currentGun.gameObject);
        }

        currentGunSO = gunSO;
        currentGun = gunSO.Spawn(gunContainer);
    }

    [System.Serializable]
    class AmmoSlot
    {
        public AmmoType ammoType;
        public int ammoAmount;
    }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        EquipGun(defaultGunSO);
        CreateAmmoLookup();
    }

    void CreateAmmoLookup()
    {
        ammoLookup = new Dictionary<AmmoType, int>();

        foreach (AmmoSlot slot in ammoSlots)
        {
            ammoLookup[slot.ammoType] = slot.ammoAmount;
        }
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        HandleMovement();
        HandleFiring();
    }

    void HandleMovement()
    {
        float speed = movementSpeed;
        bool isSprinting = playerInput.actions["Sprint"].IsPressed();

        if (isSprinting)
        {
            speed = movementSpeed * sprintMultiplier;
        }

        Vector3 movementValue = CalculateMovement();
        controller.Move(movementValue * speed * Time.deltaTime);
    }

    void HandleFiring()
    {
        if (timeSinceLastShot < currentGunSO.GetCooldown())
        {
            return;
        }

        int ammo = GetAmmo(currentGunSO.GetAmmoType());

        if (ammo <= 0)
        {
            return;
        }

        InputAction fireInput = playerInput.actions["Fire"];

        if (currentGunSO.IsAutomatic() && fireInput.IsPressed())
        {
            Shoot();
        }
        else if (!currentGunSO.IsAutomatic() && fireInput.WasPressedThisFrame())
        {
            Shoot();
        }
    }

    int GetAmmo(AmmoType ammoType)
    {
        return ammoLookup[ammoType];
    }

    void AdjustAmmo(AmmoType ammoType, int number)
    {
        ammoLookup[ammoType] += number;
    }

    void Shoot()
    {
        currentGun.Fire(defaultGunSO.GetDamage(), defaultGunSO.GetRange());
        timeSinceLastShot = 0f;
        AdjustAmmo(currentGunSO.GetAmmoType(), -1);
        print(GetAmmo(currentGunSO.GetAmmoType()));
    }

    Vector3 CalculateMovement()
    {
        Vector2 movementValue = playerInput.actions["Movement"].ReadValue<Vector2>();
        
        Vector3 right = (Camera.main.transform.right * movementValue.x).normalized;
        right.y = 0f;

        Vector3 forward = (Camera.main.transform.forward * movementValue.y).normalized;
        forward.y = 0f;

        return right + forward;
    }
}
