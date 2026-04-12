using UnityEngine;
using Unity.Netcode;

public class Fireball : NetworkBehaviour
{
    public int damage = 20;
    public float viteza = 7f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (!IsSpawned) return;

        if (collision.CompareTag("Player"))
        {
            Health h = collision.GetComponent<Health>();
            if (h != null)
            {
                h.TakeDamage(damage);
            }

            
            GetComponent<NetworkObject>().Despawn();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}