using Unity.Netcode; 
using UnityEngine;
using UnityEngine.UI; 

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
            AscundeMeniul();
        });
    }

    void AscundeMeniul()
    {
        gameObject.SetActive(false);
    }
}