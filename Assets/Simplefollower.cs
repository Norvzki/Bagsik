using UnityEngine;

public class Simplefollower : MonoBehaviour
{
    [Header("Follower Settings")]
    public float followDistance = 3.0f;
    public float detectionRange = 5.0f;
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 2.0f;
    
    [Header("References")]
    public Transform player;
    public Animator animator;
    
    private bool isFollowing = false;
    private CharacterController followerController;
    
    void Start()
    {
        // Get or add CharacterController
        followerController = GetComponent<CharacterController>();
        if (followerController == null)
        {
            followerController = gameObject.AddComponent<CharacterController>();
            followerController.center = new Vector3(0, 1, 0);
            followerController.height = 2.0f;
        }
        
        // Find player automatically
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Found player: " + player.name);
            }
            else
            {
                Debug.LogError("No player found! Make sure your player has the 'Player' tag.");
            }
        }
        
        // Get animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator found on NPC!");
            }
        }
        
        Debug.Log("NPC Follower initialized");
    }
    
    void Update()
    {
        if (player == null) 
        {
            Debug.Log("Player reference is null");
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Start following when player is close
        if (distanceToPlayer <= detectionRange && !isFollowing)
        {
            isFollowing = true;
            Debug.Log("NPC started following player! Distance: " + distanceToPlayer);
        }
        
        // Stop following if player gets too far
        if (isFollowing && distanceToPlayer > detectionRange * 2f)
        {
            isFollowing = false;
            Debug.Log("NPC stopped following - player too far! Distance: " + distanceToPlayer);
        }
        
        if (isFollowing)
        {
            FollowPlayer(distanceToPlayer);
        }
        else
        {
            // Not following - idle animation
            UpdateAnimator(0f);
        }
    }
    
    void FollowPlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > followDistance)
        {
            // Calculate direction to player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            
            // Move toward player
            followerController.Move(direction * moveSpeed * Time.deltaTime);
            
            // Rotate to face player
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
            // Walk animation
            UpdateAnimator(1f);
        }
        else
        {
            // Close enough - stop moving but face player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
            // Idle animation
            UpdateAnimator(0f);
        }
    }
    
    void UpdateAnimator(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Velocity Z", speed);
        }
    }
    
    // Visualize ranges in Scene view
    void OnDrawGizmosSelected()
    {
        // Detection range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Follow distance (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }
}