using UnityEngine;
using Unity.Netcode;

public class ButtonDoor : NetworkBehaviour
{
    [Header("Usa asociata")]
    public GameObject usa;
    public Sprite spriteUsaDeschisa;
    public Collider2D colliderUsa;

    [Header("Buton")]
    public Sprite spriteButonApasat;
    private SpriteRenderer srButon;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetButon;

    private readonly NetworkVariable<bool> esteApasat = new NetworkVariable<bool>(
        false, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    void Awake()
    {
        srButon = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        esteApasat.OnValueChanged += OnStateChanged;

        if (esteApasat.Value)
        {
            ActualizeazaVizual(true, false);
        }
    }

    public override void OnNetworkDespawn()
    {
        esteApasat.OnValueChanged -= OnStateChanged;
    }

    private void OnStateChanged(bool previousValue, bool newValue)
    {
        ActualizeazaVizual(newValue, true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;
        if (esteApasat.Value) return; 

        if (other.CompareTag("Player"))
        {
            esteApasat.Value = true;
        }
    }

    private void ActualizeazaVizual(bool apasat, bool cuSunet)
    {
        if (!apasat) return;

        if (srButon != null && spriteButonApasat != null)
            srButon.sprite = spriteButonApasat;

        if (cuSunet && audioSource != null && sunetButon != null)
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