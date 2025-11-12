using UnityEngine;
using System.Collections;
public class MusicManager : MonoBehaviour
{
    [Header("Configurações de Música")]
    public AudioSource audioSource;
    public float targetVolume = 0.05f; // volume final da música
    public float fadeDuration = 3f;   // duração do fade-in em segundos

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0f;      // começa no mudo
        audioSource.loop = true;      // toca infinitamente
        audioSource.Play();           // inicia o som
        StartCoroutine(FadeIn());     // inicia o fade suave
    }

    IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, time / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume; // garante o volume final exato
    }
}
