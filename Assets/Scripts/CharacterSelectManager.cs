using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Available Characters")]
    public CharacterData[] characters; // Angelo, Cedric, Irish, Norvel, Zerika

    [Header("Selection State")]
    private int player1SelectedIndex = -1;
    private int player2SelectedIndex = -1;
    private bool player1Ready = false;
    private bool player2Ready = false;

    public static CharacterSelectManager instance;

    // Store selected characters to pass to game scene
    public static CharacterData player1Character;
    public static CharacterData player2Character;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsCharacterTaken(int characterIndex, int playerNumber)
    {
        if (playerNumber == 1)
        {
            return player2SelectedIndex == characterIndex && player2Ready;
        }
        else
        {
            return player1SelectedIndex == characterIndex && player1Ready;
        }
    }

    public void SelectCharacter(int playerNumber, int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characters.Length)
            return;

        if (playerNumber == 1)
        {
            player1SelectedIndex = characterIndex;
            player1Ready = false;
        }
        else if (playerNumber == 2)
        {
            player2SelectedIndex = characterIndex;
            player2Ready = false;
        }

        Debug.Log($"Player {playerNumber} selected {characters[characterIndex].characterName}");
    }

    public void ConfirmSelection(int playerNumber)
    {
        if (playerNumber == 1 && player1SelectedIndex >= 0)
        {
            if (!IsCharacterTaken(player1SelectedIndex, 1))
            {
                player1Ready = true;
                Debug.Log($"Player 1 confirmed: {characters[player1SelectedIndex].characterName}");
            }
        }
        else if (playerNumber == 2 && player2SelectedIndex >= 0)
        {
            if (!IsCharacterTaken(player2SelectedIndex, 2))
            {
                player2Ready = true;
                Debug.Log($"Player 2 confirmed: {characters[player2SelectedIndex].characterName}");
            }
        }

        // Check if both players are ready
        if (player1Ready && player2Ready)
        {
            StartGame();
        }
    }

    public void CancelSelection(int playerNumber)
    {
        if (playerNumber == 1)
        {
            player1Ready = false;
        }
        else if (playerNumber == 2)
        {
            player2Ready = false;
        }
    }

    public int GetSelectedIndex(int playerNumber)
    {
        return playerNumber == 1 ? player1SelectedIndex : player2SelectedIndex;
    }

    public bool IsPlayerReady(int playerNumber)
    {
        return playerNumber == 1 ? player1Ready : player2Ready;
    }

    void StartGame()
    {
        // Store selected characters
        player1Character = characters[player1SelectedIndex];
        player2Character = characters[player2SelectedIndex];

        Debug.Log($"Starting game with P1: {player1Character.characterName}, P2: {player2Character.characterName}");

        // Load game scene
        SceneManager.LoadScene("SampleScene");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}