using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    public float speed; // Speed of the paddle
    public Rigidbody2D rb; // Reference to the Rigidbody2D component
    public Vector3 startPosition; // Starting position of the paddle

    // Reference to the AudioSource component
    private AudioSource audioSource;

    private float movement; // Input movement value

    void Start()
    {
        startPosition = transform.position;

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the paddle!");
        }
        else
        {
            Debug.Log("AudioSource reference is assigned.");
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Only the owner can control the paddle

        // Determine which input axis to use based on ownership
        if (IsServer && IsOwner) // Host controls Player 1
        {
            movement = Input.GetAxisRaw("Vertical");
            Debug.Log($"Player 1 input: {movement}");
        }
        else if (!IsServer && IsOwner) // Client controls Player 2
        {
            movement = Input.GetAxisRaw("Vertical2");
            Debug.Log($"Player 2 input: {movement}");
        }

        // Apply movement to the paddle
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, movement * speed);
    }

    // Reset the paddle's position and velocity
    public void Reset()
    {
        Debug.Log("Resetting paddle...");
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
    }

    // Detect collisions with the ball
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Paddle collided with the ball.");
            PlayPaddleHitSound();
        }
    }

    // Play the paddle hit sound effect
    private void PlayPaddleHitSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            Debug.Log("Playing paddle hit sound effect.");
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is not set!");
        }
    }
}