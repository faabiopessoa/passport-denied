using UnityEngine;
using TMPro;
using System.Collections;

public class DayTransitionUI : MonoBehaviour
{
    public TextMeshProUGUI mainMessageText;
    public TextMeshProUGUI subMessageText;
    public float displayDuration = 3f; // Quanto tempo a tela fica ativa

    // Método para exibir informações sobre a transição
    public void SetupTransition(int completedDay, int nextDay)
    {
        if (mainMessageText != null)
        {
            mainMessageText.text = $"DIA {completedDay} CONCLUÍDO!";
        }
        if (subMessageText != null)
        {
            subMessageText.text = $"PREPARANDO PARA O DIA {nextDay}...";
        }
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        // O GameManager será responsável por destruir esta tela.
        // Se você quiser um botão "Continuar", não destrua aqui.
    }
}