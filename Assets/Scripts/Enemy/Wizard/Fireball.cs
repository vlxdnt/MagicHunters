using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for player hit
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log(collision.gameObject.name + " hit for " + damage + " HP!");
            }

            Destroy(gameObject);
        }

        // Destroy on wall/ground hit
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}