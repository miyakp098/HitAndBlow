using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShowName : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI roomCreatorText; // ���[���쐬�҂̃e�L�X�g
    [SerializeField] private TextMeshProUGUI joinedPlayerText; // ���������v���C���[�̃e�L�X�g

    void Start()
    {
        UpdatePlayerList();
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
    }
}
