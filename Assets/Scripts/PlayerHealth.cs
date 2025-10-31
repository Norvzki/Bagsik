using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public int playerNumber = 1;

    [Header("UI References")]
    public Slider healthBarSlider;

    [Header("Earthquake Damage")]
    public float earthquakeDamagePerSecond = 10f;

    private TwoPlayerController playerController;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<TwoPlayerController>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Die()
    {
        isDead = true;
        Debug.Log($"Player {playerNumber} died!");

        // Disable controls
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Notify GameOverManager
        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.PlayerDied(playerNumber);
        }
        else
        {
            Debug.LogWarning("GameOverManager not found! Make sure it exists in the scene.");
        }
    }

    public bool IsDucking()
    {
        if (playerController != null)
        {
            return playerController.IsDucking();
        }
        return false;
    }

    public bool IsDead()
    {
        return isDead;
    }
}   