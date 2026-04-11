using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [Header("Usa asociata")]
    public GameObject usa;
    public Sprite spriteUsaDeschisa;
    public Sprite spriteUsaInchisa;
    public Collider2D colliderUsa;

    [Header("Buton")]
    public Sprite spriteButonApasat;
    private SpriteRenderer srButon;
    private bool esteApasat = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetButon;

    void Awake()
    {
        srButon = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (esteApasat) return;

        esteApasat = true;

        // sunet
        if (audioSource != null)
        {
            if (sunetButon != null) audioSource.PlayOneShot(sunetButon);
        }

        // sprite buton
        if (srButon != null && spriteButonApasat != null)
            srButon.sprite = spriteButonApasat;

        // deschide usa
        if (usa != null)
        {
            SpriteRenderer srUsa = usa.GetComponent<SpriteRenderer>();
            if (srUsa != null && spriteUsaDeschisa != null)
                srUsa.sprite = spriteUsaDeschisa;

            if (colliderUsa != null)
                colliderUsa.enabled = false;
        }
    }
}