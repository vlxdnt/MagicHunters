using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetAtacLiliac;

    // timer pt fps fix
    private float intervalCautare = 0.5f;
    private float timerCautare = 0f;

    private List<Transform> jucatoriInZona = new List<Transform>();

    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();

        aiPath.canMove = false;
    }

    public void JucatorIntrat(Transform jucator)
    {
        Debug.Log("JucatorIntrat apelat: " + jucator.name);
        if (!jucatoriInZona.Contains(jucator))
            jucatoriInZona.Add(jucator);

        ActualizeazaTarget();
        aiPath.canMove = true;
    }


    public void JucatorIesit(Transform jucator)
    {
        jucatoriInZona.Remove(jucator);

        if (jucatoriInZona.Count == 0)
        {
            aiPath.canMove = false;
            destinationSetter.target = null;
        }
        else ActualizeazaTarget();
    }

    void ActualizeazaTarget()
    {
        Debug.Log("ActualizeazaTarget - jucatori in zona: " + jucatoriInZona.Count);
        float distantaMinima = Mathf.Infinity;
        Transform targetNou = null;

        foreach (Transform jucator in jucatoriInZona)
        {
            if (jucator == null) continue;

            WitchAbilities witch = jucator.GetComponent<WitchAbilities>();
            if (witch != null && witch.esteInvizibil.Value) continue;

            float dist = Vector2.Distance(transform.position, jucator.position);
            if (dist < distantaMinima)
            {
                distantaMinima = dist;
                targetNou = jucator;
            }
        }
        Debug.Log("Target setat: " + (destinationSetter.target != null ? destinationSetter.target.name : "NULL"));

        destinationSetter.target = targetNou;
        aiPath.isStopped = (targetNou == null);
    }

    void Update()
    {
        // actualizare target
        if (jucatoriInZona.Count > 0)
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

        if (audioSource != null && sunetAtacLiliac != null)
            audioSource.PlayOneShot(sunetAtacLiliac);

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