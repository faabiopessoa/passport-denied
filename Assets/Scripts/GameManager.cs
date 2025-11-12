using UnityEngine;
using System.Collections; // Necessário para Coroutines
using TMPro; // Necessário para TextMeshPro
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referências de Prefabs e Cena")]
    [Tooltip("O Prefab do personagem que será instanciado.")]
    public GameObject characterPrefab;
    [Tooltip("O local onde os novos personagens irão surgir.")]
    public Transform characterSpawnPoint;
    [Tooltip("A referência para o script do HUD do contador.")]
    public CountdownHUD countdownHUD;

    [Header("Configurações de Gameplay")]
    [Tooltip("O tempo em segundos a esperar antes de gerar um novo personagem após o anterior sair.")]
    public float delayBetweenCharacters = 4f;

    private CharacterController currentCharacter;
    private bool isTransitioning = false; // Evitar ações múltiplas durante transições

    public Button botaoAceitar;
    public Button botaoNegar;
    public Button botaoPassar;
    public TextMeshProUGUI pontuacaoText; 

    private int pontuacao = 0;

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
        botaoAceitar.onClick.AddListener(AoAceitar);
        botaoNegar.onClick.AddListener(AoNegar);
        botaoPassar.onClick.AddListener(AoPassar);
        AtualizarPontuacao();
    }

    public void StartNewDay()
    {
        Debug.Log("Novo dia começando!");
        // Inicio do ciclo de personagens
        StartCoroutine(NextCharacterRoutine());
    }

    /// <summary>
    /// A rotina principal que gerencia a transição entre personagens.
    /// </summary>
    private IEnumerator NextCharacterRoutine()
    {
        // 1. Espera um tempo antes de gerar o próximo (útil entre personagens).
        yield return new WaitForSeconds(delayBetweenCharacters);

        // 2. Destrói o personagem anterior, se ele existir.
        if (currentCharacter != null)
        {
            Destroy(currentCharacter.gameObject);
        }

        // 3. Spawna um novo personagem.
        SpawnNewCharacter();
        isTransitioning = false;
    }

    public void SpawnNewCharacter()
    {
        if (characterPrefab == null || characterSpawnPoint == null)
        {
            Debug.LogError("Prefab do Personagem ou Ponto de Spawn não foram definidos no GameManager!");
            return;
        }

        // Instancia o prefab no local de spawn.
        GameObject newCharObject = Instantiate(characterPrefab, characterSpawnPoint.position, Quaternion.identity);
        currentCharacter = newCharObject.GetComponent<CharacterController>();

        // Reseta e inicia o timer para o novo personagem.
        if (countdownHUD != null)
        {
            countdownHUD.ResetTimer();
            countdownHUD.StartTimer();
        }
    }

    /// <summary>
    /// Método centralizado para processar uma decisão (Aceitar, Recusar, Timeout).
    /// </summary>
    private void ProcessDecision(bool wasApproved)
    {
        // Se já estamos em transição, ignora cliques repetidos.
        if (isTransitioning) return;
        isTransitioning = true; // Ativa a trava

        if (currentCharacter == null) return;

        countdownHUD.PauseTimer();
        currentCharacter.StartExitSequence(wasApproved);

        // Inicia a rotina para trazer o próximo personagem.
        StartCoroutine(NextCharacterRoutine());
    }

    // --- Métodos Públicos Chamados pela UI e Eventos ---

    public void HandleTimeOut()
    {
        Debug.Log("O tempo expirou! Processando recusa.");
        ProcessDecision(false);
    }

    public void AcceptDocument()
    {
        Debug.Log("Documento ACEITO. Processando decisão.");
        ProcessDecision(true);
    }

    public void RefuseDocument()
    {
        Debug.Log("Documento RECUSADO. Processando decisão.");
        ProcessDecision(false);
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