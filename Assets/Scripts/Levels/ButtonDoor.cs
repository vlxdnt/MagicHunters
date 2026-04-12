using UnityEngine;
using Unity.Netcode;

public class ButtonDoor : NetworkBehaviour
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
        if (esteApasat) return;

        if (other.CompareTag("Player"))
        {
            NetworkObject playerNetObj = other.GetComponent<NetworkObject>();

            if (playerNetObj != null && playerNetObj.IsOwner)
            {
                ApasaButonServerRpc();
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ApasaButonServerRpc()
    {
        DeschideUsaClientRpc();
    }

    [ClientRpc]
    private void DeschideUsaClientRpc()
    {
        if (esteApasat) return;
        esteApasat = true;

        if (srButon != null && spriteButonApasat != null)
            srButon.sprite = spriteButonApasat;

        if(audioSource != null)
        {
            if (sunetButon != null) audioSource.PlayOneShot(sunetButon);
        }

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