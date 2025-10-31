using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [Header("Health Bar References")]
    public Slider player1HealthBar;
    public Slider player2HealthBar;

    private PlayerHealth player1Health;
    private PlayerHealth player2Health;

    void Start()
    {
        // Find health bars by name if not assigned
        if (player1HealthBar == null)
        {
            GameObject p1Bar = GameObject.Find("P1_HealthBar");
            if (p1Bar != null)
            {
                player1HealthBar = p1Bar.GetComponent<Slider>();
                Debug.Log("Found P1 Health Bar");
            }
        }

        if (player2HealthBar == null)
        {
            GameObject p2Bar = GameObject.Find("P2_HealthBar");
            if (p2Bar != null)
            {
                player2HealthBar = p2Bar.GetComponent<Slider>();
                Debug.Log("Found P2 Health Bar");
            }
        }

        // Wait a frame for players to spawn, then find them
        Invoke("FindPlayers", 0.1f);
    }

    void FindPlayers()
    {
        // Find all PlayerHealth components
        PlayerHealth[] allPlayers = FindObjectsOfType<PlayerHealth>();

        foreach (PlayerHealth player in allPlayers)
        {
            if (player.playerNumber == 1)
            {
                player1Health = player;
                player.healthBarSlider = player1HealthBar;
                Debug.Log("Assigned health bar to Player 1");
            }
            else if (player.playerNumber == 2)
            {
                player2Health = player;
                player.healthBarSlider = player2HealthBar;
                Debug.Log("Assigned health bar to Player 2");
            }
        }
    }

    void Update()
    {
        // Update health bars
        if (player1Health != null && player1HealthBar != null)
        {
            player1HealthBar.value = player1Health.currentHealth;
            UpdateHealthBarColor(player1HealthBar, player1Health.currentHealth, player1Health.maxHealth);
        }

        if (player2Health != null && player2HealthBar != null)
        {
            player2HealthBar.value = player2Health.currentHealth;
            UpdateHealthBarColor(player2HealthBar, player2Health.currentHealth, player2Health.maxHealth);
        }
    }

    void UpdateHealthBarColor(Slider healthBar, float currentHealth, float maxHealth)
    {
        // Find the Fill image in the slider
        Image fillImage = healthBar.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            float healthPercent = currentHealth / maxHealth;

            if (healthPercent > 0.5f)
                fillImage.color = Color.green;
            else if (healthPercent > 0.25f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }
}