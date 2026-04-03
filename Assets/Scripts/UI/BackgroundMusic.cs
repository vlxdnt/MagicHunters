using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance; //ca sa fie gasig

    [Header("Setari Audio")]
    [Range(0, 1)] public float volumMaximMuzica = 0.3f;
    public float vitezaFade = 1.5f; // sec pt fade

    [Header("Melodii")]
    public AudioClip melodieMenu;
    public AudioClip melodieGame;

    private AudioSource audioSource;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.volume = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clipNou = (scene.name == "MainMenu") ? melodieMenu : melodieGame;

        if (audioSource.clip != clipNou)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeSchimbareMelodie(clipNou));
        }
    }

    IEnumerator FadeSchimbareMelodie(AudioClip clipNou)
    {
        // fade out
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * vitezaFade;
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = clipNou;
        audioSource.Play();

        // fade in
        while (audioSource.volume < volumMaximMuzica)
        {
            audioSource.volume += Time.deltaTime * vitezaFade;
            yield return null;
        }

        // final
        audioSource.volume = volumMaximMuzica;
    }

    public void SeteazaMuteMuzica(bool activat)
    {
        if (audioSource != null)
        {
            //
            audioSource.mute = !activat;
        }
    }
}