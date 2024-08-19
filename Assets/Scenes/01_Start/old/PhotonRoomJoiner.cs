using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PhotonRoomJoiner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject roomListContainer; // 部屋リストを表示するコンテナ
    [SerializeField] private GameObject roomListItemPrefab; // 部屋リストアイテムのプレハブ

    private List<RoomInfo> roomList = new List<RoomInfo>();

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Master Serverに接続しました");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("ロビーに参加しました");
        // PhotonNetwork.JoinLobby(); // この行は不要
        SceneManager.LoadScene("02_Room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log(" 部屋リストが更新されました");
        this.roomList = roomList;
        UpdateRoomListUI();
    }

    private void UpdateRoomListUI()
    {
        // 現在のリストをクリア
        foreach (Transform child in roomListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // 新しいリストを作成
        foreach (RoomInfo roomInfo in roomList)
        {
            GameObject roomListItem = Instantiate(roomListItemPrefab, roomListContainer.transform);
            roomListItem.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Name;
            roomListItem.GetComponent<Button>().onClick.AddListener(() => JoinRoom(roomInfo.Name));
        }
    }

    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに正常に参加しました");
        // 部屋に入った後の処理を追加
        SceneManager.LoadScene("02_Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
    }
}
