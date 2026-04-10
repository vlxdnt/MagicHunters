using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.AvaseazaObiectiv();

        // dezactiveaza trigger ul
        gameObject.SetActive(false);
    }
}