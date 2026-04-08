using UnityEngine;
using Pathfinding;

public class AutoScanAStar : MonoBehaviour
{
    void Start()
    {
        // Scans the level as soon as the game starts
        if (AstarPath.active != null)
        {
            AstarPath.active.Scan();
        }
    }
}