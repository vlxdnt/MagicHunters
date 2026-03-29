using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;

public class WitchVision : NetworkBehaviour
{
    [Header("Viziunea Vrajitoarei")]
    public Light2D luminaPersonala;

    public override void OnNetworkSpawn()
    {
        if (luminaPersonala != null)
        {
            LevelSettings setariNivel = FindFirstObjectByType<LevelSettings>();
            bool eIntuneric = (setariNivel != null && setariNivel.esteNivelIntunecat);

            luminaPersonala.enabled = (eIntuneric && IsOwner); 
        }
    }
}