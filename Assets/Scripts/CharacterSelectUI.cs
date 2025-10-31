using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Player 1 UI")]
    public Image[] player1CharacterSlots;
    public GameObject player1ReadyIndicator;
    public Color player1HighlightColor = Color.blue;

    [Header("Player 2 UI")]
    public Image[] player2CharacterSlots;
    public GameObject player2ReadyIndicator;
    public Color player2HighlightColor = Color.red;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color takenColor = Color.gray;

    private CharacterSelectManager selectManager;
    private int player1CurrentIndex = 0;
    private int player2CurrentIndex = 1;

    void Start()
    {
        selectManager = CharacterSelectManager.instance;

        if (selectManager == null)
        {
            Debug.LogError("CharacterSelectManager not found!");
            return;
        }

        // Initialize character icons
        for (int i = 0; i < selectManager.characters.Length; i++)
        {
            if (i < player1CharacterSlots.Length)
            {
                player1CharacterSlots[i].sprite = selectManager.characters[i].characterIcon;
            }
            if (i < player2CharacterSlots.Length)
            {
                player2CharacterSlots[i].sprite = selectManager.characters[i].characterIcon;
            }
        }

        // Set initial selections
        selectManager.SelectCharacter(1, player1CurrentIndex);
        selectManager.SelectCharacter(2, player2CurrentIndex);

        UpdateUI();
    }

    void Update()
    {
        HandlePlayer1Input();
        HandlePlayer2Input();
        UpdateUI();
    }

    void HandlePlayer1Input()
    {
        if (selectManager.IsPlayerReady(1))
        {
            // Cancel selection
            if (Input.GetKeyDown(KeyCode.E))
            {
                selectManager.CancelSelection(1);
            }
            return;
        }

        // Navigate left
        if (Input.GetKeyDown(KeyCode.A))
        {
            player1CurrentIndex--;
            if (player1CurrentIndex < 0)
                player1CurrentIndex = selectManager.characters.Length - 1;
            selectManager.SelectCharacter(1, player1CurrentIndex);
        }

        // Navigate right
        if (Input.GetKeyDown(KeyCode.D))
        {
            player1CurrentIndex++;
            if (player1CurrentIndex >= selectManager.characters.Length)
                player1CurrentIndex = 0;
            selectManager.SelectCharacter(1, player1CurrentIndex);
        }

        // Confirm selection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!selectManager.IsCharacterTaken(player1CurrentIndex, 1))
            {
                selectManager.ConfirmSelection(1);
            }
        }
    }

    void HandlePlayer2Input()
    {
        if (selectManager.IsPlayerReady(2))
        {
            // Cancel selection
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                selectManager.CancelSelection(2);
            }
            return;
        }

        // Navigate left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            player2CurrentIndex--;
            if (player2CurrentIndex < 0)
                player2CurrentIndex = selectManager.characters.Length - 1;
            selectManager.SelectCharacter(2, player2CurrentIndex);
        }

        // Navigate right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            player2CurrentIndex++;
            if (player2CurrentIndex >= selectManager.characters.Length)
                player2CurrentIndex = 0;
            selectManager.SelectCharacter(2, player2CurrentIndex);
        }

        // Confirm selection
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            if (!selectManager.IsCharacterTaken(player2CurrentIndex, 2))
            {
                selectManager.ConfirmSelection(2);
            }
        }
    }

    void UpdateUI()
    {
        // Update Player 1 UI
        for (int i = 0; i < player1CharacterSlots.Length; i++)
        {
            if (selectManager.IsPlayerReady(1) && i == player1CurrentIndex)
            {
                player1CharacterSlots[i].color = selectedColor;
            }
            else if (i == player1CurrentIndex)
            {
                player1CharacterSlots[i].color = player1HighlightColor;
            }
            else if (selectManager.IsCharacterTaken(i, 1))
            {
                player1CharacterSlots[i].color = takenColor;
            }
            else
            {
                player1CharacterSlots[i].color = normalColor;
            }
        }

        // Update Player 2 UI
        for (int i = 0; i < player2CharacterSlots.Length; i++)
        {
            if (selectManager.IsPlayerReady(2) && i == player2CurrentIndex)
            {
                player2CharacterSlots[i].color = selectedColor;
            }
            else if (i == player2CurrentIndex)
            {
                player2CharacterSlots[i].color = player2HighlightColor;
            }
            else if (selectManager.IsCharacterTaken(i, 2))
            {
                player2CharacterSlots[i].color = takenColor;
            }
            else
            {
                player2CharacterSlots[i].color = normalColor;
            }
        }

        // Update ready indicators
        if (player1ReadyIndicator != null)
        {
            player1ReadyIndicator.SetActive(selectManager.IsPlayerReady(1));
        }
        if (player2ReadyIndicator != null)
        {
            player2ReadyIndicator.SetActive(selectManager.IsPlayerReady(2));
        }
    }
}