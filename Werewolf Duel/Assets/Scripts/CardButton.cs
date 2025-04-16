using UnityEngine;
using TMPro;

public class CardButton : MonoBehaviour
{
    public string cardType;  // Tên loại card, ví dụ: "Sói", "Tiên tri", ...
    public TextMeshProUGUI resultText;  // Hiển thị kết quả
    public bool isPlayerCard = true;  // Kiểm tra thẻ của người chơi (nếu false là AI)

    // Hàm gọi khi người chơi click vào thẻ
    public void OnClick()
    {
        // Kiểm tra xem các đối tượng cần thiết đã được gán chưa
        if (resultText == null || GameManager.instance == null)
        {
            Debug.LogError("Một trong các đối tượng cần thiết chưa được gán.");
            return;
        }

        // Gọi hàm xử lý trong GameManager
        GameManager.instance.PlayerChoseCard(cardType); // Gọi hàm chọn thẻ của người chơi

        // Loại bỏ thẻ bài đã chọn khỏi PlayerPanel
        Destroy(gameObject);  // Xóa thẻ bài đã chọn
    }
}
