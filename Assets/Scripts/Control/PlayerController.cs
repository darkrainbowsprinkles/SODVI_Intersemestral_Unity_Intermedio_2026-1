using FPS.Combat;
using FPS.Core;
using FPS.Movement;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FPS.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] float jumpForce = 3f;
        [SerializeField, Range(0f, 1f)] float walkSpeedFraction = 0.5f;
        [SerializeField, Range(0f, 1f)] float sprintSpeedFraction = 1f;

        [Header("Camera Motion")]
        [SerializeField] CinemachineBasicMultiChannelPerlin cameraNoise;
        [SerializeField] float walkNoiseFrequency = 0.02f;
        [SerializeField] float sprintNoiseFrequency = 0.04f;

        [Header("Footsteps")]
        [SerializeField] float walkStepDelay = 0.6f;
        [SerializeField] float sprintStepDelay = 0.4f;
        [SerializeField] UnityEvent onFootstep;

        PlayerInput playerInput;
        Health health;
        Mover mover;
        Fighter fighter;
        AggroGroup aggroGroup;
        float timeSinceLastStep = Mathf.Infinity;

        void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            aggroGroup = FindFirstObjectByType<AggroGroup>();
        }

        void OnEnable()
        {
            playerInput.actions["Scroll Gun"].performed += OnGunScrolled;
        }

        void OnDisable()
        {
            playerInput.actions["Scroll Gun"].performed -= OnGunScrolled;
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

            timeSinceLastStep += Time.deltaTime;

            HandleMovement();
            HandleJumping();
            HandleFiring();
            HandleZoom();
        }

        void HandleMovement()
        {
            Vector2 movementValue = playerInput.actions["Movement"].ReadValue<Vector2>();

            if (movementValue.magnitude > 0)
            {
                InputAction sprintAction = playerInput.actions["Sprint"];

                if (sprintAction.IsPressed())
                {
                    mover.MoveTo(CalculateMovement(movementValue), sprintSpeedFraction);
                    cameraNoise.FrequencyGain = sprintNoiseFrequency;
                    InvokeFootstep(sprintStepDelay);
                }
                else
                {
                    mover.MoveTo(CalculateMovement(movementValue), walkSpeedFraction);
                    cameraNoise.FrequencyGain = walkNoiseFrequency;
                    InvokeFootstep(walkStepDelay);
                }
            }
            else
            {
                cameraNoise.FrequencyGain = 0f;
            }
        }

        void InvokeFootstep(float stepSpeed)
        {
            if (mover.IsGrounded() && timeSinceLastStep > stepSpeed)
            {
                timeSinceLastStep = 0f;
                onFootstep?.Invoke();
            }
        }

        void HandleJumping()
        {
            InputAction jumpAction = playerInput.actions["Jump"];

            if (jumpAction.WasPressedThisFrame())
            {
                mover.Jump(jumpForce);
            }
        }

        void HandleFiring()
        {
            if (!fighter.CanShoot())
            {
                return;
            }

            InputAction fireInput = playerInput.actions["Fire"];
            GunSO currentGunSO = fighter.GetCurrentGunSO();

            if (currentGunSO.IsAutomatic() && fireInput.IsPressed())
            {
                fighter.Shoot();
            }
            else if (!currentGunSO.IsAutomatic() && fireInput.WasPressedThisFrame())
            {
                fighter.Shoot();
            }
        }

        void HandleZoom()
        {
            InputAction zoomAction = playerInput.actions["Zoom"];
            fighter.ToggleZoom(zoomAction.IsPressed());
        }

        void OnGunScrolled(InputAction.CallbackContext context)
        {
            fighter.ScrollGun(context.ReadValue<float>());
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