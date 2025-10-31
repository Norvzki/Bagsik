using UnityEngine;

public class LNPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
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
        
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.center = new Vector3(0, 1, 0);
            controller.height = 2.0f;
        }
        
        if (mainCamera == null) mainCamera = Camera.main;
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

        // Get input - SPECIFIC WASD KEYS ONLY
        float horizontal = 0f;
        float vertical = 0f;
        
        if (Input.GetKey(KeyCode.W)) vertical = 1f;
        if (Input.GetKey(KeyCode.S)) vertical = -1f;
        if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;

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
        if (mainCamera != null && (horizontal != 0 || vertical != 0))
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
            
            if (!isGrounded)
            {
                currentSpeed *= 0.5f;
            }
            
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            
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
            animator.SetFloat("Velocity Z", 0f);
        }
    }
}