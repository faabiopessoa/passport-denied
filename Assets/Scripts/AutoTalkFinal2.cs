using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AutoTalkFinal3 : MonoBehaviour
{
    public Animator animator;       
    public TMP_Text speechText;     
    public string frase = "";
    public float duracao = 3f;     
    public int win = 0; 

    private void Start()
    {
        if(win == 0)
        {
            StartCoroutine(Perdeu());
        }
        else
        {
            StartCoroutine(Venceu());
        }
    }

    private System.Collections.IEnumerator Perdeu()
    {
        animator.SetBool("isWin", false);

        speechText.text = "você perdeu";

        yield return new WaitForSeconds(duracao);

        animator.SetBool("isTalking", false);

        speechText.text = "";

        
        SceneManager.LoadScene("MenuInicial");
    }

    private System.Collections.IEnumerator Venceu()
    {
        animator.SetBool("isWin", true);

        speechText.text = "você venceu";

        yield return new WaitForSeconds(duracao);

        animator.SetBool("isTalking", false);

        speechText.text = "";

        
        SceneManager.LoadScene("MenuInicial");
    }
}
