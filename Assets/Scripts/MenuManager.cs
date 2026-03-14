using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class MeniuManager : MonoBehaviour
{
    [Header("Panouri")]
    public GameObject panelMeniu;
    public GameObject panelLobby;

    [Header("Input & Info")]
    public TMP_InputField inputFieldCod;  // Clientul introduce codul aici
    public Button inputButton;             // Butonul OK
    public TextMeshProUGUI textCodJoin;   // Afiseaza codul pentru host

    [Header("Butoane Lobby")]
    public Button butonStart;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        panelMeniu.SetActive(true);
        panelLobby.SetActive(false);
        inputFieldCod.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);

        // Initializam Unity Services
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Butonul HOST
    public async void ButonHost_Apasat()
    {
        try
        {
            // Cream un relay pentru 2 jucatori (1 host + 1 client)
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string codJoin = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Configuram transport
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            // Afisam codul
            textCodJoin.text = "Cod Join: " + codJoin;

            AfiseazaLobby();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Eroare Host Relay: " + e.Message);
        }
    }

    // Butonul JOIN - arata input field pentru cod
    public void ButonDeschideIntroducereIP()
    {
        inputFieldCod.gameObject.SetActive(true);
        inputButton.gameObject.SetActive(true);
    }

    // Butonul OK - clientul se conecteaza cu codul
    public async void ButonFinalizeazaConexiuneClient()
    {
        try
        {
            string cod = inputFieldCod.text.Trim();
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(cod);

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

            AfiseazaLobby();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Eroare Client Relay: " + e.Message);
        }
    }

    // Butonul START (doar host)
    public void ButonStartJoc()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    void AfiseazaLobby()
    {
        panelMeniu.SetActive(false);
        inputFieldCod.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);
        panelLobby.SetActive(true);

        if (butonStart != null)
        {
            butonStart.gameObject.SetActive(NetworkManager.Singleton.IsHost);
            butonStart.interactable = false;
        }
    }
}