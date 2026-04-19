using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private Sound[] musicSounds, sfxSounds;
    [SerializeField] public AudioSource musicSource, sfxSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
    void Start()
    {
        PlayMusic("MainTheme");
    }

    public void PlayMusic(string name)
    {
        Sound s = System.Array.Find(musicSounds, sound => sound.name == name);
        if (s != null)
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void PlaySFX(string name)
    {
        Sound s = System.Array.Find(sfxSounds, sound => sound.name == name);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
