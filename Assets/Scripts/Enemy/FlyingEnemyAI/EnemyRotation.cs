using UnityEngine;
using Pathfinding;

public class EnemyRotation : MonoBehaviour
{
    public AIPath aiPath;

    void Start()
    {
        if (aiPath == null)
        {
            aiPath = GetComponentInParent<AIPath>();
        }
    }

    void Update()
    {
        if (aiPath == null) return;

        float scaleX = Mathf.Abs(transform.localScale.x); 

        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }
    }
}