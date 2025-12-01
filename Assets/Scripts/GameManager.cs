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
    public PassportController passportController; 
    
    public Button botaoAceitar;
    public Button botaoNegar;
    public Button botaoPassar;
    public TextMeshProUGUI pontuacaoText; 

    [Header("Áudio dos Carimbos")]
    public AudioSource audioSource;
    public AudioClip stampApprovedSound;
    public AudioClip stampDeniedSound;

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

        StartNewDay(0);

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

        RuleManager ruleManager = FindObjectOfType<RuleManager>();
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

        if (passportController != null)
        {
            passportController.ClosePassport();
            passportController.GenerateNewStudent();
        }

        if (countdownHUD != null)
        {
            countdownHUD.ResetTimer();
            countdownHUD.StartTimer();
        }

        charactersProcessedToday++; 
    }

    void AoAceitar()
    {
        if (isTransitioning) return;

        bool acertou = false;

        if (passportController != null && passportController.IsCurrentPassportValid)
        {
            pontuacao++;
            acertou = true;
            Debug.Log("ACERTOU: Aceitou um passaporte válido.");
        }
        else
        {
            acertou = false;
            Debug.Log("ERROU: Aceitou um passaporte inválido.");
        }

        TocarSomFeedback(acertou);
        AtualizarPontuacao();
        ProcessDecision(true);
    }

    void AoNegar()
    {
        if (isTransitioning) return;

        bool acertou = false;

        if (passportController != null && !passportController.IsCurrentPassportValid)
        {
            pontuacao++;
            acertou = true;
            Debug.Log("ACERTOU: Negou um passaporte inválido.");
        }
        else
        {
            acertou = false;
            Debug.Log("ERROU: Negou um passaporte válido.");
        }

        TocarSomFeedback(acertou);
        AtualizarPontuacao();
        ProcessDecision(false);
    }

    void AoPassar()
    {
        if (isTransitioning) return;

        Debug.Log("PASSOU: Nenhum ponto ganho.");
        TocarSomFeedback(false);
        ProcessDecision(false); 
    }

    void TocarSomFeedback(bool acertou)
    {
        Vector3 cameraPos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;

        if (acertou)
        {
            if (stampApprovedSound != null) 
            {
                AudioSource.PlayClipAtPoint(stampApprovedSound, cameraPos, 1.0f); 
            }
        }
        else
        {
            if (stampDeniedSound != null) 
            {
                AudioSource.PlayClipAtPoint(stampDeniedSound, cameraPos, 1.0f);
            }
        }
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

    private IEnumerator DayTransitionRoutine()
    {
        Debug.Log("Iniciando transição para o próximo dia...");

        if (transitionScreenPrefab != null)
        {
            GameObject transitionScreenInstance = Instantiate(transitionScreenPrefab, Vector3.zero, Quaternion.identity);
            
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

    public void HandleTimeOut()
    {
        Debug.Log("Tempo expirou!");
        TocarSomFeedback(false);
        ProcessDecision(false);
    }

    public void AcceptDocument() => AoAceitar();
    public void RefuseDocument() => AoNegar();
}