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
    public Image passportStampImage;

    //Gabarito
    [Header("Objetos do Gabarito")]
    public GameObject gabaritoUIObject; 

    [Header("Campos do Gabarito")]
    public TextMeshProUGUI gabaritoCountryText;
    public TextMeshProUGUI gabaritoNameText;
    public TextMeshProUGUI gabaritoDobText;
    public Image gabaritoPhotoImage;

    [Header("Dados para Geração")]
    private string[] countryNames = { "Arstotzka", "Kolechia", "Obristan", "United Federation", "Republic of Antegria" };
    private string[] firstNames = { "Dimitri", "Jian", "Elena", "Mikhail", "Sofia" };
    private string[] lastNames = { "Petrov", "Li", "Ivanov", "Chen", "Volkov" };

    public Sprite[] photoPool;
    public Sprite[] stampPool;

    void Start()
    {
        passportClosedObject.GetComponent<Button>().onClick.AddListener(OpenPassport);

        gabaritoUIObject.SetActive(false);

        GenerateNewStudent();
    }

    public void GenerateNewStudent()
    {
        string trueFirstName = firstNames[Random.Range(0, firstNames.Length)];
        string trueLastName = lastNames[Random.Range(0, lastNames.Length)];
        string trueCountry = countryNames[Random.Range(0, countryNames.Length)];
        int trueYear = Random.Range(1995, 2007);
        int trueMonth = Random.Range(1, 13);
        int trueDay = Random.Range(1, 29);
        Sprite truePhoto = photoPool[Random.Range(0, photoPool.Length)];
        
        gabaritoNameText.text = "Nome: " + trueLastName + ", " + trueFirstName;
        gabaritoDobText.text = "Nasc: " + trueDay.ToString("D2") + "/" + trueMonth.ToString("D2") + "/" + trueYear;
        gabaritoCountryText.text = "País: " + trueCountry;
        gabaritoPhotoImage.sprite = truePhoto;

        string passportFirstName = trueFirstName;
        string passportLastName = trueLastName;
        string passportCountry = trueCountry;
        string passportDob = trueDay.ToString("D2") + "/" + trueMonth.ToString("D2") + "/" + trueYear;
        Sprite passportPhoto = truePhoto;

        if (Random.value < 0.15f) // Chance de 15%
        {
            passportFirstName = firstNames[Random.Range(0, firstNames.Length)];
            Debug.Log("FALSIFICAÇÃO GERADA: Nome incorreto.");
        }
        if (Random.value < 0.15f) // Chance de 15%
        {
            passportCountry = countryNames[Random.Range(0, countryNames.Length)];
            Debug.Log("FALSIFICAÇÃO GERADA: País incorreto.");
        }
        if (Random.value < 0.15f) // Chance de 15%
        {
            passportDob = (trueDay).ToString("D2") + "/" + (trueMonth).ToString("D2") + "/" + (trueYear - 1);
            Debug.Log("FALSIFICAÇÃO GERADA: Data de Nascimento incorreta.");
        }
        // if (Random.value < 0.15f) // Chance de 15%
        // {
        //     passportPhoto = photoPool[Random.Range(0, photoPool.Length)];
        //     Debug.Log("FALSIFICAÇÃO GERADA: Foto incorreta.");
        // }

        passportNameText.text = passportLastName + ", " + passportFirstName;
        passportCountryText.text = passportCountry;
        passportDobText.text = passportDob;
        passportPhotoImage.sprite = passportPhoto;
    }

    public void ToggleGabarito()
    {
        gabaritoUIObject.SetActive(!gabaritoUIObject.activeSelf);
    }

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