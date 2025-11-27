using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountdownHUD : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Duração inicial do atendimento, em segundos. Valor padrão.")]
    public float defaultStartSeconds = 90f; // Mudou para defaultStartSeconds

    [Header("Referências")]
    public TextMeshProUGUI timeText;

    [Header("Eventos")]
    public UnityEvent onTimerEnd;

    private float _timeLeft;
    private bool _running;
    private float _currentServiceDuration; // Armazena a duração do serviço para o personagem atual

    void Start()
    {
        _currentServiceDuration = defaultStartSeconds; // Inicia com o valor padrão
        ResetTimer();
        UpdateLabel(); // Garante que o label exiba o tempo inicial correto
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
        _timeLeft = Mathf.Max(0f, _currentServiceDuration); // Usa a duração configurada
    }

    /// <summary>
    /// Define a duração do timer para o próximo personagem.
    /// Chamado pelo GameManager ao iniciar um novo dia ou personagem.
    /// </summary>
    public void SetStartSeconds(float newDuration)
    {
        _currentServiceDuration = newDuration;
        ResetTimer(); // Reseta o timer com a nova duração
        // O timer não é iniciado automaticamente aqui, o GameManager fará isso.
    }

    private void UpdateLabel()
    {
        if (!timeText) return;
        int m = Mathf.FloorToInt(_timeLeft / 60f);
        int s = Mathf.FloorToInt(_timeLeft % 60f);
        timeText.text = $"{m:00}:{s:00}";
    }
}