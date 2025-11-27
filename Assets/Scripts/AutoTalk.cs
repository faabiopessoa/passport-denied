using UnityEngine;
using TMPro; 

public class AutoTalk : MonoBehaviour
{
    public Animator animator;       
    public TMP_Text speechText;     
    public string frase = "oi meu nome é goku";
    public float duracao = 3f;      

    private void Start()
    {

        StartCoroutine(Falar());
    }

    private System.Collections.IEnumerator Falar()
    {
        // ativa animação de fala
        animator.SetBool("isTalking", true);

        // coloca o texto na tela
        speechText.text = frase;

        // espera o tempo configurado
        yield return new WaitForSeconds(duracao);

        // para de falar
        animator.SetBool("isTalking", false);

        // limpa ou deixa a frase (você escolhe)
        speechText.text = "Oi meu nome é goku";
    }
}
