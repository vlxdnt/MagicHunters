using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Netcode;

public class IntroDialog : NetworkBehaviour
{
    [Header("Bule")]
    public GameObject bulaWitch;
    public GameObject bulaCat;
    public TextMeshPro textWitch;
    public TextMeshPro textCat;

    [Header("Setari")]
    public float vitezaTypewriter = 0.05f;
    public float timpAfisare = 2.5f;

    //replici in ordine
    private string[] personaje = { "witch", "cat", "witch", "cat", "witch" };
    private string[] replici =
    {
        "I can't believe our sister turned out this way...",
        "Meow...",
        "We have to stay together",
        "Meow!",
        "Let's go!"
    };

    //apelat prin semnal
    public void PornesteDialog()
    {
        if (IsServer)
            StartCoroutine(RuleazaDialog());
    }

    IEnumerator RuleazaDialog()
    {
        for (int i = 0; i < replici.Length; i++){
            bool eWitch = personaje[i] == "witch";

            //bula coresp
            ArataBulaClientRpc(eWitch, replici[i]);

            //wait pt typewriter si timp afisare
            float timpTypewriter = replici[i].Length * vitezaTypewriter;
            yield return new WaitForSeconds(timpTypewriter + timpAfisare);

            //hide la bula
            AscundeBulaClientRpc(eWitch);
            yield return new WaitForSeconds(0.3f);
        }

        ActiveazaControlClientRpc();
    }

    [ClientRpc]
    void ArataBulaClientRpc(bool eWitch, string mesaj)
    {
        StartCoroutine(Typewriter(eWitch, mesaj));
    }

    [ClientRpc]
    void AscundeBulaClientRpc(bool eWitch) {
        if (eWitch)
        {
            bulaWitch.SetActive(false);
            textWitch.text = "";
        }
        else
        {
            bulaCat.SetActive(false);
            textCat.text = "";
        }
    }

    IEnumerator Typewriter(bool eWitch, string mesaj)
    {
        GameObject bula = eWitch ? bulaWitch : bulaCat;
        TextMeshPro text = eWitch ? textWitch : textCat;

        bula.SetActive(true);
        text.text = "";

        foreach(char c in mesaj)
        {
            text.text += c;
            yield return new WaitForSeconds(vitezaTypewriter);
        }
    }

    [ClientRpc]
    void ActiveazaControlClientRpc()
    {
        foreach (var player in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                Debug.Log($"{player.gameObject.name} constraints inainte: {rb.constraints}");
                rb.simulated = true;
                rb.linearVelocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                Debug.Log($"{player.gameObject.name} constraints dupa: {rb.constraints}");
            }

            if (player.IsOwner)
                player.controlActiv = true;
        }
    }

}
