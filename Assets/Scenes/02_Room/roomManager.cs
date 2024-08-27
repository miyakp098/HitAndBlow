using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class roomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI roomCreatorText; // ルーム作成者のテキスト
    [SerializeField] private TextMeshProUGUI joinedPlayerText; // 入室したプレイヤーのテキスト
    [SerializeField] private Button startButton;

    void Start()
    {
        UpdatePlayerList();
        startButton.onClick.AddListener(OnStartButtonClicked);
        startButton.interactable = false; // 初期状態でボタンを非表示
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

        // ルーム作成者だけがStartボタンを押せるようにする
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }

        // プレイヤー数が2人ならスタートボタンを活性化
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
            // 全プレイヤーにシーン遷移のRPCを送信
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartGame()
    {
        // シーンを切り替える
        SceneManager.LoadScene("03_GameScene");
    }
}
