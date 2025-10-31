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

    private void Start()
    {
        musicSource.clip = bgm;
        musicSource.Play();
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
