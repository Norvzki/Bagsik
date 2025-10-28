using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        // Get player index from Player Input component and assign control scheme
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            Debug.Log("Player spawned with index: " + playerInput.playerIndex + " | Control Scheme: " + playerInput.currentControlScheme);

            // Manually assign control scheme based on player index if empty
            if (string.IsNullOrEmpty(playerInput.currentControlScheme))
            {
                if (playerInput.playerIndex == 0)
                {
                    playerInput.SwitchCurrentControlScheme("WASD", Keyboard.current);
                    Debug.Log("Assigned WASD to Player 0");
                }
                else if (playerInput.playerIndex == 1)
                {
                    playerInput.SwitchCurrentControlScheme("Arrows", Keyboard.current);
                    Debug.Log("Assigned Arrows to Player 1");
                }
            }
        }
    }

    void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        if (rb != null)
        {
            rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    // Called automatically by Player Input component when using "Send Messages"
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}