using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificăm dacă am lovit jucătorul
        // (Asigură-te că prefab-ul Wizard are Tag-ul "Player")
        if (collision.CompareTag("Player"))
        {
            // Aici apelezi metoda de luat viață a jucătorului
            // Exemplu: collision.GetComponent<PlayerHealth>().TakeDamage(damage);

            Debug.Log("Player hit");

            // Distrugem fireball-ul la impact
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}