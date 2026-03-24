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
using System.Collections;
using System;

// creierul, gestionam meniul si tranzitia catre joc
public class MeniuManager : MonoBehaviour
{
    //panou principal + lobby
    [Header("Panouri")]
    public GameObject panelMeniu; 
    public GameObject panelLobby; 

    // things
    [Header("Input & Info")]
    public TMP_InputField inputFieldCod; // cod join
    public Button inputButton;           // ok button
    public TextMeshProUGUI textCodJoin;  //codu de join
    private string codJoinCurent = ""; // stocare
    public TextMeshProUGUI textEroare;
    public static string eroareIntreScene = "";

    // start button pt host
    [Header("Butoane Lobby")]
    public Button butonStart; 

    // loading screen
    [Header("Loading")]
    public GameObject panelLoading;

    public static MeniuManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private async void Start()
    {
        if (Instance != this) return; 

        // starea initiala
        panelMeniu.SetActive(true);
        panelLobby.SetActive(false);
        inputFieldCod.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);

        if (butonStart != null)
            butonStart.gameObject.SetActive(false);

        // Afisare eroare dupa deconectare
        if (!string.IsNullOrEmpty(eroareIntreScene))
        {
            AfiseazaEroare(eroareIntreScene);
            eroareIntreScene = ""; 
        }

        // verificam daca serviciile sunt deja pornite inainte sa le initiem
        if (UnityServices.State == Unity.Services.Core.ServicesInitializationState.Uninitialized)
        {
            await UnityServices.InitializeAsync();
        }

        // autentificare pt unity relay
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    // face o sesiunea de relay ca si host
    public async void ButonHost_Apasat()
    {
        // schimb de meniu
        panelMeniu.SetActive(false);
        panelLoading.SetActive(true);
        if (textEroare != null)
            textEroare.gameObject.SetActive(false);

        try
        {
            // alocare pt un singur jucator pana la 2
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            // generare cod
            string codJoin = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            codJoinCurent = codJoin; // salvare

            // transportul de date al serverului
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            // codul afisat
            textCodJoin.text = "Cod Join: " + codJoin;

            panelLoading.SetActive(false); // schimb meniu
            AfiseazaLobby();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Eroare Host Relay: " + e.Message);
            //intoarcere la meniu
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

    // apasare join
    public void ButonDeschideIntroducereIP()
    {
        inputFieldCod.gameObject.SetActive(true);
        inputButton.gameObject.SetActive(true);
    }

    // apasare ok
    public async void ButonFinalizeazaConexiuneClient()
    {
        string cod = inputFieldCod.text.Trim();
        //caz de cod invalid
        if (string.IsNullOrEmpty(cod))
        {
            textEroare.gameObject.SetActive(true);
            textEroare.text = "Te rog introdu un cod valid.";
            return;
        }

        try
        {
            // incearca conectarea la relay prin cod
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(cod);

            // datele de relay ale hostului
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
                // cazuri
                if (e.Reason == Unity.Services.Relay.RelayExceptionReason.JoinCodeNotFound)
                {
                    textEroare.text = "Acest cod nu exista!";
                }
                // max players
                else if (e.Message.ToLower().Contains("capacity") || e.Message.ToLower().Contains("full"))
                {
                    textEroare.text = "Acest lobby este plin!";
                }
                // lipsa internet
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

    // apasare Start si incarca pt ambii jucatori
    public void ButonStartJoc()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            LobbySelection lobby = FindFirstObjectByType<LobbySelection>();
            if (lobby != null)
            {
                LobbySelection.finalHostSelection = lobby.hostSelection.Value;
                LobbySelection.finalClientSelection = lobby.clientSelection.Value;
            }
            StartCoroutine(StartCuFade());
        }
    }

    IEnumerator StartCuFade()
    {
        // fade out inainte de a schimba scena
        yield return StartCoroutine(SceneFade.Instance.FadeOut(0.5f));
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    //panou lobby
    void AfiseazaLobby()
    {
        panelMeniu.SetActive(false);
        inputFieldCod.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);
        panelLobby.SetActive(true);

        if (butonStart != null)
        {
            // vizibilitate buton doar pt host
            butonStart.gameObject.SetActive(NetworkManager.Singleton.IsHost);
            // blocare pt a alege caracterul
            butonStart.interactable = false;
        }
    }

    //fct pt eroare
    public void AfiseazaEroare(string mesaj)
    {
        if (textEroare != null)
        {
            textEroare.text = mesaj;
            textEroare.gameObject.SetActive(true);
        }
    }

    //iesire din lobby, resetare UI
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

        // stergere cod
        if (textCodJoin != null)
            textCodJoin.text = "";
    }

    public void IesiDinJoc()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        // curatam instanta cand scena se distruge
        if (Instance == this)
        {
            Instance = null;
        }
    }
}