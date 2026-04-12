using UnityEngine;
using Unity.Netcode;

public class EnemyHealth : NetworkBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;
    
    // Variabila noua pentru scut
    public bool isShielded = false;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        // Daca are scutul activ, ignoram lovitura
        if (isShielded)
        {
            Debug.Log(gameObject.name + " a blocat atacul cu scutul!");
            // Aici poti apela un ClientRpc mai tarziu pentru un sunet de "Clang!" (metal)
            return; 
        }

        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (IsServer)
            GetComponent<NetworkObject>().Despawn();
    }
}