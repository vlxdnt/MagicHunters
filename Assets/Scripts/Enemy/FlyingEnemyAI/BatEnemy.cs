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

    // Adaugam un timer pentru a nu scana de 150 ori pe secunda (FPS Fix)
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
        float distantaMinima = Mathf.Infinity;
        Transform targetNou = null;

        // OPTIMIZARE + VERIFICARE INVIZIBILITATE (Folosim lista statica creata anterior)
        foreach (WitchAbilities p in WitchAbilities.jucatoriInScena)
        {
            // Daca nu exista sau e invizibil, liliacul il ignora
            if (p == null || p.esteInvizibil.Value == true) 
            {
                continue;
            }

            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < distantaMinima)
            {
                distantaMinima = dist;
                targetNou = p.transform;
            }
        }

        destinationSetter.target = targetNou;

        // Daca nu am gasit pe nimeni vizibil, punem frana brusc
        if (targetNou == null)
        {
            aiPath.isStopped = true;
        }
        else
        {
            aiPath.isStopped = false;
        }
    }

    void Update()
    {
        // Actualizam target-ul DOAR daca sunt jucatori in camera si DOAR o data la 0.5s
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < nextKnockbackTime) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                //aflu directia juc
                float directieX = Mathf.Sign(collision.transform.position.x - transform.position.x);

                //cat de tare e impins
                Vector2 directie = new Vector2(directieX, 0.3f).normalized;

                // forta aplicata
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(directie * forțaKnockback, ForceMode2D.Impulse);

                nextKnockbackTime = Time.time + cooldownKnockback;
            }
        }
    }
}