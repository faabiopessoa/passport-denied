using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountdownHUD : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Duração inicial do atendimento, em segundos.")]
    public float startSeconds = 90f; 

    [Header("Referências")]
    public TextMeshProUGUI timeText;

    [Header("Eventos")]
    public UnityEvent onTimerEnd;

    private float _timeLeft;
    private bool _running;

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
        if (_timeLeft < 0f)
        {
            _timeLeft = 0f;
            _running = false;
            UpdateLabel();
            onTimerEnd?.Invoke();
            return;
        }

        UpdateLabel();
    }

    public void StartTimer()
    {
        _running = true;
    }

    public void PauseTimer()
    {
        _running = false;
    }

    public void ResetTimer()
    {
        _timeLeft = Mathf.Max(0f, startSeconds);
    }

    private void UpdateLabel()
    {
        if (!timeText) return;
        int m = Mathf.FloorToInt(_timeLeft / 60f);
        int s = Mathf.FloorToInt(_timeLeft % 60f);
        timeText.text = $"{m:00}:{s:00}";
    }
}
