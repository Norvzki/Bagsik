using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI[] playerNames;
    public TextMeshProUGUI[] playerScores;
    public Button closeButton;
    
    [Header("Placeholder Data")]
    private string[] placeholderNames = { "Player 1", "Player 2", "Player 3", "Player 4", "Player 5" };
    private int[] placeholderScores = { 1000, 850, 700, 550, 400 };
    
    void Start()
    {
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }
        
        // Display placeholder data
        DisplayLeaderboard();
    }
    
    void DisplayLeaderboard()
    {
        for (int i = 0; i < playerNames.Length && i < placeholderNames.Length; i++)
        {
            if (playerNames[i] != null)
                playerNames[i].text = placeholderNames[i];
            
            if (playerScores[i] != null)
                playerScores[i].text = placeholderScores[i].ToString();
        }
    }
    
    void OnCloseClicked()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
            gameManager.CloseLeaderboard();
    }
}
