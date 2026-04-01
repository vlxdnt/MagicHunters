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

    [Header("Sunete Dialog")]
    public AudioSource sursaAudioDialog;
    public AudioClip sunetTypewriter; // click pt litere
    public AudioClip[] sunetePisica;

    //replici in ordine
    private string[] personaje = { "witch", "cat", "witch", "cat", "witch" };
    private string[] replici =
    {
        "I can't believe my sister turned out this way...",
        "Meow...",
        "We have to stay together!",
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
        if (!eWitch && sunetePisica.Length > 0 && sursaAudioDialog != null)
        {
            int index = Random.Range(0, sunetePisica.Length);
            sursaAudioDialog.PlayOneShot(sunetePisica[index]);
        }
        // pornesc textu
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

        text.enableAutoSizing = false;

        if (eWitch && sunetTypewriter != null && sursaAudioDialog != null)
        {
            sursaAudioDialog.clip = sunetTypewriter;
            sursaAudioDialog.loop = true; // in loop
            sursaAudioDialog.Play();
        }

        //marime font
        if (mesaj.Length > 25) text.fontSize = 0.6f;
        else if (mesaj.Length > 15) text.fontSize = 0.75f;
        else text.fontSize = 0.9f;

        foreach (char c in mesaj)
        {
            text.text += c;

            // latimea pt fiecare litera
            float latimeText = text.preferredWidth;

            // creste pe X
            float nouaScalaX = Mathf.Clamp(latimeText + 0.4f, 1.0f, 1.8f);

            // creste Y raportat la X
            float nouaScalaY = 1f + (nouaScalaX - 1.0f) * 0.5f;
            nouaScalaY = Mathf.Clamp(nouaScalaY, 1f, 1.4f);

            bula.transform.localScale = new Vector3(nouaScalaX, nouaScalaY, 1f);

            yield return new WaitForSeconds(vitezaTypewriter);
        }
        // reset la pitch daca alegem sa-l punem
        //if (sursaAudioDialog != null) sursaAudioDialog.pitch = 1f;
        if (eWitch && sursaAudioDialog != null)
        {
            sursaAudioDialog.Stop();
        }
    }

    [ClientRpc]
    void ActiveazaControlClientRpc()
    {
        //pt fiecare player
        foreach (var player in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            //reset scale pt timeline(de siguranta)
            Vector3 currentScale = player.transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x);
            player.transform.localScale = currentScale;

            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                rb.simulated = true;
                rb.linearVelocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            if (player.IsOwner)
            {
                player.controlActiv = true;
            }
        }

        //Debug.Log("Control activat pt toti");
    }

}
