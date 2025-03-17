using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkBall : NetworkBehaviour
{
    public float speed; // Speed of the ball
    public Rigidbody2D rb; // Reference to the Rigidbody2D component

    // Reference to the NetworkTransform component
    private NetworkTransform netTransform;

    // Reference to the AudioSource component
    private AudioSource audioSource;

    void Start()
    {
        Debug.Log("Ball Start called for: " + gameObject.name);

        // Ensure Rigidbody2D is assigned
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D reference is not set in the Inspector.");
            return;
        }
        else
        {
            Debug.Log("Rigidbody2D reference is assigned.");
        }

        // Get the NetworkTransform component
        netTransform = GetComponent<NetworkTransform>();
        if (netTransform == null)
        {
            Debug.LogError("NetworkTransform component is missing on the ball!");
            return;
        }
        else
        {
            Debug.Log("NetworkTransform reference is assigned.");
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the ball!");
            return;
        }
        else
        {
            Debug.Log("AudioSource reference is assigned.");
        }

        // Initialize the ball's position on the server
        if (IsServer)
        {
            Debug.Log("Server is initializing the ball...");
            Reset(); // Reset and launch the ball
        }
        else
        {
            Debug.Log("Client detected ball; waiting for server to initialize.");
        }
    }

    // Reset the ball's position and velocity
    public void Reset()
    {
        if (IsServer)
        {
            Debug.Log("Resetting ball on the server: " + gameObject.name);
            rb.linearVelocity = Vector2.zero; // Stop the ball

            // Reset the ball's position using NetworkTransform
            Debug.Log("Teleporting ball to (0, 0, 0)...");
            netTransform.Teleport(Vector3.zero, Quaternion.identity, Vector3.one);
            Debug.Log("Ball position after reset: " + transform.position);

            Launch(); // Launch the ball again
        }
    }

    // Launch the ball in a random direction
    private void Launch()
    {
        if (IsServer)
        {
            Debug.Log("Launching ball...");
            float x = Random.Range(0, 2) == 0 ? -1 : 1; // Random horizontal direction
            float y = Random.Range(0, 2) == 0 ? -1 : 1; // Random vertical direction
            Vector2 velocity = new Vector2(speed * x, speed * y); // Calculate velocity
            rb.linearVelocity = velocity; // Apply velocity
            Debug.Log($"Ball velocity after launch: {velocity}");
        }
    }

    // Detect collisions with walls
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Ball collided with a wall.");
            PlayBounceSound();
        }
    }

    // Play the bounce sound effect
    private void PlayBounceSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            Debug.Log("Playing bounce sound effect.");
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is not set!");
        }
    }
}