using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [Header("Usa asociata")]
    public GameObject usa;
    public Sprite spriteUsaDeschisa;
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
        if (esteApasat) return;
        if (!other.CompareTag("Player")) return;

        Unity.Netcode.NetworkObject netObj = other.GetComponent<Unity.Netcode.NetworkObject>();
        if (netObj == null || !netObj.IsOwner) return;

        // Apasa local
        ApasaButon();

        // Notifica prin jucator
        ButtonPressConnector connector = other.GetComponent<ButtonPressConnector>();
        if (connector != null) connector.ApasaButonServerRpc(gameObject.name);
    }

    public void ApasaButon()
    {
        if (esteApasat) return;
        esteApasat = true;

        if (srButon != null && spriteButonApasat != null)
            srButon.sprite = spriteButonApasat;

        if (audioSource != null && sunetButon != null)
            audioSource.PlayOneShot(sunetButon);

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