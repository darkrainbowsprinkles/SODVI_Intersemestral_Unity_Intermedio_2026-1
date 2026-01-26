using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float sprintMultiplier = 2f;
    [SerializeField] Transform gunContainer;
    [SerializeField] GunSO defaultGunSO;
    [SerializeField] AmmoSlot[] ammoSlots;
    [SerializeField] CinemachineCamera firstPersonCamera;
    PlayerInput playerInput;
    CharacterController controller;
    Gun currentGun;
    GunSO currentGunSO;
    float defaultFieldOfView;
    float timeSinceLastShot = Mathf.Infinity;
    bool isZooming = false;
    Dictionary<AmmoType, int> ammoLookup;

    public event Action OnAmmoAdjusted;
    public event Action OnGunEquipped;

    public GunSO GetCurrentGun()
    {
        return currentGunSO;
    }

    public bool IsZooming()
    {
        return isZooming;
    }

    public GunSO GetCurrentGunSO()
    {
        return currentGunSO;
    }

    public void EquipGun(GunSO gunSO)
    {
        if (currentGun != null)
        {
            Destroy(currentGun.gameObject);
        }

        currentGunSO = gunSO;
        currentGun = gunSO.Spawn(gunContainer);
        OnGunEquipped?.Invoke();
    }

    public void AdjustAmmo(AmmoType ammoType, int number)
    {
        ammoLookup[ammoType] += number;
        OnAmmoAdjusted?.Invoke();
    }

    public int GetAmmo(AmmoType ammoType)
    {
        return ammoLookup[ammoType];
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
        CreateAmmoLookup();
        EquipGun(defaultGunSO);
        defaultFieldOfView = firstPersonCamera.Lens.FieldOfView;
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        HandleZoom();
    }

    void HandleZoom()
    {
        InputAction zoomAction = playerInput.actions["Zoom"];

        if (currentGunSO.CanZoom() && zoomAction.IsPressed())
        {
            firstPersonCamera.Lens.FieldOfView = currentGunSO.GetZoomAmount();
            isZooming = true;
        }
        else
        {
            firstPersonCamera.Lens.FieldOfView = defaultFieldOfView;
            isZooming = false;
        }
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
