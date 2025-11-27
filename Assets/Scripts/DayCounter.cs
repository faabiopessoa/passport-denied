using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DayCounter : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Dia inicial ao abrir a cena")]
    public int startDay = 1;

    [Header("Referências")]
    public TextMeshProUGUI dayLabel;

    [Header("Eventos")]
    public UnityEvent<int> onDayChanged;

    private int _currentDay;

    void Start()
    {
        SetDay(startDay);
    }

    public void NextDay()
    {
        SetDay(_currentDay + 1);
    }

    public void PrevDay()
    {
        SetDay(Mathf.Max(1, _currentDay - 1));
    }

    public void SetDay(int day)
    {
        _currentDay = Mathf.Max(1, day);
        if (dayLabel != null)
            dayLabel.text = $"DIA \n{_currentDay:00}";

        onDayChanged?.Invoke(_currentDay);
    }

    public int GetCurrentDay() => _currentDay;
}
