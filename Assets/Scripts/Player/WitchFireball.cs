using UnityEngine;
using Unity.Netcode;

public class PlayerFireball : NetworkBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // In multiplayer, doar serverul aplica damage si distruge obiecte
        if (!IsServer) return;

        // Loveste inamici (Ai nevoie de un Layer numit "Enemy")
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Health hp = collision.GetComponent<Health>();
            if (hp != null) hp.TakeDamage(damage);

            // In Netcode, despawnam obiectele in loc de Destroy()
            GetComponent<NetworkObject>().Despawn();
        }
        // Distruge fireball-ul cand atinge pamantul, cum e in scriptul vechi
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}