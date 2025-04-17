using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance;
    public TextMeshProUGUI statusText;   // Hiển thị trạng thái kết nối
    public GameObject createRoomButton;  // Nút Create Room
    public GameObject joinRoomButton;    // Nút Join Room

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
        PhotonNetwork.JoinRoom("GameRoom");
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Đã vào phòng!";
        PhotonNetwork.LoadLevel("MultiplayerScene");
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
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Tạo phòng thất bại!";
    }
}
