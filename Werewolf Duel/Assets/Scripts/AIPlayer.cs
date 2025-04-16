using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : MonoBehaviour
{
    public static AIPlayer instance;  // Instance static của AI

    private List<string> aiCards = new List<string> { "Werewolf", "Seer", "Hunter", "Villager", "Villager", "Villager" };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Lấy một thẻ ngẫu nhiên từ bộ bài của AI
    public string GetRandomCard()
    {
        int index = Random.Range(0, aiCards.Count);
        return aiCards[index];
    }
}
