using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Spawn Player 1 for the host
            SpawnPlayer1();
            // Listen for new client connections
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            // Stop listening for client connections
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void SpawnPlayer1()
    {
        // Get the Player 1 prefab from the NetworkManager's NetworkPrefabs list
        var player1Prefab = NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs[0].Prefab;

        // Spawn Player 1 for the host
        GameObject player1 = Instantiate(player1Prefab, new Vector3(-10, 0, 0), Quaternion.identity);
        player1.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.Singleton.LocalClientId);

        Debug.Log("Player 1 spawned for host.");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            // Check if the connected client is NOT the host
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                // Spawn Player 2 for the remote client
                SpawnPlayer2(clientId);
            }
        }
    }

    private void SpawnPlayer2(ulong clientId)
    {
        // Get the Player 2 prefab from the NetworkManager's NetworkPrefabs list
        var player2Prefab = NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs[1].Prefab;

        // Spawn Player 2 for the remote client
        GameObject player2 = Instantiate(player2Prefab, new Vector3(10, 0, 0), Quaternion.identity);
        player2.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

        Debug.Log($"Player 2 spawned for client with ID: {clientId}");
        SpawnBall();
    }

    private void SpawnBall()
    {
    var ballPrefab = NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs[2].Prefab; // Assuming the ball is the third prefab
    GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
    ball.GetComponent<NetworkObject>().Spawn();
    }
}