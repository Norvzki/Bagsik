using UnityEngine;

public class Theelf2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float duckSpeed = 2.0f;
    public float jumpHeight = 2.0f;
    
    [Header("References")]
    public Camera mainCamera;
    
    private Animator animator;
    private CharacterController controller;
    private bool isDucking = false;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    void Update()
    {
        HandleGroundCheck();
        HandleInput();
        HandleMovement();
        HandleGravity();
        UpdateAnimator();
    }
    
    void HandleGroundCheck()
    {
        isGrounded = controller != null && controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    
    void HandleInput()
    {
        // Duck with Number 1
        if (Input.GetKey(KeyCode.Alpha1) && isGrounded && !isDucking)
        {
            isDucking = true;
        }
        else if (!Input.GetKey(KeyCode.Alpha1) && isDucking)
        {
            isDucking = false;
        }
        
        // Jump with Number 0
        if (Input.GetKeyDown(KeyCode.Alpha0) && isGrounded && !isDucking)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }

    
    
    void HandleMovement()
    {
        if (mainCamera == null) return;
        
        // USE SPECIFIC ARROW KEY CHECKS INSTEAD OF AXES
        float horizontal = 0f;
        float vertical = 0f;
        
        // Arrow key input (ONLY arrows, not WASD)
        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;
        
        if (horizontal != 0 || vertical != 0)
        {
            // Camera-relative movement
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
            
            // Calculate speed
            float currentSpeed = walkSpeed;
            if (Input.GetKey(KeyCode.RightShift) && !isDucking) // Run with Right Shift
            {
                currentSpeed = runSpeed;
            }
            else if (isDucking)
            {
                currentSpeed = duckSpeed;
            }
            
            // Apply movement
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            
            // Rotate character
            if (moveDirection.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
    }
    
    void HandleGravity()
    {
        if (controller != null)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
    
    void UpdateAnimator()
    {
        if (animator == null) return;
        
        // Set ducking state
        animator.SetBool("Ducking", isDucking);
        
        // Check arrow keys for animations (same as movement)
        float horizontal = 0f;
        float vertical = 0f;
        
        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;
        
        float inputMagnitude = new Vector3(horizontal, 0, vertical).magnitude;
        
        if (isGrounded && !isDucking)
        {
            if (inputMagnitude > 0.1f)
            {
                if (Input.GetKey(KeyCode.RightShift))
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