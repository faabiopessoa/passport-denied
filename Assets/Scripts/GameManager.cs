using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referências da Cena")]
    public CountdownHUD countdownHUD;
    public CharacterController currentCharacter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartNewDay();
    }

    public void StartNewDay()
    {
        // TODO: Resetar pontuação, dia, etc.
        Debug.Log("Novo dia");
        SpawnNewCharacter();
    }

    public void SpawnNewCharacter()
    {
        // TODO: Lógica para instanciar um novo prefab de personagem.
        // Por enquanto, vamos assumir que o 'currentCharacter' já está na cena
        // e foi arrastado no Inspector.
        if (currentCharacter != null)
        {
            countdownHUD.ResetTimer();
            countdownHUD.StartTimer();
        }
    }

    public void HandleTimeOut()
    {
        if (currentCharacter == null) return;

        Debug.Log("O tempo expirou!");

        countdownHUD.PauseTimer();

        currentCharacter.StartExitSequence(false);

        // TODO: Adicionar lógica para chamar o próximo personagem após um tempo.
        // ...SpawnNewCharacter() aqui depois de um delay.
    }

    public void AcceptDocument()
    {
        if (currentCharacter == null) return;

        Debug.Log("Documento ACEITO.");
        countdownHUD.PauseTimer();
        currentCharacter.StartExitSequence(true);
        // TODO: Adicionar lógica para chamar o próximo personagem após um tempo.
    }

    public void RefuseDocument()
    {
        if (currentCharacter == null) return;
        
        Debug.Log("Documento RECUSADO.");
        countdownHUD.PauseTimer();
        currentCharacter.StartExitSequence(false);
        // TODO: Adicionar lógica para chamar o próximo personagem após um tempo.
    }
}