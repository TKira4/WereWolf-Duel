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
        // Chỉ thiết lập UI cho local player
        if (!photonView.IsMine) return;
        InitializeCardButtons();

        // Debug: in tên các lá bài
        for (int i = 0; i < playerCards.Length; i++)
        {
            Debug.Log($"Card {i}: {playerCards[i].name}");
        }
    }

    void InitializeCardButtons()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i;
            cardButtons[i].onClick.AddListener(() => OnCardSelected(idx));
        }
    }

    public void OnCardSelected(int index)
    {
        if (index < 0 || index >= playerCards.Length) return;
        selectedCardIndex = index;
        Debug.Log($"[{photonView.Owner.NickName}] chọn: {playerCards[index].name}");

        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(OnCardChosenRPC), RpcTarget.AllBuffered, selectedCardIndex);
        }
        else
        {
            Debug.LogWarning("Chưa vào phòng Photon, không thể gửi RPC.");
        }
    }

    [PunRPC]
    private void OnCardChosenRPC(int cardIndex)
    {
        Debug.Log($"RPC nhận: lá bài được chọn là {playerCards[cardIndex].name}");
        // Ví dụ cập nhật sprite nếu có SpriteRenderer
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && playerCards[cardIndex].TryGetComponent<SpriteRenderer>(out var cardSr))
        {
            sr.sprite = cardSr.sprite;
        }
    }
}
