using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Fix: Declaring Animator reference here to make it accessible in Start()
    private Animator animator;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;   // New dedicated Walk Speed
    public float runSpeed = 6f;    // Run Speed (Replaced the old 'moveSpeed')
    public float jumpHeight = 2f;
    public float gravity = -20f;   // Adjusted gravity to a more typical value for Unity
    public float rotationSpeed = 10f;

    [Header("Camera Reference (Optional)")]
    public Transform mainCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentMoveSpeed; // Tracks the current actual speed (walkSpeed or runSpeed)

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        if (mainCamera == null)
            mainCamera = Camera.main.transform;
    }

    // for sfx
    AudioManager audioManager;

    void Awake()
    {
        var audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject == null)
        {
            Debug.LogError("No GameObject with 'Audio' tag found!");
            return;
        }
        
        audioManager = audioObject.GetComponent<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager component not found on Audio GameObject!");
        }
        else
        {
            Debug.Log("AudioManager found and assigned successfully");
        }
    }

    void Update()
    {
        // --- 1. Ground Check & Gravity Reset ---
        if (controller.isGrounded)
        {
            // Reset vertical velocity only if it was negative (hitting the ground)
            if (velocity.y < 0)
                velocity.y = -2f;
        }

        // --- 2. Input & Direction Calculation ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // This vector is the raw input direction, relative to the camera's view
        Vector3 inputDir = (mainCamera.forward * z + mainCamera.right * x);
        inputDir.y = 0f;
        inputDir.Normalize();

        // Check for movement input
        bool isMoving = inputDir.magnitude > 0.1f;
        bool isRunningInput = Input.GetKey(KeyCode.LeftShift);


        // --- Animation and Speed Logic (Walk/Run/Idle) ---
        float targetAnimSpeed = 0f;
        float targetWorldSpeed = 0f;

        if (isMoving)
        {
            if (isRunningInput)
            {
                targetAnimSpeed = 1.0f;
                targetWorldSpeed = runSpeed + 10;
            }
            else
            {
                targetAnimSpeed = 0.5f;
                targetWorldSpeed = walkSpeed;
            }
        }
        else
        {
            // Player is idle: Zero animation speed and zero world speed
            targetAnimSpeed = 0f;
            targetWorldSpeed = 0f;
        }

        // Set the ACTUAL physical movement speed
        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetWorldSpeed, Time.deltaTime * 10f);

        // 💡 Correct Animator Call: Using the "Speed" parameter
        // The 0.1f is a 'dampTime' for smooth transition, which is highly recommended.
        animator.SetFloat("Speed", targetAnimSpeed, 0.1f, Time.deltaTime);

        // --- 4. Character Movement and Rotation ---
        // This uses the calculated currentMoveSpeed to physically move the character.
        Vector3 finalMoveVector = inputDir * currentMoveSpeed * Time.deltaTime;
        controller.Move(finalMoveVector);

        // Rotation (Only rotate if there is movement)
        if (isMoving)
        {
            // Create a rotation looking in the direction of the movement vector
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            // Smoothly apply the rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // --- 5. Jump and Gravity Application ---
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            if (audioManager == null)
            {
                Debug.LogError("AudioManager is null when trying to play jump sound!");
            }
            else if (audioManager.Jump == null)
            {
                Debug.LogError("Jump AudioClip is not assigned in AudioManager!");
            }
            else
            {
                Debug.Log("Attempting to play jump sound...");
                audioManager.PlaySFX(audioManager.Jump);
            }
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // Optional: Add animator.SetTrigger("Jump") here
        }

        // Apply gravity to vertical velocity
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical motion separately
        controller.Move(velocity * Time.deltaTime);
    }
}