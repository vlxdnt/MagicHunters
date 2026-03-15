using Unity.Netcode; 
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;
using Unity.Services.Lobbies.Models;

public class MeniuManager : MonoBehaviour
{
    public Button butonHost;
    public Button butonClient;

    private void Start()
    {
        butonHost.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            AscundeMeniul();
        });

        butonClient.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            if (textCodJoin != null)
                textCodJoin.text = "Conectat la cod: " + cod;
            textEroare.gameObject.SetActive(false);

            AfiseazaLobby();
        }
        catch (Unity.Services.Relay.RelayServiceException e)
        {
            Debug.LogWarning("Eroare Relay: " + e.Message);
            
            if (textEroare != null)
            {
                // Verificam daca motivul este ca acel cod nu a fost gasit
                if (e.Reason == Unity.Services.Relay.RelayExceptionReason.JoinCodeNotFound)
                {
                    textEroare.text = "Acest cod nu exista!";
                }
                // Verificam mesajul pentru a vedea daca lobby-ul si-a atins limita maxima de jucatori
                else if (e.Message.ToLower().Contains("capacity") || e.Message.ToLower().Contains("full"))
                {
                    textEroare.text = "Acest lobby este plin!";
                }
                // Orice alta eroare de la serverele Unity (ex: lipsa internet)
                else
                {
                    textEroare.text = "Eroare de conexiune la server!";
                }
                textEroare.gameObject.SetActive(true);
            }
            ResetLobby();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Eroare Client Relay: " + e.Message);
            if (textEroare != null)
            {
                textEroare.text = "A aparut o eroare necunoscuta!";
                textEroare.gameObject.SetActive(true);
            }
            ResetLobby();
        }
    }

    // Apelata de butonul Start (doar host)
    // Incarca scena de joc pentru toti jucatorii
    public void ButonStartJoc()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            LobbySelection lobby = FindFirstObjectByType<LobbySelection>();
            if (lobby != null)
            {
                LobbySelection.alegereFinalaHost = lobby.hostSelection.Value;
                LobbySelection.alegereFinalaClient = lobby.clientSelection.Value;
            }
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    // Afiseaza panoul lobby si configureaza butonul Start
    void AfiseazaLobby()
    {
        panelMeniu.SetActive(false);
        inputFieldCod.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);
        panelLobby.SetActive(true);

        if (butonStart != null)
        {
            // Butonul Start e vizibil doar pentru host
            butonStart.gameObject.SetActive(NetworkManager.Singleton.IsHost);
            // Blocat initial pana ce ambii jucatori aleg personajul
            butonStart.interactable = false;
        }
    }

    // Reseteaza UI-ul la starea initiala (folosit la iesirea din lobby)
    public void ResetLobby()
    {
        panelMeniu.SetActive(true);
        panelLobby.SetActive(false);
        inputFieldCod.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);
        inputFieldCod.text = "";

        if (panelLoading != null)
            panelLoading.SetActive(false);

        if (butonStart != null)
        {
            butonStart.gameObject.SetActive(false);
            butonStart.interactable = false;
        }

        // Stergem codul de join afisat
        if (textCodJoin != null)
            textCodJoin.text = "";
    }
}