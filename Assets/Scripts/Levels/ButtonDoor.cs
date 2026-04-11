using UnityEngine;
using Unity.Netcode; // Adaugam libraria pentru retea

// Schimbam din MonoBehaviour in NetworkBehaviour
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

    void Awake()
    {
        srButon = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (esteApasat) return;

        // Verificam daca cel care a atins butonul este un jucator
        if (other.CompareTag("Player"))
        {
            NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
            
            // Pentru a nu trimite aceeasi comanda de 2 ori (si de pe ecranul tau, si de pe al prietenului),
            // ii dam voie doar jucatorului LOCAL (cel care "detine" caracterul) sa anunte serverul
            if (playerNetObj != null && playerNetObj.IsOwner)
            {
                ApasaButonServerRpc();
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ApasaButonServerRpc()
    {
        // Serverul primeste semnalul si le spune TUTUROR jucatorilor sa deschida usa
        DeschideUsaClientRpc();
    }

    [ClientRpc]
    private void DeschideUsaClientRpc()
    {
        // Ne asiguram ca nu o deschidem de doua ori
        if (esteApasat) return;
        esteApasat = true;

        // 1. Schimbam sprite-ul butonului
        if (srButon != null && spriteButonApasat != null)
            srButon.sprite = spriteButonApasat;

        // 2. Schimbam usa si oprim coliziunea ei
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