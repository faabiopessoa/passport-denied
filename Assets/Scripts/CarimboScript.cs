using UnityEngine;
using UnityEngine.UI;
using TMPro; // necessário se estiver usando TextMeshPro

public class CarimboUI : MonoBehaviour
{
    public Button botaoAceitar;
    public Button botaoNegar;
    public Button botaoPassar;
    public TextMeshProUGUI pontuacaoText; 

    private int pontuacao = 0;

    void Start()
    {
        botaoAceitar.onClick.AddListener(AoAceitar);
        botaoNegar.onClick.AddListener(AoNegar);
        botaoPassar.onClick.AddListener(AoPassar);
        AtualizarPontuacao();
    }

    void AoAceitar()
    {
        pontuacao += 1;
        Debug.Log("Points: " + pontuacao);
        AtualizarPontuacao();
    }

    void AoNegar()
    {
        pontuacao -= 1;
        Debug.Log("Points: " + pontuacao);
        AtualizarPontuacao();
    }

    void AoPassar()
    {
        Debug.Log("Passou para o proximo candidato!");
        // Aqui entra a lógica quando o jogador passa
    }

    void AtualizarPontuacao()
    {
        pontuacaoText.text = "Points: " + pontuacao;
    }
}
