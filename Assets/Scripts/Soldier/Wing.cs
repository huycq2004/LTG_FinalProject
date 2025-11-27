using UnityEngine;

public class Wing : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        
        if (other.CompareTag("Player"))
        {
            // Enable double jump
            SoldierController player = other.GetComponent<SoldierController>();
            if (player != null)
            {
                player.isAbleToDoubleJump = true;
    
                // Optional: play sound or particle effect here
                // AudioSource.PlayClipAtPoint(pickupSound, transform.position);

                // Destroy the wing object
                Destroy(gameObject);
            }

        }
    }
}
