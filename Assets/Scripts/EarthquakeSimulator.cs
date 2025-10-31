using UnityEngine;

public class EarthquakeSimulator : MonoBehaviour
{
    [Header("Earthquake Settings")]
    public float earthquakeDuration = 5f;
    public float shakeIntensity = 0.3f;
    public float shakeSpeed = 25f;
    public float pushForce = 1f;
    public float upwardForce = 0.3f;
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
            
            if (shouldShake && obj.transform.parent != null)
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
                if (rb == null) continue;

                // Calculate random shake forces using Perlin noise
                float noiseX = (Mathf.PerlinNoise(Time.time * shakeSpeed, rb.GetInstanceID()) - 0.5f) * 2f;
                float noiseY = (Mathf.PerlinNoise(rb.GetInstanceID(), Time.time * shakeSpeed) - 0.5f) * 2f;
                float noiseZ = (Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed + rb.GetInstanceID()) - 0.5f) * 2f;

                Vector3 shakeForce = new Vector3(noiseX, noiseY, noiseZ) * pushForce;

                // Add upward force to make objects jump/bounce
                shakeForce.y += upwardForce * Mathf.Abs(noiseY);

                // Apply force to rigidbody
                rb.AddForce(shakeForce, ForceMode.Force);

                // Apply random torque to make objects tumble
                Vector3 randomTorque = new Vector3(
                    (Mathf.PerlinNoise(Time.time * shakeSpeed * 0.7f, rb.GetInstanceID() + 100) - 0.5f) * 2f,
                    (Mathf.PerlinNoise(Time.time * shakeSpeed * 0.7f, rb.GetInstanceID() + 200) - 0.5f) * 2f,
                    (Mathf.PerlinNoise(Time.time * shakeSpeed * 0.7f, rb.GetInstanceID() + 300) - 0.5f) * 2f
                ) * torqueForce;

                rb.AddTorque(randomTorque, ForceMode.Force);
            }

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

    void StartEarthquake()
    {
        isShaking = true;
        shakeTimer = 0f;

        audioManager.PlaySFX(audioManager.earthquake);
        // Enable physics on all affected objects
        foreach (Rigidbody rb in affectedObjects)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.WakeUp();
            }
        }
        
        Debug.Log("Earthquake started! Objects will now fall and move!");
    }
    
    void StopEarthquake()
    {
        isShaking = false;

        audioManager.StopSFX();
        
        // Objects stay where they fell - no reset!
        // Physics remains enabled so objects can settle naturally
        
        Debug.Log("Earthquake ended! Objects remain where they fell.");
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
