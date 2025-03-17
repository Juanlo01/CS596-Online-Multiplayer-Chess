using Unity.Netcode;
using UnityEngine;

public class NetworkGoal : NetworkBehaviour
{
    public bool isPlayer1Goal;
    public NetworkGameManager gameManager; // Reference to the GameManager
    public Ball ball; // Reference to the ball

    // Reference to the AudioSource component
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the goal!");
        }
        else
        {
            Debug.Log("AudioSource reference is assigned.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball entered goal trigger.");
            if (IsServer) // Only the server should handle scoring
            {
                if (!isPlayer1Goal)
                {
                    Debug.Log("Player 1 Scored...");
                    gameManager.Player1Scored();
                    PlayGoalSound(); // Play the goal sound effect

                    // Check if Player 1 has won
                    if (gameManager.Player1Score.Value >= 5)
                    {
                        Debug.Log("Player 1 Wins! Resetting game...");
                        gameManager.ResetGame();
                    }
                }
                else
                {
                    Debug.Log("Player 2 Scored...");
                    gameManager.Player2Scored();
                    PlayGoalSound(); // Play the goal sound effect

                    // Check if Player 2 has won
                    if (gameManager.Player2Score.Value >= 5)
                    {
                        Debug.Log("Player 2 Wins! Resetting game...");
                        gameManager.ResetGame();
                    }
                }
            }
            else
            {
                // Notify the server that a goal was scored
                if (!isPlayer1Goal)
                {
                    Debug.Log("Client detected Player 1 Scored...");
                    gameManager.Player1ScoredServerRpc();
                }
                else
                {
                    Debug.Log("Client detected Player 2 Scored...");
                    gameManager.Player2ScoredServerRpc();
                }
            }
        }
    }

    // Play the goal sound effect
    private void PlayGoalSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            Debug.Log("Playing goal sound effect.");
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is not set!");
        }
    }
}