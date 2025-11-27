using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SealRule
{
    public string countryName;
    public bool isApproved;
}

public class RuleManager : MonoBehaviour
{
    public static RuleManager Instance;

    [Header("Configuração de Regras")]
    public List<SealRule> currentRules = new List<SealRule>();
    public int currentDay = 1;
    private int lastGeneratedDay = -1;

    // A ordem aqui deve ser a mesma da lista sealSprites abaixo
    public string[] availableCountries = { "Vastan", "Belgravia", "Zharim", "San Ibero", "Norhalm", "Ostyrra" };

    [Header("Assets Visuais")]
    [Tooltip("Arraste os sprites dos carimbos aqui na mesma ordem dos nomes acima (Vastan, Belgravia...)")]
    public List<Sprite> sealSprites; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GenerateRules(); 
    }

    public void OnDayChanged(int newDay)
    {
        currentDay = newDay;
        if (currentDay != lastGeneratedDay)
        {
            GenerateRules();
            lastGeneratedDay = currentDay;
        }
    }

    public void GenerateRules()
    {
        currentRules.Clear();
        int totalApproved = 0;

        foreach (string c in availableCountries)
        {
            bool approved = Random.value > 0.5f;
            if (approved) totalApproved++;

            currentRules.Add(new SealRule { countryName = c, isApproved = approved });
        }

        if (totalApproved < 3)
        {
            // Embaralha para garantir aleatoriedade na correção
            for (int i = currentRules.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = currentRules[i];
                currentRules[i] = currentRules[j];
                currentRules[j] = temp;
            }

            // Força aprovação até ter 3
            int approvedCount = currentRules.FindAll(x => x.isApproved).Count;
            for (int i = 0; i < currentRules.Count; i++)
            {
                if (approvedCount >= 3) break;
                if (!currentRules[i].isApproved)
                {
                    currentRules[i].isApproved = true;
                    approvedCount++;
                }
            }
        }
    }

    // --- Métodos de Validação e Busca ---

    public bool IsCountryApproved(string countryName)
    {
        foreach (var rule in currentRules)
        {
            if (rule.countryName == countryName) return rule.isApproved;
        }
        return false;
    }

    public Sprite GetSealSprite(string countryName)
    {
        for (int i = 0; i < availableCountries.Length; i++)
        {
            if (availableCountries[i] == countryName)
            {
                if (i < sealSprites.Count) return sealSprites[i];
            }
        }
        return null; // Retorna null se não achar
    }
}