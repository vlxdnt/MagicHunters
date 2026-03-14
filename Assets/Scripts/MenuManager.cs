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

    [Header("Butoane Lobby")]
    public Button butonStart; // Butonul Start, vizibil doar pentru host

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
        try
        {
            // Cream o alocare Relay pentru maxim 1 client (2 jucatori total)
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            // Generam codul de join pe care clientul il va introduce
            string codJoin = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

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
            AfiseazaLobby();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Eroare Host Relay: " + e.Message);
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
        try
        {
            string cod = inputFieldCod.text.Trim();
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
            AfiseazaLobby();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Eroare Client Relay: " + e.Message);
        }
    }

    // Apelata de butonul Start (doar host)
    // Incarca scena de joc pentru toti jucatorii
    public void ButonStartJoc()
    {
        if (NetworkManager.Singleton.IsServer)
        {
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