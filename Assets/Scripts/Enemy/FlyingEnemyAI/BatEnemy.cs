using UnityEngine;
using Pathfinding;

public class BatEnemy : MonoBehaviour
{
    [Header("Setari")]
    public float forțaKnockback = 10f;
    public float cooldownKnockback = 1f;
    public LayerMask playerLayer;

    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;
    private int jucatoriInCamera = 0;
    private float nextKnockbackTime = 0f;

    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();

        aiPath.canMove = false;
    }

    public void JucatorIntrat()
    {
        jucatoriInCamera++;
        ActualizeazaTarget();
        aiPath.canMove = true;
    }

    public void JucatorIesit()
    {
        jucatoriInCamera = Mathf.Max(0, jucatoriInCamera - 1);
        ActualizeazaTarget();

        if (jucatoriInCamera == 0)
        {
            aiPath.canMove = false;
            destinationSetter.target = null;
        }
    }

    void ActualizeazaTarget()
    {
        // cel mai apropiat
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float distantaMinima = Mathf.Infinity;
        Transform targetNou = null;

        foreach (GameObject player in players)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < distantaMinima)
            {
                distantaMinima = dist;
                targetNou = player.transform;
            }
        }

        destinationSetter.target = targetNou;
    }

    void Update()
    {
        // actualizare pt cel mai apropiat
        if (jucatoriInCamera > 0)
            ActualizeazaTarget();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < nextKnockbackTime) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // knockback
                Vector2 directie = (collision.transform.position - transform.position).normalized;
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(directie * forțaKnockback, ForceMode2D.Impulse);

                nextKnockbackTime = Time.time + cooldownKnockback;
            }
        }
    }
}