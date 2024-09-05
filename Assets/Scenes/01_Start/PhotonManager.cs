using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject roomListContainer; // �������X�g��\������R���e�i
    [SerializeField] private GameObject roomListItemPrefab; // �������X�g�A�C�e���̃v���n�u
    [SerializeField] private Button createRoomButton; // �����𗧂Ă�{�^��
    [SerializeField] private TMP_InputField userNameInputField; // ���[�U������͂���t�B�[���h
    [SerializeField] private TMP_Text errorText; // �G���[���b�Z�[�W��\������UI�v�f���擾

    // ���ꂼ��̃{�^����Unity Editor�Őݒ肷�邽�߂̔z��
    [SerializeField] private Button[] letterButtons;
    [SerializeField] private Button backspaceButton; // �o�b�N�X�y�[�X�{�^��

    [SerializeField] private AudioClip pushButton;

    private List<RoomInfo> roomList = new List<RoomInfo>();

    void Start()
    {
        // �{�^���̃N���b�N�C�x���g��ݒ�
        createRoomButton.onClick.AddListener(CreateRoom);

        // �e�{�^���ɃC�x���g���X�i�[��ǉ�
        foreach (Button button in letterButtons)
        {
            string buttonText = button.GetComponentInChildren<TMP_Text>().text; // �{�^���̕������擾
            button.onClick.AddListener(() => AddLetter(buttonText)); // �{�^���������ꂽ�Ƃ��ɕ�����ǉ�
        }

        backspaceButton.onClick.AddListener(RemoveLastLetter); // �o�b�N�X�y�[�X�{�^���������ꂽ�Ƃ��ɕ������폜

        // ���[�U����ݒ�i�f�t�H���g�� "testUser"�j
        PhotonNetwork.NickName = "������";

        // createRoomButton��񊈐���
        createRoomButton.interactable = false;

        // ���[�U�����̓t�B�[���h�̕ύX�C�x���g���Ď����ă{�^���̏�Ԃ��X�V
        userNameInputField.onValueChanged.AddListener(UpdateCreateRoomButtonState);

        // �T�[�o�ɐڑ�
        PhotonNetwork.ConnectUsingSettings();
    }

    // �{�^���������ꂽ�Ƃ��ɕ�����ǉ����郁�\�b�h
    private void AddLetter(string letter)
    {
        GameManager.instance.PlaySE(pushButton);
        userNameInputField.text += letter; // �t�B�[���h�ɕ�����ǉ�
    }

    // �o�b�N�X�y�[�X�{�^���������ꂽ�Ƃ��ɍŌ�̕������폜���郁�\�b�h
    private void RemoveLastLetter()
    {
        GameManager.instance.PlaySE(pushButton);
        if (userNameInputField.text.Length > 0)
        {
            userNameInputField.text = userNameInputField.text.Substring(0, userNameInputField.text.Length - 1); // �Ō�̕������폜
        }
    }

    private void UpdateCreateRoomButtonState(string input)
    {
        // ���̓t�B�[���h����łȂ��ꍇ�̂݃{�^����������
        createRoomButton.interactable = !string.IsNullOrEmpty(input);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Master Server�ɐڑ����܂���");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("���r�[�ɎQ�����܂���");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("�������X�g���X�V����܂���");
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
        // ���[�U����ݒ�i���̓t�B�[���h����擾�j
        if (!string.IsNullOrEmpty(userNameInputField.text))
        {
            PhotonNetwork.NickName = userNameInputField.text;
        }
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
        string translatedMessage = "";

        // ���b�Z�[�W���`�F�b�N���ē��{��ɖ|��
        switch (message)
        {
            case "Game does not exist":
                translatedMessage = "���������݂��܂���B";
                break;
            case "Game full":
                translatedMessage = "�����������ł��B";
                break;
            case "Game closed":
                translatedMessage = "�����������Ă��܂��B";
                break;
            default:
                translatedMessage = "�����ɎQ���ł��܂���ł����B";
                break;
        }

        Debug.LogError("Failed to join room: " + message);

        // ���{�ꃁ�b�Z�[�W����ʂɕ\��
        // ��: TMP_Text���g���ăG���[���b�Z�[�W��\��        
        errorText.text = translatedMessage;
    }

    private void CreateRoom()
    {
        GameManager.instance.PlaySE(pushButton);
        // ���[�U����ݒ�i���̓t�B�[���h����擾�j
        if (!string.IsNullOrEmpty(userNameInputField.text))
        {
            PhotonNetwork.NickName = userNameInputField.text;
        }

        string roomName = Random.Range(1000, 9999) + "_" + PhotonNetwork.NickName; // ���������쐬�����l�̖��O�ɂ���
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // �ő�v���C���[����ݒ�

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created Successfully");
        SceneManager.LoadScene("02_Room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {        
        Debug.LogError("Room Creation Failed: " + message);
        // ���s�����ꍇ�A���g���C�⑼�̏����������ɒǉ�
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered Room: " + newPlayer.NickName);
    }
}
