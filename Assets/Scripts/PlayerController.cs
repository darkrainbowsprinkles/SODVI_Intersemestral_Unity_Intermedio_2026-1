using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float sprintMultiplier = 2f;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] Transform gunContainer;
    [SerializeField] GunSO defaultGunSO;
    [SerializeField] AmmoSlot[] ammoSlots;
    [SerializeField] CinemachineCamera firstPersonCamera;
    [SerializeField] Camera gunCamera;
    PlayerInput playerInput;
    CharacterController controller;
    Health health;
    Gun currentGun;
    int currentGunIndex;
    List<GunSO> gunSOInventory = new();
    float verticalVelocity;
    float defaultFieldOfView;
    float timeSinceLastShot = Mathf.Infinity;
    bool isZooming = false;
    Dictionary<AmmoType, int> ammoLookup;

    public event Action OnAmmoAdjusted;
    public event Action OnGunEquipped;

    public GunSO GetCurrentGunSO()
    {
        return gunSOInventory[currentGunIndex];
    }

    public bool IsZooming()
    {
        return isZooming;
    }

    public void EquipGun(GunSO gunSO)
    {
        if (currentGun != null)
        {
            Destroy(currentGun.gameObject);
        }

        gunSOInventory.Add(gunSO);
        currentGunIndex = gunSOInventory.Count - 1;
        currentGun = gunSOInventory[currentGunIndex].Spawn(gunContainer);
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
        health = GetComponent<Health>();
        CreateAmmoLookup();
        EquipGun(defaultGunSO);
        defaultFieldOfView = firstPersonCamera.Lens.FieldOfView;
    }

    void OnEnable()
    {
        playerInput.actions["Scroll Gun"].performed += ScrollGun;
    }

    void OnDisable()
    {
        playerInput.actions["Scroll Gun"].performed -= ScrollGun;
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (health.IsDead())
        {
            return;
        }

        timeSinceLastShot += Time.deltaTime;
        CalculateVerticalVelocity();
        HandleMovement();
        HandleJumping();
        HandleFiring();
        HandleZoom();
    }

    void ScrollGun(InputAction.CallbackContext context)
    {
        if (gunSOInventory == null || gunSOInventory.Count == 0) return;

        float scrollValue = context.ReadValue<float>();

        if (scrollValue > 0f)
        {
            currentGunIndex = (currentGunIndex + 1) % gunSOInventory.Count;
        }
        else if (scrollValue < 0f)
        {
            currentGunIndex = (currentGunIndex - 1 + gunSOInventory.Count) % gunSOInventory.Count;
        }

        if (currentGun != null)
        {
            Destroy(currentGun.gameObject);
        }

        currentGun = gunSOInventory[currentGunIndex].Spawn(gunContainer);
        OnGunEquipped?.Invoke();
    }

    void CreateAmmoLookup()
    {
        ammoLookup = new Dictionary<AmmoType, int>();

        foreach (AmmoSlot slot in ammoSlots)
        {
            ammoLookup[slot.ammoType] = slot.ammoAmount;
        }
    }

    void CalculateVerticalVelocity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }

    void HandleZoom()
    {
        InputAction zoomAction = playerInput.actions["Zoom"];
        GunSO currentGunSO = GetCurrentGunSO();

        if (currentGunSO.CanZoom() && zoomAction.IsPressed())
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

    void HandleMovement()
    {
        float speed = movementSpeed;
        bool isSprinting = playerInput.actions["Sprint"].IsPressed();

        if (isSprinting)
        {
            speed = movementSpeed * sprintMultiplier;
        }

        Vector3 gravity = Vector3.up * verticalVelocity;
        Vector3 movementMotion = CalculateMovement() * speed;
        controller.Move((gravity + movementMotion) * Time.deltaTime);
    }

    void HandleJumping()
    {
        if (!controller.isGrounded)
        {
            return;
        }

        InputAction jumpAction = playerInput.actions["Jump"];

        if (jumpAction.WasPressedThisFrame())
        {
            verticalVelocity += jumpForce;
        }
    }

    void HandleFiring()
    {
        GunSO currentGunSO = GetCurrentGunSO();

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
        GunSO currentGunSO = GetCurrentGunSO();
        currentGun.Fire(currentGunSO.GetDamage(), currentGunSO.GetRange());
        timeSinceLastShot = 0f;
        AdjustAmmo(currentGunSO.GetAmmoType(), -1);
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
