using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SealRule
{
    public string countryName;
    public bool isApproved;
}

public class RuleManager : MonoBehaviour
{
    [Header("Configuração de Regras")]
    public List<SealRule> currentRules = new List<SealRule>();
    public int currentDay = 1;
    private int lastGeneratedDay = -1; // 🔹 guarda o último dia que gerou as regras

    private string[] countries = { "Vastan", "Belgravia", "Zharim", "San Ibero", "Norhalm", "Ostyrra" };

    void Start()
    {
        GenerateRules(); // gera apenas no primeiro dia
    }

    public void OnDayChanged(int newDay)
    {
        // Atualiza o dia atual antes de gerar novas regras
        currentDay = newDay;


        // Se for um novo dia (não o mesmo de antes)
        if (currentDay != lastGeneratedDay)
        {
            GenerateRules();
            lastGeneratedDay = currentDay;
        }
    }

    public void GenerateRules()
    {
        currentRules.Clear();

        string[] countries = { "Vastan", "Belgravia", "Zharim", "San Ibero", "Norhalm", "Ostyrra" };
        int totalApproved = 0;

        // 1️⃣ Gera tudo de forma aleatória
        foreach (string c in countries)
        {
            bool approved = Random.value > 0.5f;
            if (approved) totalApproved++;

            currentRules.Add(new SealRule
            {
                countryName = c,
                isApproved = approved
            });
        }

        // 2️⃣ Garante que pelo menos 3 estejam aprovados
        if (totalApproved < 3)
        {
            // Embaralha os países
            for (int i = currentRules.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = currentRules[i];
                currentRules[i] = currentRules[j];
                currentRules[j] = temp;
            }

            // Corrige aprovados até ter 3
            int toApprove = 3 - totalApproved;
            for (int i = 0; i < toApprove; i++)
            {
                currentRules[i].isApproved = true;
            }
        }

    }
}
