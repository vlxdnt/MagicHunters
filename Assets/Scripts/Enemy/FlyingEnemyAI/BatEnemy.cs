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

    [Header("Damage")]
    public int damageLaAtac = 10;

    // timer pt fps fix
    private float intervalCautare = 0.5f;
    private float timerCautare = 0f;

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
        
        if (jucatoriInCamera == 0)
        {
            aiPath.canMove = false;
            destinationSetter.target = null;
        }
        else
        {
            ActualizeazaTarget();
        }
    }

    void ActualizeazaTarget()
    {
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
        aiPath.isStopped = (targetNou == null);
    }

    void Update()
    {
        // actualizare target
        if (jucatoriInCamera > 0)
        {
            timerCautare -= Time.deltaTime;
            if (timerCautare <= 0f)
            {
                ActualizeazaTarget();
                timerCautare = intervalCautare;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < nextKnockbackTime) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float directieX = Mathf.Sign(other.transform.position.x - transform.position.x);
            Vector2 directie = new Vector2(directieX, 0.3f).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(directie * forțaKnockback, ForceMode2D.Impulse);
        }

        // da damage
        Health health = other.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damageLaAtac);

        nextKnockbackTime = Time.time + cooldownKnockback;
    }
}