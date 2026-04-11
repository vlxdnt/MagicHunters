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
        rbJucatori = new Rigidbody2D[players.Length];
        vitezeSalvate = new Vector2[players.Length];
        gravitySalvata = new float[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            rbJucatori[i] = players[i].GetComponent<Rigidbody2D>();
            if (rbJucatori[i] != null)
            {
                vitezeSalvate[i] = rbJucatori[i].linearVelocity;
                gravitySalvata[i] = rbJucatori[i].gravityScale;
                rbJucatori[i].linearVelocity = Vector2.zero;
                rbJucatori[i].gravityScale = 0f;
            }
        }

        Time.timeScale = 0f;
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

        if (rbJucatori != null)
        {
            for (int i = 0; i < rbJucatori.Length; i++)
            {
                if (rbJucatori[i] != null)
                {
                    rbJucatori[i].gravityScale = gravitySalvata[i];
                    rbJucatori[i].linearVelocity = vitezeSalvate[i];
                }
            }
        }

        Time.timeScale = 1f;
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