using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections;

public partial class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;  
    public GameObject pauseMenuUI;
    public GameObject settingsPanel; 
    private bool isPaused = false;

    private Vector2[] vitezeSalvate;
    private Rigidbody2D[] rbJucatori;
    private float[] gravitySalvata;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Revino();
            else Pauza();
        }
    }

    public void Pauza()
    {
        isPaused = true;
        pauseMenuCanvas.SetActive(true);
        pauseMenuUI.SetActive(true);

        PlayerInput[] players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (!player.IsOwner) continue; // doar owner-ul

            player.miscareBlocata = true;
            var input = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.DeactivateInput();

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Revino()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        PlayerInput[] players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (!player.IsOwner) continue; // doar owner-ul

            player.miscareBlocata = false;
            var input = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.ActivateInput();
        }

        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DeschideSetari()
    {
        pauseMenuUI.SetActive(false);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            // incarcare
            SettingsManager sm = GetComponent<SettingsManager>();
            if (sm != null) sm.IncarcaSetari();
        }
    }

    public void IesiDinJoc()
    {
        Time.timeScale = 1f;
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void InchideSetari()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        pauseMenuUI.SetActive(true); // revenie
    }
}