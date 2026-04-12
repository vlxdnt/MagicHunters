using UnityEngine;
using Unity.Netcode;

public class TargetDoor : NetworkBehaviour
{
    [Header("Usa asociata")]
    public GameObject usa;
    public Sprite spriteUsaDeschisa;
    public Collider2D colliderUsa;

    [Header("Target")]
    public Sprite spriteTargetLovit;
    private SpriteRenderer srTarget;
    private bool esteLovit = false;

    void Awake()
    {
        srTarget = GetComponent<SpriteRenderer>();
    }

    // Aceasta functie va fi apelata din scripturile noastre de Atac si Fireball
    public void LovesteTarget()
    {
        if (esteLovit) return;

        // Atacurile noastre se inregistreaza pe Server. 
        // Serverul aproba lovitura si trimite semnalul catre toti jucatorii.
        if (!IsServer) return;

        DeschideUsaClientRpc();
    }

    [ClientRpc]
    private void DeschideUsaClientRpc()
    {
        esteLovit = true;

        // 1. Schimbam sprite-ul targetului (ex: din aprins in stins, sau rupt)
        if (srTarget != null && spriteTargetLovit != null)
            srTarget.sprite = spriteTargetLovit;

        // 2. Deschidem usa (exact ca la buton)
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