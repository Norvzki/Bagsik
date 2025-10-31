using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;

    void Start()
    {
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        // Get selected characters from CharacterSelectManager
        CharacterData p1Char = CharacterSelectManager.player1Character;
        CharacterData p2Char = CharacterSelectManager.player2Character;

        if (p1Char == null || p2Char == null)
        {
            Debug.LogError("Character data not found! Make sure to go through character select first.");
            return;
        }

        // Spawn Player 1
        if (p1Char.characterPrefab != null && player1SpawnPoint != null)
        {
            GameObject p1 = Instantiate(p1Char.characterPrefab, player1SpawnPoint.position, player1SpawnPoint.rotation);
            
            // Ensure CharacterController exists
            CharacterController p1CharController = p1.GetComponent<CharacterController>();
            if (p1CharController == null)
            {
                p1CharController = p1.AddComponent<CharacterController>();
                p1CharController.center = new Vector3(0, 1, 0);
                p1CharController.radius = 0.5f;
                p1CharController.height = 2f;
                Debug.Log("Added CharacterController to Player 1");
            }
            
            TwoPlayerController p1Controller = p1.GetComponent<TwoPlayerController>();
            if (p1Controller != null)
            {
                p1Controller.playerNumber = 1;
                p1Controller.walkSpeed = p1Char.walkSpeed;
                p1Controller.runSpeed = p1Char.runSpeed;
                p1Controller.jumpHeight = p1Char.jumpHeight;
            }
            
            // Add PlayerHealth component
            PlayerHealth p1Health = p1.GetComponent<PlayerHealth>();
            if (p1Health == null)
            {
                p1Health = p1.AddComponent<PlayerHealth>();
            }
            p1Health.playerNumber = 1;

            // Set animator controller
            Animator p1Animator = p1.GetComponent<Animator>();
            if (p1Animator == null)
            {
                p1Animator = p1.AddComponent<Animator>();
                Debug.Log("Added Animator to Player 1");
            }
            
            // Find and assign Avatar from the model
            if (p1Animator.avatar == null)
            {
                Animator[] childAnimators = p1.GetComponentsInChildren<Animator>();
                foreach (Animator childAnim in childAnimators)
                {
                    if (childAnim.avatar != null)
                    {
                        p1Animator.avatar = childAnim.avatar;
                        Debug.Log($"Assigned Avatar to Player 1: {childAnim.avatar.name}");
                        break;
                    }
                }
            }
            
            if (p1Animator != null && p1Char.animatorController != null)
            {
                p1Animator.runtimeAnimatorController = p1Char.animatorController;
                Debug.Log($"Set animator controller for Player 1: {p1Char.animatorController.name}");
            }
            else if (p1Char.animatorController == null)
            {
                Debug.LogWarning("Player 1 CharacterData has no animator controller assigned!");
            }
            
            if (p1Animator.avatar == null)
            {
                Debug.LogWarning("Player 1 has no Avatar assigned! Animations may not work.");
            }

            Debug.Log($"Spawned Player 1: {p1Char.characterName}");
        }

        // Spawn Player 2
        if (p2Char.characterPrefab != null && player2SpawnPoint != null)
        {
            GameObject p2 = Instantiate(p2Char.characterPrefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
            
            // Ensure CharacterController exists
            CharacterController p2CharController = p2.GetComponent<CharacterController>();
            if (p2CharController == null)
            {
                p2CharController = p2.AddComponent<CharacterController>();
                p2CharController.center = new Vector3(0, 1, 0);
                p2CharController.radius = 0.5f;
                p2CharController.height = 2f;
                Debug.Log("Added CharacterController to Player 2");
            }
            
            TwoPlayerController p2Controller = p2.GetComponent<TwoPlayerController>();
            if (p2Controller != null)
            {
                p2Controller.playerNumber = 2;
                p2Controller.walkSpeed = p2Char.walkSpeed;
                p2Controller.runSpeed = p2Char.runSpeed;
                p2Controller.jumpHeight = p2Char.jumpHeight;
            }
            
            // Add PlayerHealth component
            PlayerHealth p2Health = p2.GetComponent<PlayerHealth>();
            if (p2Health == null)
            {
                p2Health = p2.AddComponent<PlayerHealth>();
            }
            p2Health.playerNumber = 2;

            // Set animator controller
            Animator p2Animator = p2.GetComponent<Animator>();
            if (p2Animator == null)
            {
                p2Animator = p2.AddComponent<Animator>();
                Debug.Log("Added Animator to Player 2");
            }
            
            // Find and assign Avatar from the model
            if (p2Animator.avatar == null)
            {
                Animator[] childAnimators = p2.GetComponentsInChildren<Animator>();
                foreach (Animator childAnim in childAnimators)
                {
                    if (childAnim.avatar != null)
                    {
                        p2Animator.avatar = childAnim.avatar;
                        Debug.Log($"Assigned Avatar to Player 2: {childAnim.avatar.name}");
                        break;
                    }
                }
            }
            
            if (p2Animator != null && p2Char.animatorController != null)
            {
                p2Animator.runtimeAnimatorController = p2Char.animatorController;
                Debug.Log($"Set animator controller for Player 2: {p2Char.animatorController.name}");
            }
            else if (p2Char.animatorController == null)
            {
                Debug.LogWarning("Player 2 CharacterData has no animator controller assigned!");
            }
            
            if (p2Animator.avatar == null)
            {
                Debug.LogWarning("Player 2 has no Avatar assigned! Animations may not work.");
            }

            Debug.Log($"Spawned Player 2: {p2Char.characterName}");
        }
    }
}