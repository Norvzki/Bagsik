using UnityEngine;

// Enforce that an Animator and CharacterController must be on the GameObject
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    [Header("Animation Settings")]
    public float walkSpeed = 2f;      // Actual world speed for walking
    public float runSpeed = 6f;       // Actual world speed for running
    public float rotationSpeed = 10f; // How quickly player turns

    [Header("Movement Settings")]
    public float gravity = 20.0f;     // Gravity for falling

    // Private variables to track state
    private float currentAnimationSpeed = 0f;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        // Get the required components when the game starts
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- 1. Get Input and Determine Direction ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // This vector holds the direction based on keyboard input (WASD)
        Vector3 inputDir = new Vector3(h, 0, v);

        // Get the magnitude (0 to 1) of the input, representing how much the keys are pressed
        float inputMagnitude = inputDir.normalized.magnitude;
        bool isMoving = inputMagnitude > 0.1f;
        bool isRunningInput = Input.GetKey(KeyCode.LeftShift);

        // --- 2. Calculate World Movement Speed and Direction ---
        if (controller.isGrounded)
        {
            // The direction the player intends to move (relative to the camera/world)
            moveDirection = inputDir;

            // Determine the target speed based on input and running state
            float targetWorldSpeed = 0f;
            if (isMoving)
            {
                targetWorldSpeed = isRunningInput ? runSpeed : walkSpeed;
            }

            // Apply speed to the movement direction
            moveDirection *= targetWorldSpeed;

            // --- 3. Handle Rotation ---
            if (isMoving)
            {
                // Create a rotation looking in the direction of the input
                Quaternion targetRot = Quaternion.LookRotation(inputDir, Vector3.up);
                // Smoothly rotate the player to face the direction of movement
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
            }

            // Apply gravity *after* calculating movement, but set the Y-component to 0 
            // before multiplying by speed, then apply the Y velocity here.
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // --- 4. Move the CharacterController ---
        // Move the player in the calculated direction
        controller.Move(moveDirection * Time.deltaTime);


        // --- 5. Animate the Character ---
        // The animation system often uses a normalized speed (like 0=Idle, 0.5=Walk, 1.0=Run)
        // We will use the ratio of the current speed to the full runSpeed.

        float targetAnimationSpeed = 0f;

        if (isMoving)
        {
            // If the player is running, set animation speed to 1.0
            if (isRunningInput)
            {
                targetAnimationSpeed = 1.0f;
            }
            // If the player is only walking, set animation speed to 0.5 (or walkSpeed / runSpeed)
            else
            {
                targetAnimationSpeed = walkSpeed / runSpeed;
            }
        }

        // Smoothly transition the "Speed" parameter in the Animator
        currentAnimationSpeed = Mathf.Lerp(currentAnimationSpeed, targetAnimationSpeed, Time.deltaTime * 5f);
        animator.SetFloat("Speed", currentAnimationSpeed);

        // Optional: Send the 'IsJumping' or 'IsGrounded' state to the Animator
        animator.SetBool("IsJumping", !controller.isGrounded);
    }
}