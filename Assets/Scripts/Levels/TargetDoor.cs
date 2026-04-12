using UnityEngine;

public class TargetDoor : MonoBehaviour
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
        DeschideUsa();

        TargetHitConnector connector = FindFirstObjectByType<TargetHitConnector>();
        if (connector != null) connector.LovesteTargetServerRpc(gameObject.name);
    }

    public void DeschideUsa()
    {
        if (esteLovit) return;
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