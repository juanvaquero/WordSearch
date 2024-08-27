using UnityEngine;
using static AudioClipReference;

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
        PlayMusic(AudioReferences.BACKGROUND_MUSIC);
    }

    public void PlayMusic(string id)
    {
        AudioClipEntry clipRef = audioClipReference.GetClipById(id);
        if (clipRef.Clip == null) return;

        if (musicSource.clip == clipRef.Clip) return;
        musicSource.clip = clipRef.Clip;
        musicSource.loop = true;
        musicSource.pitch = clipRef.Pitch;
        musicSource.Play();
    }

    public void PlaySFX(string id)
    {
        AudioClipEntry clipRef = audioClipReference.GetClipById(id);
        if (clipRef.Clip != null)
        {
            sfxSource.pitch = clipRef.Pitch;
            sfxSource.PlayOneShot(clipRef.Clip);
        }
    }
}
