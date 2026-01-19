using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3f;
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
        Vector3 movementValue = CalculateMovement();
        controller.Move(movementValue * movementSpeed * Time.deltaTime);

        bool isSprinting = playerInput.actions["Sprint"].IsPressed();

        if (isSprinting)
        {
            print("Sprinting!");
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
