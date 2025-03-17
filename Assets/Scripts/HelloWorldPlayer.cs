using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        // NetworkVariable to synchronize paddle position
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        // Speed of paddle movement
        public float speed = 5f;

        public override void OnNetworkSpawn()
        {
            // Set initial position based on player type (Player 1 or Player 2)
            if (IsOwner)
            {
                if (IsHost) // Player 1 (Host)
                {
                    transform.position = new Vector3(-10, 0, 0);
                }
                else // Player 2 (Client)
                {
                    transform.position = new Vector3(10, 0, 0);
                }

                // Update the NetworkVariable to sync the initial position
                Position.Value = transform.position;
            }

            // Subscribe to position changes
            Position.OnValueChanged += OnPositionChanged;
        }

        public override void OnNetworkDespawn()
        {
            // Unsubscribe from position changes
            Position.OnValueChanged -= OnPositionChanged;
        }

        private void OnPositionChanged(Vector3 previous, Vector3 current)
        {
            // Update the paddle's position when the NetworkVariable changes
            if (!IsOwner) // Only update if this is not the owner (to avoid double updates)
            {
                transform.position = current;
            }
        }

        private void Update()
        {
            if (!IsOwner) return; // Only the owner can move the paddle

            // Handle input for paddle movement
            float moveInput = Input.GetAxis("Vertical"); // Use "W" and "S" or arrow keys
            Vector3 moveDirection = new Vector3(0, moveInput, 0) * speed * Time.deltaTime;

            // Move the paddle locally
            transform.Translate(moveDirection);

            // Update the NetworkVariable to sync the new position
            Position.Value = transform.position;
        }
    }
}