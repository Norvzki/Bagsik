using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------- Audio Source -----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("----------- Audio Clip -----------")]
    public AudioClip bgm;
    public AudioClip earthquake;
    public AudioClip Jump;
    public AudioClip victory;
    public AudioClip walking;
    public AudioClip follow;
    public AudioClip zone;
    public AudioClip win;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Use bgm if assigned, otherwise use background
        AudioClip clipToPlay = bgm;
        if (clipToPlay != null)
        {
            musicSource.clip = clipToPlay;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource == null)
        {
            Debug.LogError("SFXSource is not assigned in AudioManager!");
            return;
        }
        if (clip == null)
        {
            Debug.LogError("Attempted to play null AudioClip!");
            return;
        }
        
        SFXSource.PlayOneShot(clip);
        Debug.Log($"Playing SFX: {clip.name}");
    }

    public void StopSFX()
    {
        SFXSource.Stop();
    }
}
