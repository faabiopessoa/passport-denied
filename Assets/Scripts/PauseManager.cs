using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    private InputAction pauseAction;

    void Awake()
    {
        pauseAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
        pauseAction.performed += ctx => TogglePause();
    }

    void OnEnable()
    {
        pauseAction.Enable();
    }

    void OnDisable()
    {
        pauseAction.Disable();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = isPaused ? 0f : 1f;

        // 🎵 Pausar / Retomar a música corretamente
        if (MusicManager.Instance != null)
        {
            if (isPaused)
                MusicManager.Instance.musicSource.Pause();
            else
                MusicManager.Instance.musicSource.UnPause();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.musicSource.UnPause();
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.musicSource.UnPause();

        SceneManager.LoadScene("MenuInicial");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
