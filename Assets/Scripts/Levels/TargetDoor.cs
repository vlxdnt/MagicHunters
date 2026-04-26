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

    private readonly NetworkVariable<bool> esteLovit = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Awake()
    {
        srTarget = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        esteLovit.OnValueChanged += OnTargetStateChanged;

        if (esteLovit.Value)
        {
            AplicaSchimbariVizuale(true);
        }
    }

    public override void OnNetworkDespawn()
    {
        esteLovit.OnValueChanged -= OnTargetStateChanged;
    }

    private void OnTargetStateChanged(bool previousValue, bool newValue)
    {
        AplicaSchimbariVizuale(newValue);
    }

    public void LovesteTarget()
    {
        if (esteLovit.Value) return;

        if (IsServer)
        {
            esteLovit.Value = true;
        }
        else
        {
            LovesteTargetServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LovesteTargetServerRpc()
    {
        esteLovit.Value = true;
    }

    private void AplicaSchimbariVizuale(bool lovit)
    {
        if (!lovit) return;

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