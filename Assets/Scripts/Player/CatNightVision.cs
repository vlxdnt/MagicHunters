using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CatNightVision : NetworkBehaviour
{
    [Header("Viziunea Pisicii")]
    public Light2D luminaOchi;

    public override void OnNetworkSpawn()
    {
        if (luminaOchi != null)
        {
            LevelSettings setariNivel = FindFirstObjectByType<LevelSettings>();
            
            bool eIntuneric = (setariNivel != null && setariNivel.esteNivelIntunecat);

            // se activeaza doar daca e level intunecat si doar pentru proprietarul acestui obiect
            luminaOchi.enabled = eIntuneric && IsOwner;
        }
    }
}
