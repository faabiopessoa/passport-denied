using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassportController : MonoBehaviour
{
    [Header("Objetos do Passaporte")]
    public GameObject passportClosedObject;
    public GameObject passportOpenObject;

    [Header("Campos do Passaporte (Aberto)")]
    public TextMeshProUGUI passportCountryText; 
    public TextMeshProUGUI passportNameText;    
    public TextMeshProUGUI passportDobText;     
    public Image passportPhotoImage;          
    public Image passportStampImage; // A imagem onde o carimbo aparece

    [Header("Gabarito (A Verdade)")]
    public GameObject gabaritoUIObject; 
    public TextMeshProUGUI gabaritoCountryText;
    public TextMeshProUGUI gabaritoNameText;
    public TextMeshProUGUI gabaritoDobText;
    public Image gabaritoPhotoImage;

    [Header("Dados")]
    public Sprite[] photoPool;
    
    [Header("Áudio")] // --- NOVO ---
    public AudioSource audioSource; // --- NOVO ---
    public AudioClip passportSound; // --- NOVO: Som de abrir/fechar passaporte

    private string[] firstNames = { "Dimitri", "Jian", "Elena", "Mikhail", "Sofia", "Lukas", "Ana", "Viktor" };
    private string[] lastNames = { "Petrov", "Li", "Ivanov", "Chen", "Volkov", "Silva", "Kozlov", "Muller" };

    public bool IsCurrentPassportValid { get; private set; }

    void Start()
    {
        if(passportClosedObject.GetComponent<Button>() != null)
            passportClosedObject.GetComponent<Button>().onClick.AddListener(OpenPassport);

        gabaritoUIObject.SetActive(false);
        
        // Garante que o selo comece invisível até gerar dados
        if(passportStampImage != null) passportStampImage.gameObject.SetActive(false);

        GenerateNewStudent();
    }

    public void GenerateNewStudent()
    {
        if (RuleManager.Instance == null)
        {
            Debug.LogError("ERRO CRÍTICO: RuleManager não encontrado na cena!");
            return;
        }

        var rules = RuleManager.Instance.currentRules;
        if(rules.Count == 0) return;

        // --- 1. GERAR DADOS REAIS (Gabarito) ---
        string trueFirstName = firstNames[Random.Range(0, firstNames.Length)];
        string trueLastName = lastNames[Random.Range(0, lastNames.Length)];
        string trueCountry = rules[Random.Range(0, rules.Count)].countryName;
        
        int trueYear = Random.Range(1995, 2007);
        int trueMonth = Random.Range(1, 13);
        int trueDay = Random.Range(1, 29);
        Sprite truePhoto = photoPool[Random.Range(0, photoPool.Length)];

        // Preenche UI Gabarito
        if(gabaritoNameText) gabaritoNameText.text = "Nome: " + trueLastName + ", " + trueFirstName;
        if(gabaritoDobText) gabaritoDobText.text = "Nasc: " + trueDay.ToString("D2") + "/" + trueMonth.ToString("D2") + "/" + trueYear;
        if(gabaritoCountryText) gabaritoCountryText.text = "País: " + trueCountry;
        if(gabaritoPhotoImage) gabaritoPhotoImage.sprite = truePhoto;

        // --- 2. GERAR DADOS DO PASSAPORTE ---
        string passFirstName = trueFirstName;
        string passLastName = trueLastName;
        string passCountry = trueCountry;
        string passDob = trueDay.ToString("D2") + "/" + trueMonth.ToString("D2") + "/" + trueYear;
        Sprite passPhoto = truePhoto;

        bool hasDataError = false; 

        // Sorteio de erros (15% chance cada)
        if (Random.value < 0.15f) 
        {
            passFirstName = firstNames[Random.Range(0, firstNames.Length)];
            hasDataError = true;
        }
        else if (Random.value < 0.15f) 
        {
            passCountry = rules[Random.Range(0, rules.Count)].countryName;
            if (passCountry != trueCountry) hasDataError = true;
        }
        else if (Random.value < 0.15f) 
        {
            passDob = trueDay.ToString("D2") + "/" + trueMonth.ToString("D2") + "/" + (trueYear - 5);
            hasDataError = true;
        }

        // --- 3. ATUALIZAR UI PASSAPORTE ---
        if(passportNameText) passportNameText.text = passLastName + ", " + passFirstName;
        if(passportCountryText) passportCountryText.text = passCountry;
        if(passportDobText) passportDobText.text = passDob;
        if(passportPhotoImage) passportPhotoImage.sprite = passPhoto;

        // --- LÓGICA DO SELO (Debugada) ---
        if (passportStampImage != null)
        {
            // Busca o sprite no RuleManager
            Sprite stamp = RuleManager.Instance.GetSealSprite(passCountry);

            if (stamp != null)
            {
                passportStampImage.sprite = stamp;
                passportStampImage.color = Color.white; // FORÇA A COR BRANCA (CASO ESTEJA TRANSPARENTE)
                passportStampImage.preserveAspect = true; // EVITA DISTORÇÃO
                passportStampImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"AVISO: O Sprite do selo para '{passCountry}' retornou NULL. Verifique o RuleManager.");
                passportStampImage.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("ERRO: O campo 'Passport Stamp Image' não está linkado no Inspector!");
        }

        // --- 4. VERIFICAÇÃO FINAL ---
        bool isCountryApproved = RuleManager.Instance.IsCountryApproved(passCountry);

        if (!hasDataError && isCountryApproved) IsCurrentPassportValid = true;
        else IsCurrentPassportValid = false;
        
        Debug.Log($"Passaporte Gerado: {passCountry} | ErroDados: {hasDataError} | Aprovado: {isCountryApproved} -> Válido: {IsCurrentPassportValid}");
    }

    public void ToggleGabarito() 
    {
        if(gabaritoUIObject) gabaritoUIObject.SetActive(!gabaritoUIObject.activeSelf);
    }
    
    public void OpenPassport()
    {
        if(passportClosedObject) passportClosedObject.SetActive(false); 
        if(passportOpenObject) passportOpenObject.SetActive(true);

        // --- NOVO: Toca som ao abrir ---
        if (audioSource != null && passportSound != null)
            audioSource.PlayOneShot(passportSound);
    }

    public void ClosePassport()
    {
        if(passportClosedObject) passportClosedObject.SetActive(true);   
        if(passportOpenObject) passportOpenObject.SetActive(false);

        // --- NOVO: Toca som ao fechar ---
        if (audioSource != null && passportSound != null)
            audioSource.PlayOneShot(passportSound);
    }
}