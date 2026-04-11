using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    public int contactDamage = 1;

    //No Trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamage(collision.gameObject);
        }
    }

    //Maintain damage
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamage(collision.gameObject);
        }
    }

    //With Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ApplyDamage(collision.gameObject);
        }
    }

    private void ApplyDamage(GameObject target)
    {
        Health h = target.GetComponent<Health>();
        if (h != null)
        {
            h.TakeDamage(contactDamage); // Calls health script
        }
    }
}