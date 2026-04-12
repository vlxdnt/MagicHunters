using UnityEngine;
using Unity.Netcode;

public class Fireball : NetworkBehaviour
{
    public int damage = 20;
    public float viteza = 7f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // REGULA DE AUR: Doar serverul calculează logica de damage și distrugere
        if (!IsServer) return;

        // VERIFICARE DE SIGURANȚĂ: Nu facem nimic dacă obiectul nu este încă pe rețea
        if (!IsSpawned) return;

        if (collision.CompareTag("Player"))
        {
            Health h = collision.GetComponent<Health>();
            if (h != null)
            {
                h.TakeDamage(damage);
            }

            Debug.Log("Jucator lovit de fireball-ul inamicului!");
            
            GetComponent<NetworkObject>().Despawn();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}