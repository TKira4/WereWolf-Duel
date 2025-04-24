using Photon.Pun;
using UnityEngine;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static MultiplayerManager Instance;
    public string playerPrefabName = "Player";

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

    public override void OnJoinedRoom()
    {
        Vector3 spawnPos = Vector3.zero;
        PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
    }
}
