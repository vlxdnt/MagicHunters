using UnityEngine;
using Unity.Netcode;

public class PlayerFireball : NetworkBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        Debug.Log("Fireball a lovit: " + collision.gameObject.name + " Layer: " + collision.gameObject.layer);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Health hp = collision.GetComponent<Health>();
            if (hp != null) hp.TakeDamage(damage);

            TargetDoor target = collision.GetComponent<TargetDoor>();
            if (target != null) target.LovesteTarget();

            EnemyHealth ehp = collision.GetComponent<EnemyHealth>();
            if (ehp != null) ehp.TakeDamage(damage);

            GetComponent<NetworkObject>().Despawn();
        }
        // distruge fb
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}