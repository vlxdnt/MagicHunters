using UnityEngine;
using Unity.Netcode;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class IntroTimelineManager : NetworkBehaviour
{
    public PlayableDirector director;
    private bool timelineStarted = false;

    public GameObject gameUI;

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

        foreach (var output in director.playableAsset.outputs)
            director.ClearGenericBinding(output.sourceObject);

        director.RebuildGraph();

        PlayerInput[] allPlayers = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var player in allPlayers)
        {
            player.transform.rotation = Quaternion.identity;

            Vector3 scale = player.transform.localScale;
            scale.x = Mathf.Abs(scale.x); // mereu pozitiv
            player.transform.localScale = scale;

            if (player.TryGetComponent(out Unity.Netcode.Components.NetworkTransform nt))
            {
                nt.enabled = true;
                if (player.IsOwner)
                    nt.Teleport(player.transform.position, player.transform.rotation, Vector3.one);
            }

            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                rb.simulated = true;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        director.enabled = false;

        Debug.Log("ActiveazaControl apelat pe: " + (IsServer ? "Server" : "Client"));
        Debug.Log("gameUI este: " + (gameUI != null ? gameUI.name : "NULL"));

        if (gameUI != null)
        {
            gameUI.SetActive(true);
            Debug.Log("GameUI activat!");

            PlayerUI ui = gameUI.GetComponentInChildren<PlayerUI>(true); // true = cauta si in dezactivate
            Debug.Log("PlayerUI gasit: " + (ui != null));

            if (ui != null && ObjectiveManager.Instance != null)
                ui.SetObiectiv(ObjectiveManager.Instance.GetObiectivCurent());
        }
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

            // activare control ca sa nu cada prin mapa
            SetPlayerState(witchObj, true); 
            SetPlayerState(catObj, true);

            // legare tracks in timeline
            foreach (var output in director.playableAsset.outputs)
            {
                var binding = director.GetGenericBinding(output.sourceObject);
                Debug.Log($"Track: '{output.streamName}' → Binding: {binding}");

                if (output.streamName == "WitchRun" || output.streamName == "WitchAnim")
                    director.SetGenericBinding(output.sourceObject, witchObj.GetComponent<Animator>());

                if (output.streamName == "CatRun" || output.streamName == "CatAnim")
                    director.SetGenericBinding(output.sourceObject, catObj.GetComponent<Animator>());
            }

            director.Play();

            if (SceneFade.Instance != null)
                StartCoroutine(SceneFade.Instance.FadeIn(2f)); //time pentru incarcare

            if (IsServer) timelineStarted = true;
        }
    }

    //
    void SetPlayerState(GameObject obj, bool blocat)
    {
        if (obj.TryGetComponent(out Rigidbody2D rb))
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.constraints = blocat
                ? RigidbodyConstraints2D.FreezeAll
                : RigidbodyConstraints2D.FreezeRotation;
        }
    }
}