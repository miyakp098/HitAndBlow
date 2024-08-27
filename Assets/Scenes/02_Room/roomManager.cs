using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class roomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI roomCreatorText; // ���[���쐬�҂̃e�L�X�g
    [SerializeField] private TextMeshProUGUI joinedPlayerText; // ���������v���C���[�̃e�L�X�g
    [SerializeField] private Button startButton;

    void Start()
    {
        UpdatePlayerList();
        startButton.onClick.AddListener(OnStartButtonClicked);
        startButton.interactable = false; // ������ԂŃ{�^�����\��
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        List<string> playerNames = new List<string>();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerNames.Add(player.NickName);
        }

        Player[] players = PhotonNetwork.PlayerList;
        //playerListText.text = "Players in room:\n" + string.Join("\n", playerNames);
        roomCreatorText.text = players[0].NickName;

        // 2�l�ڂ̃v���C���[������ꍇ�̕\��
        if (players.Length > 1)
        {
            joinedPlayerText.text = players[1].NickName;
        }
        else
        {
            joinedPlayerText.text = "�ҋ@��...";
        }

        // ���[���쐬�҂�����Start�{�^����������悤�ɂ���
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }

        // �v���C���[����2�l�Ȃ�X�^�[�g�{�^����������
        if(players.Length == 2)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
        
    }

    private void OnStartButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // �S�v���C���[�ɃV�[���J�ڂ�RPC�𑗐M
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartGame()
    {
        // �V�[����؂�ւ���
        SceneManager.LoadScene("03_GameScene");
    }
}
