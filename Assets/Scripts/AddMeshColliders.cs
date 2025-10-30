using UnityEngine;

public class AddMeshColliders : MonoBehaviour
{
    [ContextMenu("Add Mesh Colliders to Children")]
    void AddCollidersToChildren()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        
        foreach (MeshFilter meshFilter in meshFilters)
        {
            GameObject obj = meshFilter.gameObject;
            
            // Skip if already has a collider
            if (obj.GetComponent<Collider>() != null)
                continue;
            
            // Add MeshCollider
            MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
            meshCollider.convex = false;
            
            Debug.Log("Added MeshCollider to: " + obj.name);
        }
        
        Debug.Log("Finished adding mesh colliders!");
    }
}
