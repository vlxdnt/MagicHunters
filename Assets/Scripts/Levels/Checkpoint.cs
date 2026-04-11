using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Setari")]
    public int prioritate = 0;
    public Sprite spriteInactiv; // verde
    public Sprite spriteActiv; // rosu

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetCheckpoint;

    private SpriteRenderer sr;
    private bool esteActiv = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // respawn-ul
        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
        if (respawn == null) return;

        // prin prioritate
        if (prioritate >= respawn.prioritateCheckpoint)
        {
            respawn.SetCheckpoint(transform.position, prioritate);
            ActiveazaCheckpoint();
        }
    }

    void ActiveazaCheckpoint()
    {
        if (esteActiv) return;
        esteActiv = true;

        // sunet
        if (audioSource != null && sunetCheckpoint != null)
            audioSource.PlayOneShot(sunetCheckpoint);

        if (sr != null && spriteActiv != null)
            sr.sprite = spriteActiv;
    }

    public void Dezactiveaza()
    {
        esteActiv = false;
        if (sr != null && spriteInactiv != null)
            sr.sprite = spriteInactiv;
    }
}