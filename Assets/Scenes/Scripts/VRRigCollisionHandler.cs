using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script ensures the VR rig (representing the player) cannot pass through walls.


public class VRRigCollisionHandler : MonoBehaviour
{
    public float speed = 3f; // Speed of movement for the VR rig
    private Rigidbody vrRigidbody;

    void Start()
    {
        // Ensure the VR rig has a Rigidbody component
        vrRigidbody = GetComponent<Rigidbody>();
        if (vrRigidbody == null)
        {
            vrRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        // Configure the Rigidbody to handle physics interactions
        vrRigidbody.useGravity = false; // VR rigs don't need gravity as they are moved manually
        vrRigidbody.isKinematic = false; // Allow the Rigidbody to be affected by collisions
        vrRigidbody.constraints = RigidbodyConstraints.FreezeRotation; // Prevent the VR rig from rotating unexpectedly
    }

    void FixedUpdate()
    {
        // Handle basic movement of the VR rig
        float horizontal = Input.GetAxis("Horizontal"); // Get horizontal input (e.g., from keyboard or joystick)
        float vertical = Input.GetAxis("Vertical"); // Get vertical input

        // Calculate the movement vector
        Vector3 movement = new Vector3(horizontal, 0, vertical) * speed * Time.fixedDeltaTime;
        Vector3 targetPosition = transform.position + movement;

        // Move the VR rig using Rigidbody to respect collisions
        vrRigidbody.MovePosition(targetPosition);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with other objects
        Debug.Log("Collided with: " + collision.gameObject.name);

        // Example: Provide feedback to the player when colliding with walls
        // You can add haptic feedback, sound effects, or visual indicators here
    }
}

