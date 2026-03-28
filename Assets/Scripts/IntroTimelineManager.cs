using UnityEngine;
using Unity.Netcode;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class IntroTimelineManager : NetworkBehaviour
{
    public PlayableDirector director;
    private bool timelineStarted = false;

    void Update()
    {
        if (IsServer && timelineStarted)
        {
            // daca s-a term timeline
            if (director.time >= director.duration)
            {
                timelineStarted = false;
                ActiveazaControlClientRpc();
                this.enabled = false; // dezactivare
            }
        }
    }

    [ClientRpc]
    void ActiveazaControlClientRpc()
    {
        if (director == null) return;

        director.Stop();

        // eliberare obiecte
        foreach (var output in director.playableAsset.outputs)
        {
            director.ClearGenericBinding(output.sourceObject);
        }

        director.RebuildGraph();

        // pt control, sa nu ramana pe hold in timeline
        PlayerInput[] allPlayers = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var player in allPlayers)
        {
            // la pozitii
            player.transform.localScale = Vector3.one;
            player.transform.rotation = Quaternion.identity;

            if (player.TryGetComponent(out Unity.Netcode.Components.NetworkTransform nt))
            {
                nt.enabled = true;

                //teleportare - posibil glitch la terminarea timeline-ului
                if (player.IsOwner)
                {
                    nt.Teleport(player.transform.position, player.transform.rotation, Vector3.one);
                }
            }

            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                rb.simulated = true;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
            }

            if (player.IsOwner)
            {
                player.enabled = true;
            }
        }

        director.enabled = false;
    }

    public void LeagaPersonaje(GameObject witch, GameObject cat)
    {
        NetworkObject witchNet = witch.GetComponent<NetworkObject>();
        NetworkObject catNet = cat.GetComponent<NetworkObject>();
        StartTimelineClientRpc(witchNet, catNet);
    }

    [ClientRpc]
    void StartTimelineClientRpc(NetworkObjectReference witchRef, NetworkObjectReference catRef)
    {
        if (witchRef.TryGet(out NetworkObject witchNet) && catRef.TryGet(out NetworkObject catNet))
        {
            GameObject witchObj = witchNet.gameObject;
            GameObject catObj = catNet.gameObject;

            // dezactivare control ca sa nu cada prin mapa
            SetPlayerState(witchObj, false);
            SetPlayerState(catObj, false);

            // legare tracks in timeline
            foreach (var output in director.playableAsset.outputs)
            {
                if (output.streamName == "WitchRun" || output.streamName == "WitchAnim")
                    director.SetGenericBinding(output.sourceObject, witchObj.GetComponent<Animator>());

                if (output.streamName == "CatRun" || output.streamName == "CatAnim")
                    director.SetGenericBinding(output.sourceObject, catObj.GetComponent<Animator>());
            }

            director.Play();
            if (IsServer) timelineStarted = true;
        }
    }

    //
    void SetPlayerState(GameObject obj, bool state)
    {
        if (obj.TryGetComponent(out PlayerInput pi)) pi.enabled = state;
        if (obj.TryGetComponent(out Rigidbody2D rb)) rb.simulated = state;
        if (obj.TryGetComponent(out Unity.Netcode.Components.NetworkTransform nt)) nt.enabled = state;
    }
}