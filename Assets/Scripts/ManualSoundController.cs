using UnityEngine;

public class ManualSoundController : MonoBehaviour
{
    [Header("Configurações de Áudio")]
    public AudioSource sfxSource;
    public AudioClip clickSound;

    // Funções chamadas pelos botões e eventos do Manual
    public void PlayClickSound()
    {
        if (sfxSource != null && clickSound != null)
            sfxSource.PlayOneShot(clickSound);
    }
}
