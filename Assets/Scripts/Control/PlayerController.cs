using System;
using System.Collections.Generic;
using FPS.Combat;
using FPS.Core;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FPS.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float movementSpeed = 3f;
        [SerializeField] float sprintMultiplier = 2f;
        [SerializeField] float jumpForce = 3f;

        [Header("Gun Settings")]
        [SerializeField] Transform gunContainer;
        [SerializeField] GunSO defaultGunSO;
        [SerializeField] AmmoSlot[] ammoSlots;

        [Header("Cameras")]
        [SerializeField] CinemachineCamera firstPersonCamera;
        [SerializeField] Camera gunCamera;
        [SerializeField] CinemachineBasicMultiChannelPerlin cameraNoise;
        [SerializeField] float walkNoiseFrequency = 0.02f;
        [SerializeField] float sprintNoiseFrequency = 0.04f;

        [Header("Footsteps")]
        [SerializeField] float walkStepSpeed = 0.6f;
        [SerializeField] float sprintStepSpeed = 0.4f;
        [SerializeField] UnityEvent onFootstep;

        PlayerInput playerInput;
        CharacterController controller;
        Health health;
        Gun currentGun;
        List<GunSO> gunSOInventory = new();
        int currentGunIndex;
        float verticalVelocity;
        float defaultFieldOfView;
        float timeSinceLastShot = Mathf.Infinity;
        float timeSinceLastStep = Mathf.Infinity;
        bool isZooming = false;
        Dictionary<AmmoType, int> ammoLookup;
        AggroGroup aggroGroup;

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
            if (gunSOInventory.Contains(gunSO))
            {
                int existingIndex = gunSOInventory.IndexOf(gunSO);

                if (existingIndex != currentGunIndex)
                {
                    EquipExistingGun(existingIndex);
                }

                return;
            }

            AddGun(gunSO);
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

        [Serializable]
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
            aggroGroup = FindFirstObjectByType<AggroGroup>();
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
            if (aggroGroup.GroupDead())
            {
                return;
            }

            if (health.IsDead())
            {
                return;
            }

            timeSinceLastShot += Time.deltaTime;
            timeSinceLastStep += Time.deltaTime;

            CalculateVerticalVelocity();
            HandleMovement();
            HandleJumping();
            HandleFiring();
            HandleZoom();
        }

        void EquipExistingGun(int index)
        {
            DestroyCurrentGun();
            currentGunIndex = index;
            currentGun = gunSOInventory[currentGunIndex].Spawn(gunContainer);
            OnGunEquipped?.Invoke();
        }

        void AddGun(GunSO gunSO)
        {
            DestroyCurrentGun();
            gunSOInventory.Add(gunSO);
            currentGunIndex = gunSOInventory.Count - 1;
            currentGun = gunSOInventory[currentGunIndex].Spawn(gunContainer);
            OnGunEquipped?.Invoke();
        }

        void DestroyCurrentGun()
        {
            if (currentGun != null)
            {
                Destroy(currentGun.gameObject);
            }
        }

        void ScrollGun(InputAction.CallbackContext context)
        {
            if (gunSOInventory.Count == 0) 
            {
                return;
            }

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
            InputAction sprintAction = playerInput.actions["Sprint"];
            Vector2 movementValue = playerInput.actions["Movement"].ReadValue<Vector2>();

            float speed = movementSpeed;
            float stepSpeed = walkStepSpeed;

            if (movementValue.magnitude > 0)
            {
                if (sprintAction.IsPressed())
                {
                    speed = movementSpeed * sprintMultiplier;
                    cameraNoise.FrequencyGain = sprintNoiseFrequency;
                    stepSpeed = sprintStepSpeed;
                }
                else
                {
                    cameraNoise.FrequencyGain = walkNoiseFrequency;
                }

                if (controller.isGrounded && timeSinceLastStep > stepSpeed)
                {
                    timeSinceLastStep = 0f;
                    onFootstep?.Invoke();
                }
            }
            else
            {
                cameraNoise.FrequencyGain = 0f;
            }

            Vector3 gravity = Vector3.up * verticalVelocity;
            Vector3 movementMotion = CalculateMovement(movementValue) * speed;
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

        Vector3 CalculateMovement(Vector2 movementValue)
        {
            Vector3 right = (Camera.main.transform.right * movementValue.x).normalized;
            right.y = 0f;

            Vector3 forward = (Camera.main.transform.forward * movementValue.y).normalized;
            forward.y = 0f;

            return right + forward;
        }
    }
}