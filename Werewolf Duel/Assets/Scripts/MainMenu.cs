using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    // Các Panel UI trong MainMenu
    public GameObject aiModePanel;
    public GameObject onlineModePanel;
    public GameObject difficultyPanel;

    // Hàm gọi khi người chơi chọn chế độ AI và độ khó
    public void PlayAI(int difficulty)
    {
        // Lưu độ khó AI vào PlayerPrefs và chuyển sang Scene game
        PlayerPrefs.SetInt("AIDifficulty", difficulty);  // 1: Easy, 2: Medium, 3: Hard
        PlayerPrefs.SetString("GameMode", "AI");  // Lưu chế độ AI
        SceneManager.LoadScene("GameScene");  // Chuyển sang Scene Game với chế độ AI
    }

    // Hàm gọi khi người chơi chọn chế độ Online
    public void PlayOnline()
    {
        // Lưu chế độ Online và chuyển sang GameScene
        PlayerPrefs.SetString("GameMode", "Online");
        SceneManager.LoadScene("MultiplayerScene");  // Chuyển sang Scene Multiplayer với Online
    }

    // Hiện panel AI và ẩn các panel khác
    public void ShowAIModePanel()
    {
        aiModePanel.SetActive(true);
        onlineModePanel.SetActive(false);
        difficultyPanel.SetActive(true);  // Ẩn các nút độ khó khi chưa chọn AI
    }

    // Hiện panel Online và ẩn các panel khác
    public void ShowOnlineModePanel()
    {
        aiModePanel.SetActive(false);
        onlineModePanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    // Hàm này để thoát game
    public void ExitGame()
    {
        Application.Quit();  // Thoát khỏi game
    }
}
