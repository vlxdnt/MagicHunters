using UnityEngine;
using Unity.Netcode;
using System.Collections; // Necesari pt Coroutine

public class PlayerRespawn : NetworkBehaviour
{
    public Transform spawnInitial; 
    public Vector2 pozitieRespawn;
    public int prioritateCheckpoint = -1;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        GameObject spawnPoint = GameObject.Find("SpawnInitial");
        if (spawnPoint != null)
            pozitieRespawn = spawnPoint.transform.position;
        else
            pozitieRespawn = transform.position;
    }

    public void SetCheckpoint(Vector2 pozitie, int prioritate)
    {
        pozitieRespawn = pozitie;
        prioritateCheckpoint = prioritate;
    }

    public void Respawn()
    {
        if (IsOwner)
        {
            // 1. Mutăm jucătorul local
            transform.position = new Vector3(pozitieRespawn.x, pozitieRespawn.y, 0f);

            // 2. Anunțăm serverul Imediat să reseteze inamicii
            SincronizeazaRespawnServerRpc(pozitieRespawn);
        }
    }

    [ServerRpc]
    void SincronizeazaRespawnServerRpc(Vector2 nouaPozitie)
    {
        // Forțăm mutarea și pe server în același frame
        transform.position = new Vector3(nouaPozitie.x, nouaPozitie.y, 0f);

        // Declanșăm pierderea targetului pentru inamici
        StartCoroutine(DropAggroTemporar());
    }

    private IEnumerator DropAggroTemporar()
    {
        WitchAbilities witch = GetComponent<WitchAbilities>();
        if (witch != null)
        {
            // Salvăm starea inițială (în caz că avea deja o vrajă de invizibilitate activă)
            bool eraInvizibil = witch.esteInvizibil.Value;
            
            // Îl facem invizibil pentru inamici ca să îi dea drop din target
            witch.esteInvizibil.Value = true;

            // Așteptăm suficient cât scripturile AI să ruleze un ciclu de căutare (0.5s la liliac)
            yield return new WaitForSeconds(0.6f); 

            // Îi redăm starea inițială
            if (!eraInvizibil) 
            {
                witch.esteInvizibil.Value = false;
            }
        }
    }
}