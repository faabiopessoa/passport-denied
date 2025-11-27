using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDayConfig", menuName = "Game Config/Day Configuration")]
public class DayConfig : ScriptableObject
{
    [Tooltip("O número de personagens que o jogador deve processar neste dia.")]
    public int numberOfCharacters = 5;

    [Tooltip("A duração total do timer para cada personagem neste dia (em segundos).")]
    public float characterServiceDuration = 60f;

    [Tooltip("O tempo de atraso entre um personagem sair e o próximo surgir.")]
    public float delayBetweenCharacters = 4f;
}