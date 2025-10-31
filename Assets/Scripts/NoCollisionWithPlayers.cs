using UnityEngine;

public class NoCollisionWithPlayers : MonoBehaviour
{
    void Start()
    {
        // Set this object's collider as a trigger so characters pass through
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
}
