using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviourPun
{
    [Header("Các lá bài có thể chọn (gán trên prefab)")]
    public GameObject[] playerCards;

    [Header("Các nút UI tương ứng (nếu UI nằm trong prefab)")]
    public Button[] cardButtons;

    private int selectedCardIndex = -1;

    void Start()
    {
        if (!photonView.IsMine) return; // Chỉ thiết lập UI cho local player
        InitializeCardButtons();
    }

    void InitializeCardButtons()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i;
            cardButtons[i].onClick.AddListener(() => OnCardSelected(idx)); // Gán sự kiện click cho các nút
        }
    }

    public void OnCardSelected(int index)
    {
        if (index < 0 || index >= playerCards.Length)
        {
            Debug.LogError("Chỉ số thẻ bài không hợp lệ!");
            return;
        }

        selectedCardIndex = index;
        Debug.Log($"[{photonView.Owner.NickName}] chọn: {playerCards[index].name}");

        if (PhotonNetwork.InRoom)
        {
            MultiplayerGameManager gameManager = FindObjectOfType<MultiplayerGameManager>();
            if (gameManager != null)
            {
                Debug.Log("Gọi OnCardSelected trong MultiplayerGameManager.");
                gameManager.OnCardSelected(selectedCardIndex);
            }
            else
            {
                Debug.LogError("MultiplayerGameManager không được tìm thấy trong Scene!");
            }
        }
        else
        {
            Debug.LogWarning("Chưa vào phòng Photon, không thể gửi RPC.");
        }
    }


}
