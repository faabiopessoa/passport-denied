using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuração de Níveis/Dias")]
    public List<DayConfig> dayConfigurations;

    [Header("Referências de Prefabs e Cena")]
    public GameObject characterPrefab;
    public Transform characterSpawnPoint;
    public CountdownHUD countdownHUD;
    public DayCounter dayCounter; 
    public GameObject transitionScreenPrefab;

    [Header("Referências de UI e Controle")]
    // IMPORTANTE: Arraste o objeto que tem o script PassportController aqui no Inspector
    public PassportController passportController; 
    
    // Botões que agora estão na cena principal
    public Button botaoAceitar;
    public Button botaoNegar;
    public Button botaoPassar;
    public TextMeshProUGUI pontuacaoText; 

    private CharacterController currentCharacter;
    private bool isTransitioning = false; 

    private int currentDayIndex = 0; 
    private int charactersProcessedToday = 0; 
    private DayConfig currentDayConfig; 
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
        if (dayConfigurations == null || dayConfigurations.Count == 0)
        {
            Debug.LogError("Nenhuma configuração de dia encontrada no GameManager!");
            return;
        }

        // Inicia o jogo no primeiro dia.
        StartNewDay(0);

        // Configura os cliques dos botões
        if(botaoAceitar != null) botaoAceitar.onClick.AddListener(AoAceitar);
        if(botaoNegar != null) botaoNegar.onClick.AddListener(AoNegar);
        if(botaoPassar != null) botaoPassar.onClick.AddListener(AoPassar);
        
        AtualizarPontuacao();
    }

    public void StartNewDay(int dayIndex) 
    {
        currentDayIndex = dayIndex; 
        if (currentDayIndex >= dayConfigurations.Count) 
        {
            Debug.Log("Todos os dias foram completados! Fim do jogo."); 
            return; 
        }

        currentDayConfig = dayConfigurations[currentDayIndex]; 
        
        if (dayCounter != null) 
        {
            dayCounter.SetDay(currentDayIndex + 1); 
        }

        charactersProcessedToday = 0; 
        isTransitioning = false; 

        if (countdownHUD != null) 
        {
            countdownHUD.SetStartSeconds(currentDayConfig.characterServiceDuration); 
        }

        // Força a atualização das regras do dia no RuleManager, se existir
        RuleManager ruleManager = FindObjectOfType<RuleManager>(); // Correção aqui também por garantia
        if (ruleManager != null)
        {
            ruleManager.OnDayChanged(currentDayIndex + 1);
        }

        StartCoroutine(NextCharacterRoutine(true)); 
    }

    private IEnumerator NextCharacterRoutine(bool isFirstCharacterOfDay = false) 
    {
        if (!isFirstCharacterOfDay && currentDayConfig.delayBetweenCharacters > 0f) 
        {
            yield return new WaitForSeconds(currentDayConfig.delayBetweenCharacters);
        }

        if (currentCharacter != null)
        {
            Destroy(currentCharacter.gameObject);
            currentCharacter = null; 
        }

        if (charactersProcessedToday < currentDayConfig.numberOfCharacters)
        {
            SpawnNewCharacter();
        }
        else
        {
            Debug.Log($"Dia {currentDayIndex + 1} finalizado.");
            StartCoroutine(DayTransitionRoutine()); 
        }

        isTransitioning = false;
    }

    public void SpawnNewCharacter()
    {
        if (characterPrefab == null || characterSpawnPoint == null) return;

        GameObject newCharObject = Instantiate(characterPrefab, characterSpawnPoint.position, Quaternion.identity);
        currentCharacter = newCharObject.GetComponent<CharacterController>();

        // Gera o passaporte para este novo personagem
        if (passportController != null)
        {
            passportController.ClosePassport(); // Fecha visualmente para reiniciar
            passportController.GenerateNewStudent(); // Cria os dados e o carimbo
        }

        if (countdownHUD != null)
        {
            countdownHUD.ResetTimer();
            countdownHUD.StartTimer();
        }

        charactersProcessedToday++; 
    }

    // --- LÓGICA DE DECISÃO E PONTUAÇÃO ---

    void AoAceitar()
    {
        if (isTransitioning) return;

        // Se o passaporte é VÁLIDO e o jogador aceitou -> PONTO
        if (passportController != null && passportController.IsCurrentPassportValid)
        {
            pontuacao++;
            Debug.Log("ACERTOU: Aceitou um passaporte válido.");
        }
        else
        {
            Debug.Log("ERROU: Aceitou um passaporte inválido.");
            // Lógica opcional: tirar pontos ou vidas
        }

        AtualizarPontuacao();
        ProcessDecision(true); // Aprova o personagem visualmente
    }

    void AoNegar()
    {
        if (isTransitioning) return;

        // Se o passaporte é INVÁLIDO e o jogador negou -> PONTO
        if (passportController != null && !passportController.IsCurrentPassportValid)
        {
            pontuacao++;
            Debug.Log("ACERTOU: Negou um passaporte inválido.");
        }
        else
        {
            Debug.Log("ERROU: Negou um passaporte válido.");
        }

        AtualizarPontuacao();
        ProcessDecision(false); // Reprova o personagem visualmente
    }

    void AoPassar()
    {
        if (isTransitioning) return;

        Debug.Log("PASSOU: Nenhum ponto ganho.");
        // Considera como recusa/saída sem pontuar
        ProcessDecision(false); 
    }

    void AtualizarPontuacao()
    {
        if (pontuacaoText != null)
        {
            pontuacaoText.text = "Points: " + pontuacao;
        }
    }

    private void ProcessDecision(bool wasApproved)
    {
        if (isTransitioning) return;
        isTransitioning = true; 

        if (currentCharacter == null) 
        {
            isTransitioning = false; 
            return;
        }

        if (countdownHUD != null) countdownHUD.PauseTimer();
        
        currentCharacter.StartExitSequence(wasApproved);

        StartCoroutine(NextCharacterRoutine());
    }

    // --- Transição de Dia ---
    private IEnumerator DayTransitionRoutine()
    {
        Debug.Log("Iniciando transição para o próximo dia...");

        if (transitionScreenPrefab != null)
        {
            GameObject transitionScreenInstance = Instantiate(transitionScreenPrefab, Vector3.zero, Quaternion.identity);
            
            // CORREÇÃO: Usando FindObjectOfType (compatível com Unity antigo)
            Canvas parentCanvas = FindObjectOfType<Canvas>(); 
            
            if (parentCanvas != null) 
            {
                transitionScreenInstance.transform.SetParent(parentCanvas.transform, false); 
                RectTransform rt = transitionScreenInstance.GetComponent<RectTransform>(); 
                if (rt != null) 
                {
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                    rt.sizeDelta = Vector2.zero;
                    rt.anchoredPosition = Vector2.zero;
                }
            }

            // Tenta configurar o script da tela de transição se ele existir
            DayTransitionUI transitionUI = transitionScreenInstance.GetComponent<DayTransitionUI>(); 
            if (transitionUI != null) 
            {
                transitionUI.SetupTransition(currentDayIndex + 1, currentDayIndex + 2); 
                yield return new WaitForSeconds(transitionUI.displayDuration); 
            }
            else
            {
                yield return new WaitForSeconds(3f); 
            }

            if (transitionScreenInstance != null) Destroy(transitionScreenInstance);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        StartNewDay(currentDayIndex + 1);
    }

    // --- Métodos Públicos para UI externa (caso precise) ---
    public void HandleTimeOut()
    {
        Debug.Log("Tempo expirou!");
        // Timeout conta como negar, mas sem ganhar ponto (ou pode tirar ponto se preferir)
        ProcessDecision(false);
    }

    // Mantidos para compatibilidade caso algum botão antigo ainda chame estes métodos diretamente
    public void AcceptDocument() => AoAceitar();
    public void RefuseDocument() => AoNegar();
}