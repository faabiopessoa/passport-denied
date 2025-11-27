using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider sensSlider;

    void Start()
    {
        // Carrega valores atuais
        musicSlider.value = MusicManager.Instance.musicVolume;
        sfxSlider.value = MusicManager.Instance.sfxVolume;
        sensSlider.value = PlayerPrefs.GetFloat("MouseSens", 1f);

        // Listener
        musicSlider.onValueChanged.AddListener(OnMusicChange);
        sfxSlider.onValueChanged.AddListener(OnSFXChange);
        sensSlider.onValueChanged.AddListener(OnSensChange);
    }

    void OnMusicChange(float v)
    {
        MusicManager.Instance.SetMusicVolume(v);
    }

    void OnSFXChange(float v)
    {
        MusicManager.Instance.SetSFXVolume(v);
    }

    void OnSensChange(float v)
    {
        PlayerPrefs.SetFloat("MouseSens", v);
    }
}
