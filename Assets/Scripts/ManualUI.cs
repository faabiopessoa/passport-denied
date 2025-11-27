using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ManualUI : MonoBehaviour
{
    [Header("Referências do UI")]
    public GameObject manualPanel;
    public CanvasGroup canvasGroup; // controla o fade do livro

    [Header("Slots (4 por folha)")]
    public Image slot1Image;
    public Image slot2Image;
    public Image slot3Image;
    public Image slot4Image;

    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;
    public TextMeshProUGUI slot4Text;

    [Header("Navegação (sem fade)")]
    public Button nextButton;
    public Button prevButton;
    public TextMeshProUGUI pageLabel;

    [Header("Sprites dos Selos (ordem fixa)")]
    public Sprite vastanSprite;
    public Sprite belgraviaSprite;
    public Sprite zharimSprite;
    public Sprite sanIberoSprite;
    public Sprite norhalmSprite;
    public Sprite ostyrraSprite;

    [Header("Config de Fade")]
    public float bookFadeDuration = 0.35f;

    private RuleManager ruleManager;
    private int currentPage = 0;
    private int totalPages = 0;
    private const int slotsPerPage = 4;

    private List<SealRule> orderedRules = new List<SealRule>();
    private List<Sprite> orderedSprites = new List<Sprite>();
    private bool isFading = false;

    void Start()
    {
        // painel fica ativo, mas invisível e sem interação
        if (manualPanel != null) manualPanel.SetActive(true);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        ruleManager = FindFirstObjectByType<RuleManager>();

        // listeners de navegação (sem fade)
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (prevButton != null) prevButton.onClick.AddListener(PrevPage);

        // ordem fixa dos selos
        orderedSprites = new List<Sprite>
        {
            vastanSprite, belgraviaSprite, zharimSprite,
            sanIberoSprite, norhalmSprite, ostyrraSprite
        };
    }

    // Chamado pelo botão REGULAMENTO
    public void ToggleManual()
    {
        if (isFading) return;

        bool isOpen = canvasGroup != null && canvasGroup.alpha > 0.01f;
        if (isOpen)
        {
            StartCoroutine(FadeBook(false));
        }
        else
        {
            currentPage = 0;
            UpdateManual();
            StartCoroutine(FadeBook(true));
        }
    }

    // Chamado pelo botão X
    public void CloseManual()
    {
        if (!isFading)
            StartCoroutine(FadeBook(false));
    }

    // Fade só do livro (abrir/fechar)
    private IEnumerator FadeBook(bool opening)
    {
        isFading = true;

        if (canvasGroup == null)
        {
            // fallback sem fade
            manualPanel.SetActive(opening);
            isFading = false;
            yield break;
        }

        float start = canvasGroup.alpha;
        float end = opening ? 1f : 0f;
        float elapsed = 0f;

        if (opening)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while (elapsed < bookFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / bookFadeDuration);
            canvasGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        canvasGroup.alpha = end;

        if (!opening)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        isFading = false;
    }

    // Atualiza a folha (4 slots). Sem fade entre páginas.
    public void UpdateManual()
    {
        if (ruleManager == null || ruleManager.currentRules == null || ruleManager.currentRules.Count == 0)
            return;

        orderedRules = ruleManager.currentRules;
        totalPages = Mathf.CeilToInt(orderedRules.Count / (float)slotsPerPage);

        int startIndex = currentPage * slotsPerPage;
        int endIndex = Mathf.Min(startIndex + slotsPerPage, orderedRules.Count);

        ClearSlots();

        var slotImages = new Image[] { slot1Image, slot2Image, slot3Image, slot4Image };
        var slotTexts = new TextMeshProUGUI[] { slot1Text, slot2Text, slot3Text, slot4Text };

        // esconde tudo
        for (int i = 0; i < 4; i++)
        {
            if (slotImages[i]) slotImages[i].gameObject.SetActive(false);
            if (slotTexts[i]) slotTexts[i].gameObject.SetActive(false);
        }

        int slotIndex = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            var rule = orderedRules[i];
            var sprite = orderedSprites[i];

            if (slotImages[slotIndex])
            {
                slotImages[slotIndex].sprite = sprite;
                slotImages[slotIndex].gameObject.SetActive(true);
            }

            if (slotTexts[slotIndex])
            {
                slotTexts[slotIndex].text = rule.isApproved ? "APROVADO" : "NEGADO";
                slotTexts[slotIndex].color = rule.isApproved ? new Color(0f, 1f, 0f) : new Color(1f, 0f, 0f);
                slotTexts[slotIndex].gameObject.SetActive(true);
            }

            slotIndex++;
        }

        if (pageLabel != null)
            pageLabel.text = $"Página {currentPage + 1}/{totalPages}";

        if (prevButton) prevButton.interactable = currentPage > 0;
        if (nextButton) nextButton.interactable = currentPage < totalPages - 1;
    }

    private void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            UpdateManual();
        }
    }

    private void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateManual();
        }
    }

    private void ClearSlots()
    {
        if (slot1Image) slot1Image.sprite = null;
        if (slot2Image) slot2Image.sprite = null;
        if (slot3Image) slot3Image.sprite = null;
        if (slot4Image) slot4Image.sprite = null;

        if (slot1Text) slot1Text.text = "";
        if (slot2Text) slot2Text.text = "";
        if (slot3Text) slot3Text.text = "";
        if (slot4Text) slot4Text.text = "";
    }
}
