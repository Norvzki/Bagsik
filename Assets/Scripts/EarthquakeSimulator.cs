using UnityEngine;

public class EarthquakeSimulator : MonoBehaviour
{
    [Header("Earthquake Settings")]
    public float earthquakeDuration = 5f;
    public float shakeIntensity = 0.3f;
    public float shakeSpeed = 25f;
    public KeyCode triggerKey = KeyCode.E;
    
    [Header("What to Shake")]
    public string[] tagsToShake = { "Furniture", "Prop", "Decoration" };
    public bool shakeUntaggedObjects = true;
    
    private bool isShaking = false;
    private float shakeTimer = 0f;
    
    // Store original positions and rotations
    private System.Collections.Generic.Dictionary<Transform, Vector3> originalPositions = new System.Collections.Generic.Dictionary<Transform, Vector3>();
    private System.Collections.Generic.Dictionary<Transform, Quaternion> originalRotations = new System.Collections.Generic.Dictionary<Transform, Quaternion>();
    
    void Start()
    {
        // Find all objects to shake and store their original transforms
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // Skip if it's a player, camera, light, or has specific components we don't want to shake
            if (obj.GetComponent<Camera>() != null || 
                obj.GetComponent<Light>() != null ||
                obj.GetComponent<TwoPlayerController>() != null ||
                obj.name.Contains("Player") ||
                obj.name.Contains("Ground") ||
                obj.name.Contains("Floor") ||
                obj.name.Contains("Wall") ||
                obj.name.Contains("Roof") ||
                obj.name.Contains("FullHouse"))
                continue;
            
            // Check if object should be shaken based on tags
            bool shouldShake = false;
            
            if (shakeUntaggedObjects && obj.tag == "Untagged")
            {
                shouldShake = true;
            }
            else
            {
                foreach (string tag in tagsToShake)
                {
                    if (obj.tag == tag)
                    {
                        shouldShake = true;
                        break;
                    }
                }
            }
            
            if (shouldShake && obj.transform.parent != null)
            {
                // Only shake child objects (furniture parts, decorations, etc.)
                originalPositions[obj.transform] = obj.transform.localPosition;
                originalRotations[obj.transform] = obj.transform.localRotation;
            }
        }
        
        Debug.Log($"EarthquakeSimulator: Found {originalPositions.Count} objects to shake. Press '{triggerKey}' to trigger earthquake.");
    }
    
    void Update()
    {
        // Trigger earthquake
        if (Input.GetKeyDown(triggerKey) && !isShaking)
        {
            StartEarthquake();
        }
        
        // Shake objects during earthquake
        if (isShaking)
        {
            shakeTimer += Time.deltaTime;
            
            foreach (var kvp in originalPositions)
            {
                Transform obj = kvp.Key;
                if (obj == null) continue;
                
                Vector3 originalPos = kvp.Value;
                Quaternion originalRot = originalRotations[obj];
                
                // Calculate shake offset using Perlin noise for smooth random movement
                float noiseX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f;
                float noiseY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f;
                float noiseZ = (Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed) - 0.5f) * 2f;
                
                Vector3 shakeOffset = new Vector3(noiseX, noiseY, noiseZ) * shakeIntensity;
                
                // Apply shake to position
                obj.localPosition = originalPos + shakeOffset;
                
                // Apply slight rotation shake
                float rotationShake = (Mathf.PerlinNoise(Time.time * shakeSpeed * 0.5f, 100f) - 0.5f) * 2f * shakeIntensity * 10f;
                obj.localRotation = originalRot * Quaternion.Euler(rotationShake, rotationShake * 0.5f, rotationShake * 0.3f);
            }
            
            // End earthquake
            if (shakeTimer >= earthquakeDuration)
            {
                StopEarthquake();
            }
        }
    }
    
    void StartEarthquake()
    {
        isShaking = true;
        shakeTimer = 0f;
        Debug.Log("Earthquake started!");
    }
    
    void StopEarthquake()
    {
        isShaking = false;
        
        // Return all objects to original positions
        foreach (var kvp in originalPositions)
        {
            Transform obj = kvp.Key;
            if (obj == null) continue;
            
            obj.localPosition = kvp.Value;
            obj.localRotation = originalRotations[obj];
        }
        
        Debug.Log("Earthquake ended!");
    }
    
    // Optional: Trigger earthquake from other scripts
    public void TriggerEarthquake()
    {
        if (!isShaking)
        {
            StartEarthquake();
        }
    }
}
