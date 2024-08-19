using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PhotonConnect : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // ���[�U����ݒ�
        PhotonNetwork.NickName = "testUser";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        CreateRoom();
    }

    private void CreateRoom()
    {
        string roomName = "Room_" + Random.Range(1000, 9999); // �����_���ȕ������𐶐�
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4; // �ő�v���C���[����ݒ�

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created Successfully");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room Creation Failed: " + message);
        // ���s�����ꍇ�A���g���C�⑼�̏����������ɒǉ�
    }
}
