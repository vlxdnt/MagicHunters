using UnityEngine;
using TMPro;

// de pus pe fiecare panel, pe viitor de implementat pt GameScene
public class LocalizedPanel : MonoBehaviour
{
    void OnEnable()
    {
        Language.OnLimbaSchimbata += Actualizeaza;
        Actualizeaza();
    }

    void OnDisable()
    {
        Language.OnLimbaSchimbata -= Actualizeaza;
    }

    void Actualizeaza()
    {
        if (Language.Instance == null) return;

        //actualizare
        foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>())
        {
            //pt parintele textului
            string cheieParinte = tmp.transform.parent != null ? tmp.transform.parent.name : "";
            //pt text in sine
            string cheieObiect = tmp.gameObject.name;

            string textLocalizat = Language.Instance.Get(cheieParinte);

            // dupa obiect in sine daca nu gaseste parinte
            if (textLocalizat == cheieParinte)
                textLocalizat = Language.Instance.Get(cheieObiect);

            // actualizare din json
            if (textLocalizat != cheieParinte && textLocalizat != cheieObiect)
                tmp.text = textLocalizat;
        }
    }
}