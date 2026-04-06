using UnityEngine;
using Pathfinding; // FOARTE IMPORTANT: Trebuie să incluzi asta pentru a accesa AIDestinationSetter

public class FindPlayerTarget : MonoBehaviour
{
    private AIDestinationSetter destinationSetter;

    void Start()
    {
        // Preluăm componenta AIDestinationSetter de pe inamic
        destinationSetter = GetComponent<AIDestinationSetter>();

        // Căutăm player-ul abia când inamicul se spawnează / începe scena
        FindAndSetTarget();
    }

    void Update()
    {
        // Dacă din greșeală target-ul se pierde (ex: player-ul moare și e distrus),
        // inamicul va încerca să îl caute din nou în fiecare cadru până îl găsește.
        if (destinationSetter.target == null)
        {
            FindAndSetTarget();
        }
    }

    private void FindAndSetTarget()
    {
        // Caută obiectul cu tag-ul "Player" în scena activă
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Dacă l-a găsit, îi atribuie Transform-ul în AI Destination Setter
            destinationSetter.target = player.transform;
        }
    }
}