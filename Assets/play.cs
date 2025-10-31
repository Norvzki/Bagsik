using UnityEngine;

public class play : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 1.0f;
    public float runSpeed = 2.0f;
    public float duckSpeed = 0.5f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    
    [Header("References")]
    public Camera mainCamera;
    
    private Animator animator;
    private CharacterController controller;
    private bool isDucking = false;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isJumping = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
        }
        
        // Get or add CharacterController
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.center = new Vector3(0, 1, 0);
            controller.height = 2.0f;
            Debug.Log("CharacterController added automatically to " + gameObject.name);
        }
        
        if (mainCamera == null) 
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                Debug.LogError("No main camera found in scene!");
        }
    }
    
    void Update()
    {
        if (controller == null) return;
        
        // Ground check
        isGrounded = controller.isGrounded;
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
        }

        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);
        
        // Check for ducking
        bool duckPressed = Input.GetKey(KeyCode.E);
        if (duckPressed && !isDucking && isGrounded && !isJumping)
        {
            isDucking = true;
            if (animator != null) animator.SetBool("Ducking", true);
        }
        else if (!duckPressed && isDucking)
        {
            isDucking = false;
            if (animator != null) animator.SetBool("Ducking", false);
        }
        
        // Check for jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDucking && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            if (animator != null) animator.SetTrigger("Jump");
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Handle movement
        if (mainCamera != null && inputDirection.magnitude > 0.1f)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;
            cameraForward.y = 0; cameraRight.y = 0;
            cameraForward.Normalize(); cameraRight.Normalize();
            
            Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
            
            float currentSpeed;
            if (isDucking)
            {
                currentSpeed = duckSpeed;
            }
            else
            {
                bool isRunning = Input.GetKey(KeyCode.LeftShift);
                currentSpeed = isRunning ? runSpeed : walkSpeed;
            }
            
            // Reduce air control when jumping
            if (!isGrounded)
            {
                currentSpeed *= 0.5f;
            }
            
            // Move the character
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            
            // Rotate to face movement direction
            if (moveDirection.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }

        // Apply gravity
        controller.Move(velocity * Time.deltaTime);

        // Update animator
        UpdateAnimator(horizontal, vertical);
    }
    
    void UpdateAnimator(float horizontal, float vertical)
    {
        if (animator == null) return;
        
        animator.SetBool("IsGrounded", isGrounded);
        
        if (isGrounded && !isDucking && !isJumping)
        {
            float inputMagnitude = new Vector3(horizontal, 0, vertical).magnitude;
            
            if (inputMagnitude > 0.1f)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    animator.SetFloat("Velocity Z", 2.0f); // Run
                }
                else
                {
                    animator.SetFloat("Velocity Z", 1.0f); // Walk
                }
            }
            else
            {
                animator.SetFloat("Velocity Z", 0f); // Idle
            }
        }
        else
        {
            // In air or ducking
            animator.SetFloat("Velocity Z", 0f);
        }
    }
}