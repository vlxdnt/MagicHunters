using UnityEngine;
using Unity.Netcode;

public class EnemyHealth : NetworkBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;
    
    public bool isShielded = false;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        if (isShielded) return;

        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            GetComponent<NetworkObject>().Despawn();
        else
            gameObject.SetActive(false);
    }
}