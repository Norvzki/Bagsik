using UnityEngine;

public class TwoPlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public int playerNumber = 1; // 1 for WASD, 2 for Arrow Keys
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -20f;
    public float doubleTapTime = 0.3f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isSprinting = false;
    private bool isDucking = false;
    private bool wasDucking = false;
    private float lastTapTime = 0f;

    private float lastTapTime = 0f;

    // for the audio
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
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Ground check and gravity reset
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = 0f;
        float vertical = 0f;

        // Get input based on player number
        if (playerNumber == 1)
        {
            horizontal = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);
            vertical = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
            
            // Double tap any WASD key for sprint
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || 
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                if (Time.time - lastTapTime < doubleTapTime)
                {
                    isSprinting = true;
                }
                lastTapTime = Time.time;
            }
            
            // Stop sprinting when all WASD keys are released
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && 
                !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                isSprinting = false;
            }
            
            // Jump with Spacebar
            if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            
            // Duck with E key
            isDucking = Input.GetKey(KeyCode.E);
            
            // Handle ducking animation freeze
            if (isDucking && !wasDucking)
            {
                // Just started ducking - play animation normally
                if (animator != null)
                {
                    animator.speed = 1f;
                }
            }
            else if (!isDucking && wasDucking)
            {
                // Released duck button - resume normal animation
                if (animator != null)
                {
                    animator.speed = 1f;
                }
            }
            
            wasDucking = isDucking;
                // for the audio
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
            }
        }
        else if (playerNumber == 2)
        {
            horizontal = Input.GetKey(KeyCode.LeftArrow) ? -1f : (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);
            vertical = Input.GetKey(KeyCode.UpArrow) ? 1f : (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f);
            
            // Double tap any Arrow key for sprint
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || 
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (Time.time - lastTapTime < doubleTapTime)
                {
                    isSprinting = true;
                }
                lastTapTime = Time.time;
            }
            
            // Stop sprinting when all Arrow keys are released
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && 
                !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                isSprinting = false;
            }
            
            // Jump with Numpad 0
            if (Input.GetKeyDown(KeyCode.Keypad0) && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            
            // Duck with Right Ctrl key
            isDucking = Input.GetKey(KeyCode.RightControl);
            
            // Handle ducking animation freeze
            if (isDucking && !wasDucking)
            {
                // Just started ducking - play animation normally
                if (animator != null)
                {
                    animator.speed = 1f;
                }
            }
            else if (!isDucking && wasDucking)
            {
                // Released duck button - resume normal animation
                if (animator != null)
                {
                    animator.speed = 1f;
                }
            }
            
            wasDucking = isDucking;
        }

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Determine speed and animation
        float currentSpeed = walkSpeed;
        float animSpeed = 0.5f; // Walk animation speed

        if (moveDirection.magnitude > 0.1f)
        {
            if (isSprinting)
            {
                currentSpeed = runSpeed;
                animSpeed = 1.0f; // Run animation speed
            }

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);

            // Rotate character to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Set animation speed
            if (animator != null)
            {
                animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
                animator.SetBool("IsDucking", isDucking);
                
                // Freeze animation on last frame if ducking
                if (isDucking)
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    // Check if we're in a ducking animation state and it has completed
                    if (stateInfo.IsName("Duck") || stateInfo.IsName("Ducking") || stateInfo.IsName("Crouch"))
                    {
                        if (stateInfo.normalizedTime >= 0.99f)
                        {
                            animator.speed = 0f;
                        }
                    }
                }
            }
        }
        else
        {
            // Idle animation
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
                animator.SetBool("IsDucking", isDucking);
                
                // Freeze animation on last frame if ducking
                if (isDucking)
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    // Check if we're in a ducking animation state and it has completed
                    if (stateInfo.IsName("Duck") || stateInfo.IsName("Ducking") || stateInfo.IsName("Crouch"))
                    {
                        if (stateInfo.normalizedTime >= 0.99f)
                        {
                            animator.speed = 0f;
                        }
                    }
                }
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
