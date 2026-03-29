using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class AudioSettingUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Value Text")]
    public TMP_Text masterValueText;
    public TMP_Text musicValueText;
    public TMP_Text sfxValueText;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    void Start()
    {
        LoadVolume(masterSlider, "MasterVolume", masterValueText);
        LoadVolume(musicSlider, "MusicVolume", musicValueText);
        LoadVolume(sfxSlider, "SFXVolume", sfxValueText);

        masterSlider.onValueChanged.AddListener(v => SetVolume(masterSlider, "MasterVolume", masterValueText));
        musicSlider.onValueChanged.AddListener(v => SetVolume(musicSlider, "MusicVolume", musicValueText));
        sfxSlider.onValueChanged.AddListener(v => SetVolume(sfxSlider, "SFXVolume", sfxValueText));
    }

    void LoadVolume(Slider slider, string parameter, TMP_Text valueText)
    {
        float savedValue = Mathf.Clamp01(PlayerPrefs.GetFloat(parameter, 0.75f));

        slider.value = savedValue;

        UpdateText(slider, valueText);
        ApplyVolume(parameter, savedValue);
    }

    void SetVolume(Slider slider, string parameter, TMP_Text valueText)
    {
        float value = slider.value;

        ApplyVolume(parameter, value);

        PlayerPrefs.SetFloat(parameter, value);
        PlayerPrefs.Save();

        UpdateText(slider, valueText);
    }

    void ApplyVolume(string parameter, float value)
    {
        float volume = Mathf.Max(value, 0.0001f);
        audioMixer.SetFloat(parameter, Mathf.Log10(volume) * 20);
    }

    void UpdateText(Slider slider, TMP_Text text)
    {
        text.text = Mathf.RoundToInt(slider.value * 100) + "%";
    }
}