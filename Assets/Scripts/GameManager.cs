using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI countdownText;
    public GameObject leaderboardPanel;
    
    [Header("Game Settings")]
    public float countdownTime = 5f;
    public float earthquakeInterval = 10f;
    public float roundDuration = 120f; // 2 minutes
    
    [Header("References")]
    public EarthquakeSimulator earthquakeSimulator;
    
    private float gameTimer = 0f;
    private float earthquakeTimer = 0f;
    private bool gameStarted = false;
    private bool roundEnded = false;
    
    void Start()
    {
        // Hide leaderboard at start
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        
        // Start countdown
        StartCoroutine(StartCountdown());
        
        // Find earthquake simulator if not assigned
        if (earthquakeSimulator == null)
            earthquakeSimulator = FindObjectOfType<EarthquakeSimulator>();
    }
    
    void Update()
    {
        if (!gameStarted || roundEnded) return;
        
        // Update game timer
        gameTimer += Time.deltaTime;
        
        // Trigger earthquake at intervals
        earthquakeTimer += Time.deltaTime;
        if (earthquakeTimer >= earthquakeInterval)
        {
            earthquakeTimer = 0f;
            if (earthquakeSimulator != null)
                earthquakeSimulator.TriggerEarthquake();
        }
        
        // Check if round ended (2 minutes)
        if (gameTimer >= roundDuration)
        {
            EndRound();
        }
    }
    
    System.Collections.IEnumerator StartCountdown()
    {
        float timer = countdownTime;
        
        while (timer > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(timer).ToString();
            
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }
        
        // Show START!
        if (countdownText != null)
        {
            countdownText.text = "START!";
            yield return new WaitForSeconds(1f);
            countdownText.gameObject.SetActive(false);
        }
        
        // Start game
        gameStarted = true;
    }
    
    void EndRound()
    {
        roundEnded = true;
        
        // Show leaderboard
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);
    }
    
    public void CloseLeaderboard()
    {
        // Return to main menu
        SceneManager.LoadScene("Menu");
    }
}
