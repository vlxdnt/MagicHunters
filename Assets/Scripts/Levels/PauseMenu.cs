using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.InputSystem;

public partial class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;    // Panel-ul principal (PauseMenu din imaginea ta)
    public GameObject settingsPanel;  // Trage aici panelul de setari din SettingsManager
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
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Revino()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false); // Închidem și setările dacă erau deschise

        Time.timeScale = 1f;

        // Blocăm mouse-ul înapoi în joc
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DeschideSetari()
    {
        // Dezactivăm butoanele de pauză și activăm setările
        // Poți folosi direct referințele din SettingsManager-ul tău
        pauseMenuUI.transform.Find("PanelOptiuni").gameObject.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void IesiDinJoc()
    {
        Time.timeScale = 1f;
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        SceneManager.LoadScene("MainMenu");
    }
}