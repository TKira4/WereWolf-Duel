using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI resultText;

    private List<string> playerCards = new List<string> { "Werewolf", "Seer", "Villager", "Wizard", "Hunter" };
    private List<string> aiCards = new List<string> { "Werewolf", "Seer", "Villager", "Wizard", "Hunter" };

    private bool waitingForPlayer = true;

    void Awake()
    {
        instance = this;
    }

    public void PlayerChoseCard(string playerCard)
    {
        if (!waitingForPlayer) return;

        waitingForPlayer = false;

        playerCards.Remove(playerCard);  // Loại bỏ lá bài người chơi đã chọn

        string aiCard = ChooseAICard(); // AI chọn bài ngẫu nhiên
        aiCards.Remove(aiCard);

        Debug.Log("Người chơi: " + playerCard + " - AI: " + aiCard);
        string outcome = ResolveTurn(playerCard, aiCard);

        resultText.text = "Bạn chọn: " + playerCard + " | AI: " + aiCard + "\n" + outcome;

        // Đợi vài giây trước khi cho chơi tiếp
        Invoke("ResetTurn", 2f);
    }

    // AI chọn 1 lá bài ngẫu nhiên
    string ChooseAICard()
    {
        int i = Random.Range(0, aiCards.Count);
        return aiCards[i];
    }

    // Xử lý kết quả theo luật
    string ResolveTurn(string player, string ai)
    {
        // Sói vs Dân
        if (player == "Werewolf" && ai == "Villager")
            return "Bạn là Sói, AI là Dân – BẠN THẮNG!";
        if (ai == "Werewolf" && player == "Villager")
            return "Bạn là Dân, AI là Sói – BẠN THUA!";

        // Tiên tri đoán đúng
        if (player == "Seer" && ai == "Werewolf")
            return "Bạn dùng Tiên tri đoán trúng Sói – BẠN THẮNG!";
        if (ai == "Seer" && player == "Werewolf")
            return "AI dùng Tiên tri đoán trúng Sói – BẠN THUA!";

        // Phù thủy cứu mình
        if (player == "Wizard" && ai == "Werewolf")
            return "Bạn là Phù thủy, cứu được mình khỏi Sói – BẠN THẮNG!";
        if (ai == "Wizard" && player == "Werewolf")
            return "AI là Phù thủy, cứu được mình khỏi Sói – BẠN THUA!";

        // Thợ săn kéo theo chết 1 lá của đối phương
        if (player == "Hunter" && ai == "Villager")
            return "Bạn là Thợ săn, AI là Dân – BẠN THẮNG! Và AI mất 1 lá!";
        if (ai == "Hunter" && player == "Villager")
            return "AI là Thợ săn, bạn là Dân – BẠN THUA! Và bạn mất 1 lá!";

        // Kẻ câm vô hiệu hóa Tiên tri
        if (player == "Mute" && ai == "Seer")
            return "Bạn là Kẻ câm, Tiên tri không thể đoán bạn – BẠN THẮNG!";
        if (ai == "Mute" && player == "Seer")
            return "AI là Kẻ câm, Tiên tri không thể đoán AI – BẠN THUA!";

        return "Hòa lượt này.";
    }

    // Reset lại sau mỗi lượt
    void ResetTurn()
    {
        waitingForPlayer = true;
        if (playerCards.Count == 0 || aiCards.Count == 0)
        {
            resultText.text += "\nHết bài – Trận hòa!";
        }
        else
        {
            resultText.text += "\nHãy chọn lượt tiếp theo.";
        }
    }
}
