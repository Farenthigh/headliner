using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public AudioMixer audioMixer;

    private void Start()
    {
        // โหลดค่าที่เคยเซฟ
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        masterSlider.value = savedMaster;
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}