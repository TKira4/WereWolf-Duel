using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public enum GameMode { AI, Online }
    public GameMode currentGameMode;
    public int aiDifficulty;  // Độ khó AI (1: Easy, 2: Medium, 3: Hard)

    void Start()
    {
        // Lấy chế độ chơi và độ khó từ PlayerPrefs
        currentGameMode = (GameMode)System.Enum.Parse(typeof(GameMode), PlayerPrefs.GetString("GameMode", "AI"));
        aiDifficulty = PlayerPrefs.GetInt("AIDifficulty", 2);  // Mặc định là Medium

        // Cấu hình game theo chế độ chơi
        if (currentGameMode == GameMode.AI)
        {
            ConfigureAI(aiDifficulty);
        }
        else if (currentGameMode == GameMode.Online)
        {
            ConfigureOnline();
        }
    }

    void ConfigureAI(int difficulty)
    {
        switch (difficulty)
        {
            case 1: // Easy
                Debug.Log("AI Difficulty: Easy");
                break;
            case 2: // Medium
                Debug.Log("AI Difficulty: Medium");
                break;
            case 3: // Hard
                Debug.Log("AI Difficulty: Hard");
                break;
        }
        // Thực hiện các thay đổi trong gameplay cho AI với độ khó tương ứng
    }

    void ConfigureOnline()
    {
        Debug.Log("Chế độ Online");
        // Cấu hình cho chế độ Online
    }
}
