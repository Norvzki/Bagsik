using UnityEditor.SearchService;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-------------- Audio Source -----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-------------- Audio Clip -----------")]
    public AudioClip background;
    public AudioClip walking;
    public AudioClip earthquake;
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






    public void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }



}