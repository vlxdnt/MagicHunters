using UnityEngine;
using System.Collections;

public class DamageOnContact : MonoBehaviour
{
    public int contactDamage = 20;
    public float damageDelay = 3f; // delay

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(DamageDelay(collision.gameObject));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            StartCoroutine(DamageDelay(collision.gameObject));
    }

    IEnumerator DamageDelay(GameObject target)
    {
        yield return new WaitForSeconds(damageDelay);
        ApplyDamage(target);
    }

    private void ApplyDamage(GameObject target)
    {
        Health h = target.GetComponent<Health>();
        if (h != null) h.TakeDamage(contactDamage);
    }
}