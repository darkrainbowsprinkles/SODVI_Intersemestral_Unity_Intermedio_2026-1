using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float sprintMultiplier = 2f;
    [SerializeField] Gun gun;
    PlayerInput playerInput;
    CharacterController controller;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float speed = movementSpeed;
        bool isSprinting = playerInput.actions["Sprint"].IsPressed();

        if (isSprinting)
        {
            speed = movementSpeed * sprintMultiplier;
        }

        Vector3 movementValue = CalculateMovement();
        controller.Move(movementValue * speed * Time.deltaTime);

        bool isFiring = playerInput.actions["Fire"].IsPressed();

        if (isFiring)
        {
            gun.Fire();
        }
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
