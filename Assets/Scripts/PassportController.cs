using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassportController : MonoBehaviour
{
    [Header("Objetos do UI")]
    public GameObject passportClosedObject;
    public GameObject passportOpenObject;

    [Header("Campos do Passaporte (Aberto)")]
    public TextMeshProUGUI passportCountryText; 
    public TextMeshProUGUI passportNameText;    
    public TextMeshProUGUI passportDobText;     
    public Image passportPhotoImage;          
    public Image passportStampImage; // Onde vai aparecer o selo

    [Header("Gabarito (A Verdade)")]
    public GameObject gabaritoUIObject; 
    public TextMeshProUGUI gabaritoCountryText;
    public TextMeshProUGUI gabaritoNameText;
    public TextMeshProUGUI gabaritoDobText;
    public Image gabaritoPhotoImage;

    [Header("Banco de Dados (Nomes/Fotos)")]
    public Sprite[] photoPool;
    private string[] firstNames = { "Dimitri", "Jian", "Elena", "Mikhail", "Sofia", "Lukas", "Ana", "Viktor" };
    private string[] lastNames = { "Petrov", "Li", "Ivanov", "Chen", "Volkov", "Silva", "Kozlov" };

    // Variável que guarda se este passaporte é válido ou não
    public bool IsCurrentPassportValid { get; private set; }

    void Start()
    {
        passportClosedObject.GetComponent<Button>().onClick.AddListener(OpenPassport);
        gabaritoUIObject.SetActive(false);
        
        // Gera o primeiro ao iniciar
        GenerateNewStudent();
    }

    public void GenerateNewStudent()
    {
        // Verifica se RuleManager existe
        if (RuleManager.Instance == null)
        {
            Debug.LogError("RuleManager não encontrado na cena!");
            return;
        }

        // --- 1. GERAR A VERDADE (GABARITO) ---
        string trueFirstName = firstNames[Random.Range(0, firstNames.Length)];
        string trueLastName = lastNames[Random.Range(0, lastNames.Length)];
        
        // Pega um país aleatório da lista de regras disponíveis
        var rules = RuleManager.Instance.currentRules;
        string trueCountry = rules[Random.Range(0, rules.Count)].countryName;

        int trueYear = Random.Range(1995, 2007);
        int trueMonth = Random.Range(1, 13);
        int trueDay = Random.Range(1, 29);
        Sprite truePhoto = photoPool[Random.Range(0, photoPool.Length)];
        
        // Preenche UI do Gabarito
        gabaritoNameText.text = "Nome: " + trueLastName + ", " + trueFirstName;
        gabaritoDobText.text = "Nasc: " + trueDay.ToString("D2") + "/" + trueMonth.ToString("D2") + "/" + trueYear;
        gabaritoCountryText.text = "País: " + trueCountry;
        gabaritoPhotoImage.sprite = truePhoto;

        // --- 2. GERAR O PASSAPORTE (COM POSSÍVEIS ERROS) ---
        string passFirstName = trueFirstName;
        string passLastName = trueLastName;
        string passCountry = trueCountry;
        string passDob = trueDay.ToString("D2") + "/" + trueMonth.ToString("D2") + "/" + trueYear;
        Sprite passPhoto = truePhoto;

        bool hasDataError = false; // Erro de digitação (nome, data, país errado)

        // Chance de erro no NOME (15%)
        if (Random.value < 0.15f)
        {
            passFirstName = firstNames[Random.Range(0, firstNames.Length)];
            hasDataError = true;
            Debug.Log("ERRO GERADO: Nome incorreto.");
        }
        // Chance de erro no PAÍS (texto alterado) (15%)
        else if (Random.value < 0.15f)
        {
            // Pega outro país qualquer
            passCountry = rules[Random.Range(0, rules.Count)].countryName;
            if (passCountry != trueCountry) hasDataError = true;
            Debug.Log("ERRO GERADO: País diferente da verdade.");
        }
        // Chance de erro na DATA (15%)
        else if (Random.value < 0.15f)
        {
            passDob = trueDay.ToString("D2") + "/" + trueMonth.ToString("D2") + "/" + (trueYear - 5);
            hasDataError = true;
            Debug.Log("ERRO GERADO: Data de nascimento errada.");
        }

        // --- 3. ATUALIZAR UI DO PASSAPORTE ---
        passportNameText.text = passLastName + ", " + passFirstName;
        passportCountryText.text = passCountry;
        passportDobText.text = passDob;
        passportPhotoImage.sprite = passPhoto;

        // Colocar o Selo (Carimbo) correspondente ao PAÍS ESCRITO no passaporte
        Sprite seal = RuleManager.Instance.GetSealSprite(passCountry);
        if (seal != null)
        {
            passportStampImage.sprite = seal;
            passportStampImage.gameObject.SetActive(true);
        }
        else
        {
            // Se não tiver sprite configurado
            passportStampImage.gameObject.SetActive(false);
        }

        // --- 4. CALCULAR VALIDADE FINAL ---
        // Para ser válido:
        // A) Não pode ter erros de dados (Gabarito == Passaporte)
        // B) O país do passaporte deve estar APROVADO no RuleManager

        bool isRuleApproved = RuleManager.Instance.IsCountryApproved(passCountry);

        if (!hasDataError && isRuleApproved)
        {
            IsCurrentPassportValid = true;
            Debug.Log($"<color=green>PASSAPORTE VÁLIDO: {passCountry} é permitido e dados batem.</color>");
        }
        else
        {
            IsCurrentPassportValid = false;
            string motivo = hasDataError ? "Dados falsos" : "País Negado pelo Regulamento";
            Debug.Log($"<color=red>PASSAPORTE INVÁLIDO: {motivo}.</color>");
        }
    }

    public void ToggleGabarito() => gabaritoUIObject.SetActive(!gabaritoUIObject.activeSelf);

    public void OpenPassport()
    {
        passportClosedObject.SetActive(false); 
        passportOpenObject.SetActive(true);    
    }

    public void ClosePassport()
    {
        passportClosedObject.SetActive(true);   
        passportOpenObject.SetActive(false);  
    }
}