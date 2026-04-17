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
        PlayMusic("theme");
    }
    void Update()
    {
        // ตรวจสอบว่ามีการกดปุ่มใดๆ และปุ่มนั้นไม่ใช่คลิกเมาส์ (0=ซ้าย, 1=ขวา, 2=กลาง)
        if (Input.anyKeyDown &&
            !Input.GetMouseButtonDown(0) &&
            !Input.GetMouseButtonDown(1) &&
            !Input.GetMouseButtonDown(2))
        {
            AudioManager.instance.PlaySFX("typing");
        }

        // แยกเสียงคลิกเมาส์ออกมาต่างหาก (ถ้าต้องการ)
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.instance.PlaySFX("click");
        }
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
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // ฟังก์ชันปรับความดัง SFX
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
