using Photon.Pun;
using System;
using UnityEngine;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static MultiplayerManager Instance;

    [Tooltip("Tên prefab nằm trong Assets/Resources (không kèm .prefab)")]
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

    // Khi đã join room thành công, spawn player của riêng client này
    public override void OnJoinedRoom()
    {
        Vector3 spawnPos = Vector3.zero;
        Debug.Log(playerPrefabName);
        PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
    }
}
