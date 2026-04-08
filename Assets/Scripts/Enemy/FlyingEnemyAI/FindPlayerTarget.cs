using UnityEngine;
using Pathfinding;

public class FindPlayerTarget : MonoBehaviour
{
    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;

    [Header("Aggro Settings")]
    public float aggroRadius = 5f;
    public LayerMask playerLayer; // Set this to your "Player" layer

    void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();

        // Start disabled so the bat doesn't move immediately
        if (aiPath != null) aiPath.canMove = false;
    }

    void Update()
    {
        FindClosestPlayer();
    }

    private void FindClosestPlayer()
    {
        // Find all objects on the Player layer within radius
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, aggroRadius, playerLayer);

        if (playersInRange.Length > 0)
        {
            float closestDistance = Mathf.Infinity;
            Transform targetPlayer = null;

            foreach (Collider2D p in playersInRange)
            {
                float dist = Vector2.Distance(transform.position, p.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    targetPlayer = p.transform;
                }
            }

            // Target found: set destination and enable movement
            if (targetPlayer != null)
            {
                destinationSetter.target = targetPlayer;
                aiPath.canMove = true;
            }
        }
        else
        {
            // No players in range: stop moving
            destinationSetter.target = null;
            aiPath.canMove = false;
        }
    }
}