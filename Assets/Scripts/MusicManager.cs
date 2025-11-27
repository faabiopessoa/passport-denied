using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Áudios das cenas")]
    public AudioClip menuMusic;
    public AudioClip officeMusic;

    [Header("Volumes Globais")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Configurações")]
    public AudioSource musicSource;
    public AudioSource sfxSource;  // agora temos um canal separado!
    public float fadeDuration = 2f;

    private string lastScene = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource == null)
                musicSource = GetComponent<AudioSource>();

            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadSettings();
        OnSceneChanged(default, SceneManager.GetActiveScene());
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        string sceneName = newScene.name;

        if (sceneName == lastScene) return;
        lastScene = sceneName;

        if (sceneName == "MenuInicial")
            PlayMusic(menuMusic);
        else if (sceneName == "SampleScene 1")
            PlayMusic(officeMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeToClip(clip));
    }

    IEnumerator FadeToClip(AudioClip newClip)
    {
        float startVol = musicSource.volume;

        // Fade Out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        // Fade In
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    // FORA DE QUALQUER MÉTODO:
    public void SetMusicVolume(float v)
    {
        musicVolume = v;
        musicSource.volume = v;
        PlayerPrefs.SetFloat("MusicVolume", v);
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = v;
        sfxSource.volume = v;
        PlayerPrefs.SetFloat("SFXVolume", v);
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicSource.volume = musicVolume;
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            sfxSource.volume = sfxVolume;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
