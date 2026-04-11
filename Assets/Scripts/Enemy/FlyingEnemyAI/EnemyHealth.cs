using UnityEngine;
using Unity.Netcode;

public class EnemyHealth : NetworkBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (IsServer)
            GetComponent<NetworkObject>().Despawn();
    }
}