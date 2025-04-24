using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Thêm để quản lý scene

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance;
    public TextMeshProUGUI statusText;  // Trạng thái kết nối
    public GameObject createRoomButton; // Nút tạo phòng
    public GameObject joinRoomButton;   // Nút tham gia phòng
    public GameObject[] cardImages; // Mảng chứa các hình ảnh của lá bài
    public GameObject gameStatusText; // Trạng thái game sau khi vào phòng

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        statusText.text = "Đang kết nối với Photon...";

        // Ẩn các lá bài ngay khi vào game
        HideCardImages();
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Đã kết nối Photon!";
        createRoomButton.SetActive(true);
        joinRoomButton.SetActive(true);
    }

    public void CreateRoom()
    {
        statusText.text = "Đang tạo phòng...";
        PhotonNetwork.CreateRoom("GameRoom", new RoomOptions { MaxPlayers = 2 });
    }

    public void JoinRoom()
    {
        statusText.text = "Đang tham gia phòng...";
        PhotonNetwork.JoinOrCreateRoom("GameRoom", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Đã vào phòng!";

        Debug.Log("Số người chơi trong phòng: " + PhotonNetwork.CurrentRoom.PlayerCount);

        // Ẩn các nút Join và Create sau khi vào phòng
        joinRoomButton.SetActive(false);
        createRoomButton.SetActive(false);

        // Hiển thị trạng thái game
        gameStatusText.SetActive(true);
        gameStatusText.GetComponent<TextMeshProUGUI>().text = "Game Status: Choose a card to start!";

        // Hiển thị các lá bài sau khi tạo phòng
        ShowCardImages();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            statusText.text = "Game is starting...";  // Cập nhật UI khi phòng đủ 2 người chơi
        }
        else
        {
            // Nếu chỉ có 1 người, ta để trạng thái "Waiting"
            if (PhotonNetwork.IsMasterClient)
            {
                statusText.text = "Waiting for Player 2...";
            }
            else
            {
                statusText.text = "Waiting for Player 1...";
            }
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Tham gia thất bại, tạo phòng mới...";
        PhotonNetwork.CreateRoom("GameRoom", new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnCreatedRoom()
    {
        statusText.text = "Tạo phòng thành công!";
        PhotonNetwork.LoadLevel("MultiplayerScene");

        // Ẩn các nút "Join" và "Create" sau khi tạo phòng thành công
        createRoomButton.SetActive(false);
        joinRoomButton.SetActive(false);
    }

    // Hàm để ẩn các lá bài khi vừa vào game
    private void HideCardImages()
    {
        foreach (GameObject card in cardImages)
        {
            card.SetActive(false);
        }
    }

    // Hàm để hiển thị các lá bài khi tạo phòng
    private void ShowCardImages()
    {
        foreach (GameObject card in cardImages)
        {
            card.SetActive(true);
        }
    }

    // Hàm để dừng game và hiển thị panel kết thúc game
    public void EndGame(string result)
    {
        // Hiển thị kết quả game
        gameStatusText.GetComponent<TextMeshProUGUI>().text = result;

        // Hiển thị panel kết thúc game
        gameStatusText.SetActive(true);
        // Ẩn các lá bài khi game kết thúc
        HideCardImages();
    }

    // Hàm quay lại Main Menu khi bấm nút
    public void GoToMainMenu()
    {
        PhotonNetwork.LoadLevel("MainMenu");  // Quay lại Main Menu
    }
}
