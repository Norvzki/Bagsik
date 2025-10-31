using UnityEngine;

public class EarthquakeSimulator : MonoBehaviour
{
    [Header("Earthquake Settings")]
    public float earthquakeDuration = 5f;
    public float shakeIntensity = 0.3f;
    public float shakeSpeed = 25f;
    public float pushForce = 1f;
    public float upwardForce = 1.5f;
    public float torqueForce = 0.5f;
    public KeyCode triggerKey = KeyCode.E;
    
    [Header("What to Shake")]
    public string[] tagsToShake = { "Furniture", "Prop", "Decoration" };
    public bool shakeUntaggedObjects = true;
    
    private bool isShaking = false;
    private float shakeTimer = 0f;
    
    // Store rigidbodies of objects that will move
    private System.Collections.Generic.List<Rigidbody> affectedObjects = new System.Collections.Generic.List<Rigidbody>();
    
    void Start()
    {
        // Find all objects to shake and add physics to them
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
            
            if (shouldShake)
            {
                // Add Rigidbody if it doesn't have one
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = obj.AddComponent<Rigidbody>();
                    rb.mass = 1f;
                    rb.linearDamping = 0.5f;
                    rb.angularDamping = 0.5f;
                    rb.useGravity = true; // Make sure gravity is enabled!
                }
                
                // Add collider if it doesn't have one
                if (obj.GetComponent<Collider>() == null)
                {
                    BoxCollider collider = obj.AddComponent<BoxCollider>();
                }
                
                // Start with physics disabled
                rb.isKinematic = true;
                
                affectedObjects.Add(rb);
                Debug.Log($"Added {obj.name} to earthquake objects (Tag: {obj.tag})");
            }
        }
        
        Debug.Log($"EarthquakeSimulator: Found {affectedObjects.Count} objects to shake. Press '{triggerKey}' to trigger earthquake.");
    }

    void Update()
    {
        // Trigger earthquake
        if (Input.GetKeyDown(triggerKey) && !isShaking)
        {
            StartEarthquake();
        }

        // Apply forces during earthquake
        if (isShaking)
        {
            shakeTimer += Time.deltaTime;

            foreach (Rigidbody rb in affectedObjects)
            {
                if (rb == null || !originalPositions.ContainsKey(rb)) continue;

                // Calculate random shake offset using Perlin noise
                float noiseX = (Mathf.PerlinNoise(Time.time * shakeSpeed, rb.GetInstanceID()) - 0.5f) * 2f;
                float noiseY = (Mathf.PerlinNoise(rb.GetInstanceID(), Time.time * shakeSpeed) - 0.5f) * 2f;
                float noiseZ = (Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed + rb.GetInstanceID()) - 0.5f) * 2f;

                Vector3 shakeOffset = new Vector3(noiseX, noiseY, noiseZ) * shakeIntensity;

                // Apply shake offset to original position (vibrate in place)
                rb.transform.position = originalPositions[rb] + shakeOffset;
            }
            
            // Damage players who are not ducking
            DamagePlayers();

            // End earthquake
            if (shakeTimer >= earthquakeDuration)
            {
                StopEarthquake();
            }
        }
    }
    
    AudioManager audioManager;

    void Awake()
    {
        var audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject == null)
        {
            Debug.LogError("No GameObject with 'Audio' tag found!");
            return;
        }
        
        audioManager = audioObject.GetComponent<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager component not found on Audio GameObject!");
        }
        else
        {
            Debug.Log("AudioManager found and assigned successfully");
        }
    }

    // Store original positions for vibration
    private System.Collections.Generic.Dictionary<Rigidbody, Vector3> originalPositions = new System.Collections.Generic.Dictionary<Rigidbody, Vector3>();
    
    void DamagePlayers()
    {
        // Find all players with PlayerHealth component
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        
        foreach (PlayerHealth player in players)
        {
            if (player != null && !player.IsDead())
            {
                // Only damage if not ducking
                if (!player.IsDucking())
                {
                    player.TakeDamage(player.earthquakeDamagePerSecond * Time.deltaTime);
                }
            }
        }
    }

    void StartEarthquake()
    {
        isShaking = true;
        shakeTimer = 0f;

        if (audioManager != null)
            audioManager.PlayLoopingSFX(audioManager.earthquake);
        
        // Store original positions
        originalPositions.Clear();
        foreach (Rigidbody rb in affectedObjects)
        {
            if (rb != null)
            {
                originalPositions[rb] = rb.transform.position;
            }
        }
        
        Debug.Log("Earthquake started! Objects will vibrate!");
    }
    
    void StopEarthquake()
    {
        isShaking = false;

        if (audioManager != null)
            audioManager.StopSFX();
        
        // Return objects to original positions
        foreach (Rigidbody rb in affectedObjects)
        {
            if (rb != null && originalPositions.ContainsKey(rb))
            {
                rb.transform.position = originalPositions[rb];
            }
        }
        
        originalPositions.Clear();
        
        Debug.Log("Earthquake ended! Objects returned to original positions.");
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
