using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject roomListContainer; // 部屋リストを表示するコンテナ
    [SerializeField] private GameObject roomListItemPrefab; // 部屋リストアイテムのプレハブ
    [SerializeField] private Button createRoomButton; // 部屋を立てるボタン
    [SerializeField] private TMP_InputField userNameInputField; // ユーザ名を入力するフィールド
    [SerializeField] private TMP_Text errorText; // エラーメッセージを表示するUI要素を取得

    // それぞれのボタンをUnity Editorで設定するための配列
    [SerializeField] private Button[] letterButtons;
    [SerializeField] private Button backspaceButton; // バックスペースボタン

    [SerializeField] private AudioClip pushButton;

    private List<RoomInfo> roomList = new List<RoomInfo>();

    void Start()
    {
        // ボタンのクリックイベントを設定
        createRoomButton.onClick.AddListener(CreateRoom);

        // 各ボタンにイベントリスナーを追加
        foreach (Button button in letterButtons)
        {
            string buttonText = button.GetComponentInChildren<TMP_Text>().text; // ボタンの文字を取得
            button.onClick.AddListener(() => AddLetter(buttonText)); // ボタンが押されたときに文字を追加
        }

        backspaceButton.onClick.AddListener(RemoveLastLetter); // バックスペースボタンが押されたときに文字を削除

        // ユーザ名を設定（デフォルトは "testUser"）
        PhotonNetwork.NickName = "名無し";

        // createRoomButtonを非活性化
        createRoomButton.interactable = false;

        // ユーザ名入力フィールドの変更イベントを監視してボタンの状態を更新
        userNameInputField.onValueChanged.AddListener(UpdateCreateRoomButtonState);

        // サーバに接続
        PhotonNetwork.ConnectUsingSettings();
    }

    // ボタンが押されたときに文字を追加するメソッド
    private void AddLetter(string letter)
    {
        GameManager.instance.PlaySE(pushButton);
        userNameInputField.text += letter; // フィールドに文字を追加
    }

    // バックスペースボタンが押されたときに最後の文字を削除するメソッド
    private void RemoveLastLetter()
    {
        GameManager.instance.PlaySE(pushButton);
        if (userNameInputField.text.Length > 0)
        {
            userNameInputField.text = userNameInputField.text.Substring(0, userNameInputField.text.Length - 1); // 最後の文字を削除
        }
    }

    private void UpdateCreateRoomButtonState(string input)
    {
        // 入力フィールドが空でない場合のみボタンを活性化
        createRoomButton.interactable = !string.IsNullOrEmpty(input);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Master Serverに接続しました");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("ロビーに参加しました");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("部屋リストが更新されました");
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
        // ユーザ名を設定（入力フィールドから取得）
        if (!string.IsNullOrEmpty(userNameInputField.text))
        {
            PhotonNetwork.NickName = userNameInputField.text;
        }
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
        string translatedMessage = "";

        // メッセージをチェックして日本語に翻訳
        switch (message)
        {
            case "Game does not exist":
                translatedMessage = "部屋が存在しません。";
                break;
            case "Game full":
                translatedMessage = "部屋が満員です。";
                break;
            case "Game closed":
                translatedMessage = "部屋が閉じられています。";
                break;
            default:
                translatedMessage = "部屋に参加できませんでした。";
                break;
        }

        Debug.LogError("Failed to join room: " + message);

        // 日本語メッセージを画面に表示
        // 例: TMP_Textを使ってエラーメッセージを表示        
        errorText.text = translatedMessage;
    }

    private void CreateRoom()
    {
        GameManager.instance.PlaySE(pushButton);
        // ユーザ名を設定（入力フィールドから取得）
        if (!string.IsNullOrEmpty(userNameInputField.text))
        {
            PhotonNetwork.NickName = userNameInputField.text;
        }

        string roomName = Random.Range(1000, 9999) + "_" + PhotonNetwork.NickName; // 部屋名を作成した人の名前にする
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // 最大プレイヤー数を設定

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
        // 失敗した場合、リトライや他の処理をここに追加
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered Room: " + newPlayer.NickName);
    }
}
