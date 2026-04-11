using UnityEngine;
using Unity.Netcode;

public class PlayerRespawn : NetworkBehaviour
{
    public Transform spawnInitial; //
    public Vector3 pozitieRespawn;
    public int prioritateCheckpoint = -1;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            pozitieRespawn = transform.position; //
    }

    public void SetCheckpoint(Vector3 pozitie, int prioritate)
    {
        pozitieRespawn = pozitie;
        prioritateCheckpoint = prioritate;
    }

    public void Respawn()
    {
        if (IsOwner)
            transform.position = pozitieRespawn;
    }
}