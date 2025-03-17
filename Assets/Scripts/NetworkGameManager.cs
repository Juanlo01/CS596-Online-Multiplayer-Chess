using Unity.Netcode;
using UnityEngine;
using TMPro;

public class NetworkGameManager : NetworkBehaviour
{
    [Header("Ball")]
    public GameObject ball;

    [Header("Player 1")]
    public GameObject player1Paddle;
    public GameObject player1Goal;

    [Header("Player 2")]
    public GameObject player2Paddle;
    public GameObject player2Goal;

    [Header("Score UI")]
    public GameObject Player1Text;
    public GameObject Player2Text;

    // NetworkVariables to synchronize scores
    public NetworkVariable<int> Player1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> Player2Score = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        // Subscribe to score changes
        Player1Score.OnValueChanged += OnPlayer1ScoreChanged;
        Player2Score.OnValueChanged += OnPlayer2ScoreChanged;

        // Initialize UI with current scores
        UpdateScoreUI();
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe from score changes
        Player1Score.OnValueChanged -= OnPlayer1ScoreChanged;
        Player2Score.OnValueChanged -= OnPlayer2ScoreChanged;
    }

    private void OnPlayer1ScoreChanged(int oldValue, int newValue)
    {
        Debug.Log($"Player 1 score changed from {oldValue} to {newValue}");
        UpdateScoreUI();

        // Check if Player 1 has won
        if (newValue >= 5)
        {
            Debug.Log("Player 1 Wins! Resetting game...");
            ResetGame();
        }
    }

    private void OnPlayer2ScoreChanged(int oldValue, int newValue)
    {
        Debug.Log($"Player 2 score changed from {oldValue} to {newValue}");
        UpdateScoreUI();

        // Check if Player 2 has won
        if (newValue >= 5)
        {
            Debug.Log("Player 2 Wins! Resetting game...");
            ResetGame();
        }
    }

    private void UpdateScoreUI()
    {
        Player1Text.GetComponent<TextMeshProUGUI>().text = Player1Score.Value.ToString();
        Player2Text.GetComponent<TextMeshProUGUI>().text = Player2Score.Value.ToString();
    }

    public void Player1Scored()
    {
        if (IsServer) // Only the server can update scores
        {
            Debug.Log("Player 1 Scored. Resetting positions...");
            Player1Score.Value++;
            ResetPosition();
        }
    }

    public void Player2Scored()
    {
        if (IsServer) // Only the server can update scores
        {
            Debug.Log("Player 2 Scored. Resetting positions...");
            Player2Score.Value++;
            ResetPosition();
        }
    }

    [ServerRpc]
    public void Player1ScoredServerRpc()
    {
        Player1Scored();
    }

    [ServerRpc]
    public void Player2ScoredServerRpc()
    {
        Player2Scored();
    }

    private void ResetPosition()
    {
        if (IsServer) // Only the server can reset positions
        {
            Debug.Log("Resetting ball and paddles...");
            ball.GetComponent<NetworkBall>().Reset();
            player1Paddle.GetComponent<NetworkTransformTest>().Reset();
            player2Paddle.GetComponent<NetworkTransformTest>().Reset();
        }
    }

    public void ResetGame()
    {
        if (IsServer) // Only the server can reset the game
        {
            Debug.Log("Resetting game...");

            // Reset scores
            Player1Score.Value = 0;
            Player2Score.Value = 0;

            // Reset positions
            ResetPosition();
        }
    }
}