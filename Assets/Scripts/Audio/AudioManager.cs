using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClipReference audioClipReference; // Reference to the ScriptableObject

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start playing background music if you have one set up with a specific ID
        PlayMusic("BackgroundMusic"); // Replace with your actual music ID
    }

    public void PlayMusic(string id)
    {
        AudioClip clip = audioClipReference.GetClipById(id);
        if (clip == null) return;

        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(string id)
    {
        AudioClip clip = audioClipReference.GetClipById(id);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Event handler methods
    public void OnLetterSelected()
    {
        PlaySFX("LetterSelect"); // Replace with your actual SFX ID
    }

    public void OnWordFound()
    {
        PlaySFX("WordFound"); // Replace with your actual SFX ID
    }
}
