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

        string aiCard = ChooseAICard();  // AI chọn bài ngẫu nhiên hoặc chiến lược
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
            i = ChooseAICardEasy();
        }
        else if (aiDifficulty == 2) // Medium: AI chọn ngẫu nhiên nhưng ưu tiên thẻ mạnh hơn
        {
            i = ChooseAICardAlphaBeta(); // Minimax hoặc Alpha-Beta
        }
        else // Hard: AI chọn thẻ thông minh hơn
        {
            i = ChooseAICardMCTS(); // Monte Carlo Tree Search hoặc chiến lược khác
        }
        return aiCards[i];
    }

    // AI chọn thẻ với Minimax
    int ChooseAICardMinimax()
    {
        // Implement Minimax hoặc Alpha-Beta Pruning ở đây (đơn giản hoá)
        return Random.Range(0, aiCards.Count);
    }

    // AI chọn thẻ với MCTS
    int ChooseAICardMCTS()
    {
        string bestMove = aiCards[0];
        float bestValue = float.MinValue;

        foreach (var card in aiCards)
        {
            float score = SimulateMCTS(card);  // MCTS sẽ tính toán khả năng chiến thắng của mỗi thẻ
            if (score > bestValue)
            {
                bestValue = score;
                bestMove = card;
            }
        }

        return aiCards.IndexOf(bestMove);  // Trả về chỉ mục của lá bài chọn tốt nhất
    }

    // Mô phỏng MCTS: Mô phỏng nhiều lượt và tính điểm cho mỗi thẻ
    float SimulateMCTS(string card)
    {
        // Giả lập 1000 lần chơi và tính trung bình kết quả
        float totalScore = 0;
        for (int i = 0; i < 1000; i++)
        {
            totalScore += Random.Range(0f, 1f);  // Mô phỏng ngẫu nhiên, có thể thay bằng mô phỏng thực tế
        }

        return totalScore / 1000;  // Điểm trung bình
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

        if(player == "Hunter")
        {
            if (ai == "Villager") return "Bạn bắn chết dân – BẠN THUA!";
            else if (ai == "Hunter") return "Cả 2 đều chết - Hòa lượt này";
            else return "Bạn bắn chết đối phương - BẠN THẮNG!";
        }

        if (ai == "Hunter")
        {
            if (player == "Villager") return "AI bắn chết dân – BẠN THẮNG!";
            else if (player == "Hunter") return "Cả 2 đều chết - Hòa lượt này";
            else return "Bạn bị bắn chết - BẠN THUA!";
        }

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
            restartButton.SetActive(true); // Tắt nút Restart
        }
        else
        {
            // Nếu hòa, chỉ có thể chơi lại
            resultText.text = outcome;
            mainMenuButton.SetActive(true);  // Ẩn nút Main Menu
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

    int ChooseAICardEasy()
    {
        return Random.Range(0, aiCards.Count);  // Trả về chỉ mục của thẻ chọn ngẫu nhiên
    }


    // Tìm nước đi tốt nhất với Alpha-Beta
    int ChooseAICardAlphaBeta()
    {
        string bestMove = aiCards[0];
        int bestValue = int.MinValue;

        // Duyệt qua tất cả các thẻ để tìm nước đi tốt nhất
        foreach (var card in aiCards)
        {
            int score = AlphaBeta(card, int.MinValue, int.MaxValue, 3, true);  // 3 là độ sâu của cây tìm kiếm
            if (score > bestValue)
            {
                bestValue = score;
                bestMove = card;
            }
        }

        return aiCards.IndexOf(bestMove);  // Trả về chỉ mục của lá bài chọn tốt nhất
    }


    // Alpha-Beta Pruning Recursive Function
    int AlphaBeta(string card, int alpha, int beta, int depth, bool isMaximizingPlayer)
    {
        if (depth == 0)
        {
            return EvaluateBoard(card);  // Đánh giá điểm của cây tìm kiếm
        }

        if (isMaximizingPlayer)
        {
            int bestScore = int.MinValue;
            foreach (var nextCard in aiCards)
            {
                int score = AlphaBeta(nextCard, alpha, beta, depth - 1, false);
                bestScore = Mathf.Max(bestScore, score);
                alpha = Mathf.Max(alpha, bestScore);
                if (beta <= alpha)
                {
                    break; // Cắt bớt nhánh không cần thiết
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            foreach (var nextCard in aiCards)
            {
                int score = AlphaBeta(nextCard, alpha, beta, depth - 1, true);
                bestScore = Mathf.Min(bestScore, score);
                beta = Mathf.Min(beta, bestScore);
                if (beta <= alpha)
                {
                    break; // Cắt bớt nhánh không cần thiết
                }
            }
            return bestScore;
        }
    }

    int EvaluateBoard(string card)
    {
        return Random.Range(1, 10);  // Đánh giá ngẫu nhiên, có thể thay bằng logic chiến thuật.
    }
}
