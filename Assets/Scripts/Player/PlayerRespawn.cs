using UnityEngine;
using Unity.Netcode;

public class PlayerRespawn : NetworkBehaviour
{
    public Transform spawnInitial; //
    public Vector2 pozitieRespawn;
    public int prioritateCheckpoint = -1;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            pozitieRespawn = transform.position; //
    }

    public void SetCheckpoint(Vector2 pozitie, int prioritate)
    {
        pozitieRespawn = pozitie;
        prioritateCheckpoint = prioritate;
    }

    public void Respawn()
    {
        if (IsOwner)
            transform.position = new Vector3(pozitieRespawn.x, pozitieRespawn.y, 0f);
    }
}