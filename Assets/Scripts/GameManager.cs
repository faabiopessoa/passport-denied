using UnityEngine;
using System.Collections; // Necessário para Coroutines
using System.Collections.Generic; // Necessário para List

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuração de Níveis/Dias")] // NOVO HEADER
    [Tooltip("A lista de configurações para cada dia do jogo.")] // NOVO
    public List<DayConfig> dayConfigurations; // NOVO: Lista de ScriptableObjects DayConfig

    [Header("Referências de Prefabs e Cena")]
    [Tooltip("O Prefab do personagem que será instanciado.")]
    public GameObject characterPrefab;
    [Tooltip("O local onde os novos personagens irão surgir.")]
    public Transform characterSpawnPoint;
    [Tooltip("A referência para o script do HUD do contador.")]
    public CountdownHUD countdownHUD;
    [Tooltip("A referência para o script do contador de dias.")] // NOVO
    public DayCounter dayCounter; // NOVO: Referência para o DayCounter
    [Tooltip("O Prefab da tela de transição entre os dias.")] // NOVO
    public GameObject transitionScreenPrefab; // NOVO: Prefab da tela de transição

    // REMOVIDO: delayBetweenCharacters (agora vem do DayConfig)

    private CharacterController currentCharacter;
    private bool isTransitioning = false; // Evitar ações múltiplas durante transições

    private int currentDayIndex = 0; // NOVO: Índice do dia atual na lista dayConfigurations
    private int charactersProcessedToday = 0; // NOVO: Contador de personagens processados no dia
    private DayConfig currentDayConfig; // NOVO: A configuração do dia atual

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
        // Certifique-se de que temos configurações de dias.
        if (dayConfigurations == null || dayConfigurations.Count == 0) // NOVO
        {
            Debug.LogError("Nenhuma configuração de dia encontrada no GameManager!"); // NOVO
            return; // NOVO
        }

        // Inicia o jogo no primeiro dia.
        StartNewDay(0); // ALTERADO: Começa com o índice 0 (primeiro dia)
    }

    /// <summary>
    /// Inicia um novo dia com base no índice fornecido.
    /// </summary>
    public void StartNewDay(int dayIndex) // ALTERADO: Adicionado parâmetro dayIndex
    {
        currentDayIndex = dayIndex; // NOVO
        if (currentDayIndex >= dayConfigurations.Count) // NOVO: Verifica se todos os dias foram completados
        {
            Debug.Log("Todos os dias foram completados! Fim do jogo."); // NOVO
            // TODO: Chamar tela de fim de jogo ou créditos
            return; // NOVO
        }

        currentDayConfig = dayConfigurations[currentDayIndex]; // NOVO: Carrega a configuração do dia
        Debug.Log($"Novo dia {currentDayIndex + 1} começando com {currentDayConfig.numberOfCharacters} personagens e {currentDayConfig.characterServiceDuration}s por personagem!"); // NOVO

        // Atualiza o DayCounter
        if (dayCounter != null) // NOVO
        {
            dayCounter.SetDay(currentDayIndex + 1); // NOVO: +1 para mostrar dia 1, 2, etc.
        }

        charactersProcessedToday = 0; // NOVO: Reseta o contador de personagens do dia
        isTransitioning = false; // NOVO: Garante que a transição está desativada ao iniciar o dia

        // Configura o HUD do contador com a duração específica deste dia
        if (countdownHUD != null) // NOVO
        {
            // O SetStartSeconds precisa ser adicionado ao CountdownHUD.cs
            countdownHUD.SetStartSeconds(currentDayConfig.characterServiceDuration); // NOVO
        }

        // Inicio do ciclo do primeiro personagem do dia
        // ALTERADO: Adicionado parâmetro isFirstCharacterOfDay para NextCharacterRoutine
        StartCoroutine(NextCharacterRoutine(true)); // Força a spawn do primeiro personagem sem atraso inicial
    }

    /// <summary>
    /// A rotina principal que gerencia a transição entre personagens.
    /// </summary>
    private IEnumerator NextCharacterRoutine(bool isFirstCharacterOfDay = false) // ALTERADO: Adicionado parâmetro
    {
        // 1. Espera um tempo antes de gerar o próximo (útil entre personagens).
        // ALTERADO: Usa o delay do DayConfig e verifica se não é o primeiro personagem.
        if (!isFirstCharacterOfDay && currentDayConfig.delayBetweenCharacters > 0f) // NOVO (com delay do DayConfig)
        {
            yield return new WaitForSeconds(currentDayConfig.delayBetweenCharacters);
        }

        // 2. Destrói o personagem anterior, se ele existir.
        if (currentCharacter != null)
        {
            Destroy(currentCharacter.gameObject);
            currentCharacter = null; // NOVO: Limpa a referência
        }

        // NOVO: Verifica se ainda há personagens para este dia.
        if (charactersProcessedToday < currentDayConfig.numberOfCharacters)
        {
            // 3. Spawna um novo personagem.
            SpawnNewCharacter();
        }
        else
        {
            Debug.Log($"Todos os {currentDayConfig.numberOfCharacters} personagens do dia {currentDayIndex + 1} foram atendidos.");
            // Todos os personagens do dia foram atendidos, hora da transição de dia.
            StartCoroutine(DayTransitionRoutine()); // NOVO: Chama a rotina de transição de dia
        }

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

        charactersProcessedToday++; // NOVO: Incrementa o contador de personagens processados
    }

    /// <summary>
    /// Método centralizado para processar uma decisão (Aceitar, Recusar, Timeout).
    /// </summary>
    private void ProcessDecision(bool wasApproved)
    {
        // Se já estamos em transição, ignora cliques repetidos.
        if (isTransitioning) return;
        isTransitioning = true; // Ativa a trava

        if (currentCharacter == null) // NOVO: Adicionado verificação para currentCharacter null
        {
            isTransitioning = false; // NOVO: Libera a trava se currentCharacter for null
            return;
        }

        countdownHUD.PauseTimer();
        currentCharacter.StartExitSequence(wasApproved);

        // Inicia a rotina para trazer o próximo personagem ou finalizar o dia. // ALTERADO: Comentário
        StartCoroutine(NextCharacterRoutine());
    }

    // --- Rotina de Transição de Dia --- // NOVO MÉTODO
    private IEnumerator DayTransitionRoutine()
    {
        Debug.Log("Iniciando transição para o próximo dia...");

        // Desativa a interface do jogo (HUD, controles, etc.) para a transição
        // Você precisará de referências para os elementos da UI principal para desativá-los
        // Exemplo: UIManager.Instance.HideGameUI();

        // Instancia a tela de transição
        GameObject transitionScreenInstance = null; // ALTERADO nome da variável
        if (transitionScreenPrefab != null)
        {
            transitionScreenInstance = Instantiate(transitionScreenPrefab, Vector3.zero, Quaternion.identity);
            // Garante que a tela de transição esteja na camada de UI ou Canvas apropriado
            Canvas parentCanvas = GameObject.FindObjectOfType<Canvas>(); // NOVO
            if (parentCanvas != null) // NOVO
            {
                transitionScreenInstance.transform.SetParent(parentCanvas.transform, false); // NOVO
                // Redefine a escala e posição para garantir que cubra o Canvas
                RectTransform rt = transitionScreenInstance.GetComponent<RectTransform>(); // NOVO
                if (rt != null) // NOVO
                {
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                    rt.sizeDelta = Vector2.zero;
                    rt.anchoredPosition = Vector2.zero;
                }
            }
            else
            {
                Debug.LogError("Nenhum Canvas encontrado na cena para a tela de transição!"); // NOVO
            }

            DayTransitionUI transitionUI = transitionScreenInstance.GetComponent<DayTransitionUI>(); // NOVO
            if (transitionUI != null) // NOVO
            {
                transitionUI.SetupTransition(currentDayIndex + 1, currentDayIndex + 2); // NOVO
                yield return new WaitForSeconds(transitionUI.displayDuration); // NOVO
            }
            else
            {
                yield return new WaitForSeconds(3f); // Tempo padrão se o script DayTransitionUI não for encontrado
            }
        }
        else
        {
            yield return new WaitForSeconds(3f); // Tempo padrão se o prefab não for definido
        }


        // Destrói a tela de transição
        if (transitionScreenInstance != null)
        {
            Destroy(transitionScreenInstance);
        }

        // Reativa a interface do jogo (se você a desativou)
        // Exemplo: UIManager.Instance.ShowGameUI();

        // Inicia o próximo dia
        StartNewDay(currentDayIndex + 1);
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
}