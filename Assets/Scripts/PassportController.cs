using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PassportController : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject passportClosedObject;
    public GameObject passportOpenObject;

    [Header("Open Passport Fields")]
    public TextMeshProUGUI countryText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dobText;
    public Image photoImage;
    public Image stampImage;

    [Header("Data Pools")]
    private string[] countryNames = { "Arstotzka", "Kolechia", "Obristan", "United Federation", "Republic of Antegria" };
    private string[] firstNames = { "Dimitri", "Jian", "Elena", "Mikhail", "Sofia" };
    private string[] lastNames = { "Petrov", "Li", "Ivanov", "Chen", "Volkov" };

    public Sprite[] photoPool;
    public Sprite[] stampPool;

    void Start()
    {
        passportClosedObject.GetComponent<Button>().onClick.AddListener(OpenPassport);

        GenerateNewPassport();
    }

    public void GenerateNewPassport()
    {
        string country = countryNames[Random.Range(0, countryNames.Length)];
        string firstName = firstNames[Random.Range(0, firstNames.Length)];
        string lastName = lastNames[Random.Range(0, lastNames.Length)];
        
        int year = Random.Range(1995, 2007);
        int month = Random.Range(1, 13);
        int day = Random.Range(1, 29);

        countryText.text = country;
        nameText.text = lastName + ", " + firstName;
        dobText.text = day.ToString("D2") + "/" + month.ToString("D2") + "/" + year;

        photoImage.sprite = photoPool[Random.Range(0, photoPool.Length)];

        // bool hasStamp = Random.Range(0, 2) == 0;
        // if (hasStamp)
        // {
        //     stampImage.gameObject.SetActive(true); // Show the stamp
        //     stampImage.sprite = stampPool[Random.Range(0, stampPool.Length)];
        // }
        // else
        // {
        //     stampImage.gameObject.SetActive(false); // Hide the stamp
        // }
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