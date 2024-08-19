using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PhotonRoomJoiner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject roomListContainer; // �������X�g��\������R���e�i
    [SerializeField] private GameObject roomListItemPrefab; // �������X�g�A�C�e���̃v���n�u

    private List<RoomInfo> roomList = new List<RoomInfo>();

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Master Server�ɐڑ����܂���");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("���r�[�ɎQ�����܂���");
        // PhotonNetwork.JoinLobby(); // ���̍s�͕s�v
        SceneManager.LoadScene("02_Room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log(" �������X�g���X�V����܂���");
        this.roomList = roomList;
        UpdateRoomListUI();
    }

    private void UpdateRoomListUI()
    {
        // ���݂̃��X�g���N���A
        foreach (Transform child in roomListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // �V�������X�g���쐬
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
        Debug.Log("���[���ɐ���ɎQ�����܂���");
        // �����ɓ�������̏�����ǉ�
        SceneManager.LoadScene("02_Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
    }
}
