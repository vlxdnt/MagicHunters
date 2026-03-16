using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;

// Gestioneaza meniul principal: Host, Join, Lobby si tranzitia catre joc
public class MeniuManager : MonoBehaviour
{
    [Header("Panouri")]
    public GameObject panelMeniu;  // Panoul principal cu butoanele Host/Join
    public GameObject panelLobby;  // Panoul lobby-ului dupa conectare

    [Header("Input & Info")]
    public TMP_InputField inputFieldCod; // Input field pentru codul de join
    public Button inputButton;           // Butonul OK pentru a confirma codul
    public TextMeshProUGUI textCodJoin;  // Textul care afiseaza codul generat de host
    private string codJoinCurent = ""; // Variabila pentru a stoca codul curent 
    public TextMeshProUGUI textEroare;

    [Header("Butoane Lobby")]
    public Button butonStart; // Butonul Start, vizibil doar pentru host

    [Header("Loading")]
    public GameObject panelLoading; //pentru loading screen la apasarea "Host"

    private async void Start()
    {
        // Facem obiectul persistent intre scene
        DontDestroyOnLoad(gameObject);

        // Starea initiala a UI-ului
        panelMeniu.SetActive(true);
        panelLobby.SetActive(false);
        inputFieldCod.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);

        if (butonStart != null)
            butonStart.gameObject.SetActive(false);

        // Initializam Unity Services (necesar pentru Relay)
        await UnityServices.InitializeAsync();

        // Autentificare anonima pentru a putea folosi Relay
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Apelata cand jucatorul apasa Host
    // Creeaza o sesiune Relay si porneste ca host
    public async void ButonHost_Apasat()
    {
        // Ascundem meniul si aratam loading
        panelMeniu.SetActive(false);
        panelLoading.SetActive(true);
        try
        {
            // Cream o alocare Relay pentru maxim 1 client (2 jucatori total)
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            // Generam codul de join pe care clientul il va introduce
            string codJoin = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            codJoinCurent = codJoin; // Salvam codul curent 

            // Configuram transportul cu datele Relay
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            // Afisam codul pe ecran ca hostul sa il poata da clientului
            textCodJoin.text = "Cod Join: " + codJoin;

            panelLoading.SetActive(false); // Ascundem loading si mergem la lobby
            AfiseazaLobby();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Eroare Host Relay: " + e.Message);
            // Daca e eroare, ne intoarcem la meniu
            panelLoading.SetActive(false);
            panelMeniu.SetActive(true);
        }
    }

    public void ButonCopiazaCod()
    {
        if (!string.IsNullOrEmpty(codJoinCurent))
        {
            GUIUtility.systemCopyBuffer = codJoinCurent;
            Debug.Log("Codul de join a fost copiat in clipboard: " + codJoinCurent);
        }
    }

    // Apelata cand jucatorul apasa Join
    // Arata input field-ul pentru introducerea codului
    public void ButonDeschideIntroducereIP()
    {
        inputFieldCod.gameObject.SetActive(true);
        inputButton.gameObject.SetActive(true);
    }

    // Apelata cand clientul apasa OK dupa introducerea codului
    // Se conecteaza la sesiunea Relay a hostului
    public async void ButonFinalizeazaConexiuneClient()
    {
        string cod = inputFieldCod.text.Trim();
        if (string.IsNullOrEmpty(cod))
        {
            textEroare.gameObject.SetActive(true);
            textEroare.text = "Te rog introdu un cod valid.";
            return;
        }

        try
        {
            // Ne alaturem sesiunii Relay cu codul primit de la host
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(cod);

            // Configuram transportul cu datele Relay ale hostului
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

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
            if (lobby != null)            {
                LobbySelection.finalHostSelection = lobby.hostSelection.Value;
                LobbySelection.finalClientSelection = lobby.clientSelection.Value;
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