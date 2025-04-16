using UnityEngine;

public class CardButton : MonoBehaviour
{
    public string cardType;

    public void OnClick()
    {
        GameManager.instance.PlayerChoseCard(cardType);
    }
}
