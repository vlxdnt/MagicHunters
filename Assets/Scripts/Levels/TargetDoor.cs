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

    public void LovesteTarget()
    {
        if (esteLovit) return;

        if (!IsServer) return;

        DeschideUsaClientRpc();
    }

    [ClientRpc]
    private void DeschideUsaClientRpc()
    {
        esteLovit = true;

        if (srTarget != null && spriteTargetLovit != null)
            srTarget.sprite = spriteTargetLovit;

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