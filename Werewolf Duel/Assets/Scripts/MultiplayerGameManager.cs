using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Thêm để quản lý scene

public class MultiplayerGameManager : MonoBehaviourPun
{
    public TextMeshProUGUI resultText; // Hiển thị kết quả
    public TextMeshProUGUI statusText; // Trạng thái chờ người chơi
    public GameObject[] cardImages; // Mảng chứa các hình ảnh của lá bài
    public GameObject mainMenuButton; // Nút Quay lại Main Menu
    public GameObject gameOverPanel;  // Panel hiển thị khi game kết thúc

    private bool waitingForPlayer = true; // Cờ xác định trạng thái chơi
    private string[] cardNames = { "WereWolf", "Hunter", "Seer", "Villager", "Villager", "Villager" }; // 6 lá bài
    private string player1Choice = "";
    private string player2Choice = "";
    private int player1CardIndex = -1;
    private int player2CardIndex = -1;

    void Start()
    {
        if (statusText == null || resultText == null)
        {
            Debug.LogError("UI Text components are not assigned in the Inspector!");
            return;
        }

        // Set trạng thái chờ cho cả MasterClient và Client
        if (PhotonNetwork.IsMasterClient)
        {
            statusText.text = "Waiting for Player 2...";  // Player 1 (MasterClient)
        }
        else
        {
            statusText.text = "Waiting for Player 1...";  // Player 2
        }
    }

    // Hàm được gọi khi người chơi chọn thẻ
    public void OnCardSelected(int cardIndex)
    {
        Debug.Log("Card selected: " + cardIndex); // Đảm bảo cardIndex được nhận đúng

        // Kiểm tra PhotonView
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not assigned!");
            return;
        }

        // Nếu cả 2 người chơi đã chọn xong, không cập nhật nữa
        if (player1Choice != "" && player2Choice != "")
        {
            return; // Không thực hiện gì nữa khi cả 2 người chơi đã chọn
        }

        // Cập nhật hình ảnh của lá bài đã chọn
        UpdateCardImage(cardIndex);

