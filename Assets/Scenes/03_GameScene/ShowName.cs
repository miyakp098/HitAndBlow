using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShowName : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI roomCreatorText; // ルーム作成者のテキスト
    [SerializeField] private TextMeshProUGUI joinedPlayerText; // 入室したプレイヤーのテキスト

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

        // 2人目のプレイヤーがいる場合の表示
        if (players.Length > 1)
        {
            joinedPlayerText.text = players[1].NickName;
        }
        else
        {
            joinedPlayerText.text = "待機中...";
        }
    }
}
