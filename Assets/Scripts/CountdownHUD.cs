using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountdownHUD : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Duração inicial do atendimento, em segundos.")]
    public float startSeconds = 90f;

    [Tooltip("Segundos restantes para iniciar o alerta sonoro e visual.")]
    public float alertThreshold = 8f;

    [Header("Referências")]
    public TextMeshProUGUI timeText;
    [Tooltip("Fonte de áudio que tocará o alerta.")]
    public AudioSource alertSource;
    [Tooltip("Som de alerta (8 segundos de duração).")]
    public AudioClip alertClip;

    [Header("Eventos")]
    public UnityEvent onTimerEnd;

    private float _timeLeft;
    private bool _running;
    private bool _alertPlaying;
    private bool _isFlashing;

    private Color _defaultColor = Color.white;
    private Color _alertColor = new Color(1f, 0.25f, 0.25f); // vermelho claro
    private float _flashSpeed = 5f; // velocidade do piscar

    void Start()
    {
        ResetTimer();
        StartTimer();
        UpdateLabel();
    }

    void Update()
    {
        if (!_running) return;

        _timeLeft -= Time.deltaTime;

        // Inicia alerta sonoro e visual
        if (_timeLeft <= alertThreshold && !_alertPlaying && _timeLeft > 0)
        {
            PlayAlert();
            StartFlashing();
        }

        // Quando o tempo acaba
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            _running = false;
            StopAlert();
            StopFlashing();
            UpdateLabel();
            onTimerEnd?.Invoke();
            return;
        }

        UpdateLabel();

        // Atualiza a cor se estiver piscando
        if (_isFlashing)
            UpdateFlashEffect();
    }

    public void StartTimer()
    {
        _running = true;
    }

    public void PauseTimer()
    {
        _running = false;
        StopAlert();
        StopFlashing();
    }

    public void ResetTimer()
    {
        _timeLeft = Mathf.Max(0f, startSeconds);
        StopAlert();
        StopFlashing();
        if (timeText != null)
            timeText.color = _defaultColor;
    }

    private void PlayAlert()
    {
        if (alertSource != null && alertClip != null)
        {
            alertSource.clip = alertClip;
            alertSource.loop = false; // som tem 8s
            alertSource.Play();
            _alertPlaying = true;
        }
    }

    private void StopAlert()
    {
        if (alertSource != null && alertSource.isPlaying)
            alertSource.Stop();

        _alertPlaying = false;
    }

    private void StartFlashing()
    {
        _isFlashing = true;
    }

    private void StopFlashing()
    {
        _isFlashing = false;
        if (timeText != null)
            timeText.color = _defaultColor;
    }

    private void UpdateFlashEffect()
    {
        if (timeText == null) return;

        // alterna suavemente entre as cores
        float t = Mathf.PingPong(Time.time * _flashSpeed, 1f);
        timeText.color = Color.Lerp(_defaultColor, _alertColor, t);
    }

    private void UpdateLabel()
    {
        if (!timeText) return;
        int m = Mathf.FloorToInt(_timeLeft / 60f);
        int s = Mathf.FloorToInt(_timeLeft % 60f);
        timeText.text = $"{m:00}:{s:00}";
    }
}
