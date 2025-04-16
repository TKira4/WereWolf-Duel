using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI resultText;

    private List<string> playerCards = new List<string> { "Werewolf", "Seer", "Hunter", "Villager", "Villager", "Villager" };
    private List<string> aiCards = new List<string> { "Werewolf", "Seer", "Hunter", "Villager", "Villager", "Villager" };

    private bool waitingForPlayer = true;
    private int aiDifficulty = 2; // Mặc định là Medium

    public GameObject resultPanel;
    public GameObject mainMenuButton;
    public GameObject restartButton;

    void Awake()
    {
        instance = this;
        aiDifficulty = PlayerPrefs.GetInt("AIDifficulty", 2);  // Lấy độ khó từ PlayerPrefs
    }

    // Hàm xử lý khi người chơi chọn thẻ
    public void PlayerChoseCard(string playerCard)
    {
        if (!waitingForPlayer) return;

        waitingForPlayer = false;

        playerCards.Remove(playerCard);  // Loại bỏ lá bài người chơi đã chọn

        string aiCard = ChooseAICard();  // AI chọn bài ngẫu nhiên
        aiCards.Remove(aiCard);

        Debug.Log("Người chơi chọn: " + playerCard + " | AI chọn: " + aiCard);
        string outcome = ResolveTurn(playerCard, aiCard);  // Lấy kết quả trận đấu

        resultText.text = "Bạn chọn: " + playerCard + " | AI chọn: " + aiCard + "\n" + outcome;

        // Nếu kết quả là hòa, chơi tiếp, còn nếu thắng hoặc thua thì kết thúc trò chơi
        if (outcome.Contains("BẠN THẮNG!") || outcome.Contains("BẠN THUA!"))
        {
            // Kết thúc trận đấu
            EndGame(outcome);
            ShowEndGameUI(outcome);
        }
        else
        {
            // Nếu hòa, cho phép chơi tiếp
            Invoke("ResetTurn", 2f);
        }
    }

    public void EndGame(string result)
    {
        // Hiển thị kết quả cuối cùng
        resultText.text = result + "\nTrận đấu kết thúc!";

        // Bạn có thể thêm nút "Quay lại Menu chính" hoặc "Thoát" ở đây
        // Application.Quit(); // Nếu bạn muốn thoát game, hoặc chuyển về Main Menu
    }

    // AI chọn 1 lá bài ngẫu nhiên với độ khó
    string ChooseAICard()
    {
        int i;
        if (aiDifficulty == 1) // Easy: AI chọn ngẫu nhiên
        {
            i = Random.Range(0, aiCards.Count);
        }
        else if (aiDifficulty == 2) // Medium: AI chọn ngẫu nhiên nhưng ưu tiên thẻ mạnh hơn
        {
            i = Random.Range(0, aiCards.Count); // Tùy chỉnh logic này
        }
        else // Hard: AI chọn thẻ thông minh hơn
        {
            i = Random.Range(0, aiCards.Count); // AI có thể dựa vào chiến lược
        }
        return aiCards[i];
    }

    // Xử lý kết quả theo luật
    string ResolveTurn(string player, string ai)
    {
        if (player == "Werewolf" && ai == "Villager")
            return "Bạn là Sói, AI là Dân – BẠN THẮNG!";
        if (ai == "Werewolf" && player == "Villager")
            return "Bạn là Dân, AI là Sói – BẠN THUA!";

        if (player == "Seer" && ai == "Werewolf")
            return "Bạn dùng Tiên tri đoán trúng Sói – BẠN THẮNG!";
        if (ai == "Seer" && player == "Werewolf")
            return "AI dùng Tiên tri đoán trúng Sói – BẠN THUA!";

        if (player == "Hunter" && ai == "Villager")
            return "Bạn là Thợ săn, AI là Dân – BẠN THẮNG! Và AI mất 1 lá!";
        if (ai == "Hunter" && player == "Villager")
            return "AI là Thợ săn, bạn là Dân – BẠN THUA! Và bạn mất 1 lá!";

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

    void ShowEndGameUI(string outcome)
    {
        // Hiển thị Panel kết thúc game
        resultPanel.SetActive(true);

        // Hiển thị nút "Main Menu" và "Restart"
        if (outcome.Contains("BẠN THẮNG!") || outcome.Contains("BẠN THUA!"))
        {
            // Nếu có người thắng hoặc thua, hiển thị kết thúc
            resultText.text = outcome + "\nTrận đấu kết thúc!";
            mainMenuButton.SetActive(true);  // Quay lại Main Menu
            restartButton.SetActive(false); // Tắt nút Restart
        }
        else
        {
            // Nếu hòa, chỉ có thể chơi lại
            resultText.text = outcome;
            mainMenuButton.SetActive(false);  // Ẩn nút Main Menu
            restartButton.SetActive(true);   // Hiển thị nút Restart
        }
    }

    // Hàm để quay lại Main Menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // Quay lại Main Menu
    }

    // Hàm để restart lại game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Restart Scene hiện tại
    }
}
