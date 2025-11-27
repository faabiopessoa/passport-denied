using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referências")]
    public GameObject characterPrefab;
    public Transform characterSpawnPoint;
    public CountdownHUD countdownHUD;
    
    // Referência nova para poder validar
    public PassportController passportController; 

    [Header("Configurações")]
    public float delayBetweenCharacters = 4f;

    private CharacterController currentCharacter;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        StartNewDay();
    }

    public void StartNewDay()
    {
        StartCoroutine(NextCharacterRoutine());
    }

    private IEnumerator NextCharacterRoutine()
    {
        yield return new WaitForSeconds(delayBetweenCharacters);

        if (currentCharacter != null) Destroy(currentCharacter.gameObject);

        SpawnNewCharacter();
        isTransitioning = false;
    }

    public void SpawnNewCharacter()
    {
        if (characterPrefab == null || characterSpawnPoint == null) return;

        GameObject newCharObject = Instantiate(characterPrefab, characterSpawnPoint.position, Quaternion.identity);
        currentCharacter = newCharObject.GetComponent<CharacterController>();

        // Gera um novo passaporte para essa nova pessoa
        if (passportController != null)
        {
            passportController.ClosePassport(); // Reseta visual
            passportController.GenerateNewStudent();
        }

        if (countdownHUD != null)
        {
            countdownHUD.ResetTimer();
            countdownHUD.StartTimer();
        }
    }

    // --- LÓGICA DE DECISÃO ---

    // wasApprovedByPlayer: TRUE se clicou no botão verde, FALSE se clicou no vermelho/timeout
    private void ProcessDecision(bool wasApprovedByPlayer)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (passportController != null)
        {
            bool isActuallyValid = passportController.IsCurrentPassportValid;

            // Verifica se o jogador acertou
            if (wasApprovedByPlayer == isActuallyValid)
            {
                Debug.Log("<color=cyan>SUCESSO! O jogador acertou a decisão.</color>");
                // Aqui você pode: Adicionar Dinheiro, Pontos, Tocar som de Sucesso
            }
            else
            {
                Debug.Log("<color=magenta>ERRO! O jogador errou.</color>");
                // Aqui você pode: Tirar Vidas, Multa, Tocar som de Erro
            }
        }

        // Animação de saída do personagem
        if (currentCharacter != null)
        {
            countdownHUD.PauseTimer();
            currentCharacter.StartExitSequence(wasApprovedByPlayer);
        }

        StartCoroutine(NextCharacterRoutine());
    }

    // Botões da UI chamam isso:
    public void HandleTimeOut() => ProcessDecision(false); // Timeout conta como recusa?
    public void AcceptDocument() => ProcessDecision(true);
    public void RefuseDocument() => ProcessDecision(false);
}