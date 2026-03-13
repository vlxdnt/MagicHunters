using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Sockets;

public class MeniuManager : MonoBehaviour
{
    [Header("Panouri")]
    public GameObject panelMeniu;
    public GameObject panelLobby;

    [Header("Input & Info")]
    public TMP_InputField inputFieldIP;   // InputIP
    public Button inputButton;             // InputButton
    public TextMeshProUGUI textIPLocal;

    [Header("Butoane Lobby")]
    public Button butonStart; // Doar host il vede

    private void Start()
    {
        panelMeniu.SetActive(true);
        panelLobby.SetActive(false);
        inputFieldIP.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    // Butonul HOST
    public void ButonHost_Apasat()
    {
        NetworkManager.Singleton.StartHost();
        textIPLocal.text = "IP-ul tau: " + GetLocalIPAddress();
        AfiseazaLobby();
    }

    // Butonul JOIN - deschide panoul cu input field pentru IP
    public void ButonDeschideIntroducereIP()
    {
        inputFieldIP.gameObject.SetActive(true);
        inputButton.gameObject.SetActive(true);
    }

    // Butonul OK din panoul de IP
    public void ButonFinalizeazaConexiuneClient()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null && !string.IsNullOrEmpty(inputFieldIP.text))
        {
            transport.ConnectionData.Address = inputFieldIP.text;
        }
        NetworkManager.Singleton.StartClient();
        AfiseazaLobby();
    }

    // Butonul START (doar host)
    public void ButonStartJoc()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    void AfiseazaLobby()
    {
        panelMeniu.SetActive(false);
        panelLobby.SetActive(true);
        inputFieldIP.gameObject.SetActive(false);
        inputButton.gameObject.SetActive(false);

        if (butonStart != null)
        {
            butonStart.gameObject.SetActive(NetworkManager.Singleton.IsHost);
            butonStart.interactable = false; // blocat pana sunt 2 jucatori
        }
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        }
        return "127.0.0.1";
    }
}