        if (waitingForPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player1Choice = cardNames[cardIndex]; // Lưu lựa chọn của Player 1
                player1CardIndex = cardIndex; // Lưu index của Player 1
                photonView.RPC(nameof(OnCardChosenRPC), RpcTarget.OthersBuffered, cardIndex, player1Choice);
                photonView.RPC(nameof(UpdateStatusRPC), RpcTarget.OthersBuffered, "Your turn!");
                statusText.text = "Waiting for Player 2...";
                Debug.Log("Gửi RPC tới người chơi khác.");
            }
            else
            {
                player2Choice = cardNames[cardIndex]; // Lưu lựa chọn của Player 2
                player2CardIndex = cardIndex; // Lưu index của Player 2
                photonView.RPC(nameof(OnCardChosenRPC), RpcTarget.MasterClient, cardIndex, player2Choice);
                statusText.text = "Waiting for Player 1...";
                Debug.Log("Gửi RPC tới MasterClient.");
            }
        }
    }

    [PunRPC]
    void OnCardChosenRPC(int cardIndex, string cardName)
    {
        // Lưu lại lựa chọn của người chơi
        if (PhotonNetwork.IsMasterClient)
        {
            player2Choice = cardName; // Lưu lựa chọn của Player 2 khi nhận từ Player 1
            player2CardIndex = cardIndex;
        }
        else
        {
            player1Choice = cardName; // Lưu lựa chọn của Player 1 khi nhận từ Player 2
            player1CardIndex = cardIndex;
        }

        // Kiểm tra nếu cả 2 người chơi đã chọn
        if (player1Choice != "" && player2Choice != "")
        {
            // Hiển thị tên lá bài của cả hai người chơi sau khi cả 2 đã chọn
            resultText.text = $"Player 1 chọn: {player1Choice}\nPlayer 2 chọn: {player2Choice}\n" + CalculateGameResult();

            // Nếu có kết quả, tiếp tục hoặc kết thúc game
            string result = CalculateGameResult();

            // Cập nhật statusText cho cả 2 người chơi (MasterClient và Client)
            photonView.RPC(nameof(UpdateStatusRPC), RpcTarget.AllBuffered, result);
            // Xử lý kết quả
            if (result.Contains("Hòa"))
            {
                // Loại bỏ lá bài đã chọn khỏi deck
                cardNames[player1CardIndex] = "Used";  // Đánh dấu lá bài của Player 1 là đã dùng
                cardNames[player2CardIndex] = "Used";  // Đánh dấu lá bài của Player 2 là đã dùng

                // Cập nhật lại statusText
                statusText.text = "Draw! Choose a new card!";
                // Trì hoãn 2 giây để người chơi chọn lại
                Invoke("AllowNewChoice", 2f);
            }
            else
            {
                // Dừng game nếu có người thắng/thua
                EndGame(result);
            }
        }
    }

    // Cập nhật statusText cho cả hai client
    [PunRPC]
    void UpdateStatusRPC(string result)
    {
        statusText.text = result;
    }

    // Hàm cho phép người chơi chọn lại bài sau 2 giây
    void AllowNewChoice()
    {
        // Reset lại trạng thái và cho phép chọn bài mới
        player1Choice = "";
        player2Choice = "";
        waitingForPlayer = true;

        statusText.text = "Choose a new card!";
    }

    // Hàm dừng game và hiển thị nút thoát về Main Menu
    [PunRPC]
    void UpdateGameOverUI(string result)
    {
        resultText.text = result;
        gameOverPanel.SetActive(true); // Display the game over panel
        mainMenuButton.SetActive(true); // Display the "Main Menu" button
    }

    void EndGame(string result)
    {
        // Notify both players about the end of the game and show the result
        photonView.RPC(nameof(UpdateGameOverUI), RpcTarget.AllBuffered, result);
    }


    // Hàm tính toán kết quả giữa hai người chơi
    private string CalculateGameResult()
    {
        if (player1Choice == "WereWolf" && player2Choice == "Villager")
            return "Player 1 là Sói, Player 2 là Dân – Player 1 thắng!";
        if (player2Choice == "WereWolf" && player1Choice == "Villager")
            return "Player 1 là Dân, Player 2 là Sói – Player 2 thắng!";

        if (player1Choice == "Seer" && player2Choice == "WereWolf")
            return "Player 1 dùng Tiên tri đoán trúng Sói – Player 1 thắng!";
        if (player2Choice == "Seer" && player1Choice == "WereWolf")
            return "Player 1 là Sói, Player 2 dùng Tiên tri đoán trúng – Player 2 thắng!";

        if (player1Choice == "Hunter")
        {
            if (player2Choice == "Villager") return "Player 1 bắn chết Dân – Player 1 thua!";
            else if (player2Choice == "Hunter") return "Cả 2 đều chết - Hòa lượt này";
            else return "Player 1 bắn chết đối phương - Player 1 thắng!";
        }

        if (player2Choice == "Hunter")
        {
            if (player1Choice == "Villager") return "Player 2 bắn chết Dân – Player 2 thắng!";
            else if (player1Choice == "Hunter") return "Cả 2 đều chết - Hòa lượt này";
            else return "Player 2 bắn chết đối phương - Player 2 thắng!";
        }

        return "Hòa lượt này.";
    }

    // Hàm để cập nhật hình ảnh của lá bài khi người chơi chọn
    private void UpdateCardImage(int cardIndex)
    {
        // Loại bỏ lá bài đã chọn khỏi deck
        cardImages[cardIndex].SetActive(false); // Ẩn lá bài đã chọn
    }

    // Hàm quay lại Main Menu khi bấm nút
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // Quay lại Main Menu
    }
}
