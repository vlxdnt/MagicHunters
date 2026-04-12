using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    public AudioClip sunetButon;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        // preincarcare
        if (sunetButon != null)
        {
            audioSource.clip = sunetButon;
            audioSource.volume = 0f;
            audioSource.Play();
            audioSource.Stop();
            audioSource.volume = 1f;
        }
    }

    void Start()
    {
        // cauta fiecare buton
        Button[] butoane = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button btn in butoane)
        {
            btn.onClick.AddListener(() => PlaySound());
        }
    }

    void PlaySound()
    {
        if (sunetButon != null && audioSource != null)
            audioSource.PlayOneShot(sunetButon);
    }
}