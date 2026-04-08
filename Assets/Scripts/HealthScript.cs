using UnityEngine;
using System.Collections; // Necesar pentru Corutine

public class Health : MonoBehaviour
{
    [Header("Life settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Immunity Settings")]
    public float immunityDuration = 0.5f;
    private bool isImmune = false;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damageAmount)
    {
        //If immune, ignore dmg
        if (isImmune) return;

        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " took damage. HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            //If not dead, start immunity timer
            StartCoroutine(ImmunityRoutine());
        }
    }

    //Waiting time
    private IEnumerator ImmunityRoutine()
    {
        isImmune = true;

        //Visual feedback
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

            //wait the immunity
            yield return new WaitForSeconds(immunityDuration);

            //return to normal color
            spriteRenderer.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(immunityDuration);
        }

        isImmune = false;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }
}