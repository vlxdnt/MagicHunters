using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public BatEnemy[] inamiciDinCamera;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (BatEnemy inamic in inamiciDinCamera)
                if (inamic != null) inamic.JucatorIntrat();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (BatEnemy inamic in inamiciDinCamera)
                if (inamic != null) inamic.JucatorIesit();
        }
    }

}