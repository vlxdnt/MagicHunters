using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.InputSystem;

public partial class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;  
    public GameObject pauseMenuUI;
    public GameObject settingsPanel; 
    private bool isPaused = false;

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
        pauseMenuCanvas.SetActive(true);  // pt panel in sine
        pauseMenuUI.SetActive(true);       // optiuni
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Revino()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false); // dezactivează tot
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
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