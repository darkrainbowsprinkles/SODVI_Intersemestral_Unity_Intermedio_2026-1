using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float sprintMultiplier = 2f;
    [SerializeField] Transform gunContainer;
    [SerializeField] GunSO gunSO;
    PlayerInput playerInput;
    CharacterController controller;
    Gun gun;
    float timeSinceLastShot = Mathf.Infinity;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        EquipGun();
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
        if (timeSinceLastShot < gunSO.GetCooldown())
        {
            return;
        }

        InputAction fireInput = playerInput.actions["Fire"];

        if (gunSO.IsAutomatic() && fireInput.IsPressed())
        {
            Shoot();
        }
        else if (!gunSO.IsAutomatic() && fireInput.WasPressedThisFrame())
        {
            Shoot();
        }
    }

    void Shoot()
    {
        timeSinceLastShot = 0f;
        gun.Fire(gunSO.GetDamage(), gunSO.GetRange());
    }

    void EquipGun()
    {
        gun = gunSO.Spawn(gunContainer);
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
