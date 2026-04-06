using UnityEngine;
using Pathfinding;

public class EnemyRotation : MonoBehaviour
{
    public AIPath aiPath;

    void Start()
    {
        // Dacă ai uitat să tragi referința în Inspector, 
        // acest cod o va căuta automat în obiectele părinte.
        if (aiPath == null)
        {
            aiPath = GetComponentInParent<AIPath>();
        }
    }

    void Update()
    {
        // Verificăm dacă aiPath există pentru a evita eroarea "NullReferenceException"
        if (aiPath == null) return;

        // Logica ta de flip (scara originală pare a fi ~1.9, deci folosim 1f pentru direcție)
        // Atenție: Dacă liliacul tău dispare sau se face mic, 
        // înlocuiește 1f cu valoarea sa de scală originală (1.906513f din imaginea ta).

        float scaleX = Mathf.Abs(transform.localScale.x); // Păstrează mărimea curentă

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