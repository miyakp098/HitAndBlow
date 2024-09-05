using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using System.Linq;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviourPun
{
    public Button[] numberButtons; // 数字ボタンを格納する配列
    public Button backspaceButton;
    public Button postButton;
    public Button generateAnswerButton;
    public Button returnStartSceneButton;

    public TextMeshProUGUI guessNumberText; // 数字を表示するText
    public TextMeshProUGUI myAnswerText; // 自分の答え
    public TextMeshProUGUI opponentAnswerText; // 相手の答え

    public TextMeshProUGUI explainText;
    public TextMeshProUGUI myGuessListText;
    public TextMeshProUGUI opponentGuessListText;
    public TextMeshProUGUI opponentTurnText;
    public GameObject objectToHide;

    public AudioClip pushButtonNum;
    public AudioClip pushButtonBS;

    private List<int> buttonHistory = new List<int>(); // クリックされたボタンの履歴
    private const int DIGIT_NUM = 3;
    private int myHits = 0;
    private int opponentHits = 0;

    private int[] answer;
    private List<int[]> guesses;
    public List<int[]> GetGuesses()
    {
        return guesses;
    }

    private bool isGameOver = false; // ゲーム終了フラグ

    void Start()
    {
        guessNumberText.text = "";
        myAnswerText.text = "";
        opponentAnswerText.text = "";
        opponentTurnText.text = "";
        explainText.text = "自分の3桁の数字を設定";

        InitializeButtons();

        guesses = new List<int[]>();
    } 

    void InitializeButtons()
    {        
        // 各ボタンにリスナーを追加
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int index = i; // ローカル変数にコピーしてクロージャを防ぐ
            numberButtons[i].onClick.AddListener(() => OnNumberButtonClicked(index));
        }

        // イベントリスナーの追加
        backspaceButton.onClick.AddListener(OnBackspaceClicked);
        postButton.onClick.AddListener(OnPostButtonClick);
        generateAnswerButton.onClick.AddListener(OnGenerateAnswerClick);
        returnStartSceneButton.onClick.AddListener(OnReturnStartSceneClick);
        
        // 初期状態でバックスペースボタンとポストボタンを無効化
        postButton.interactable = false;
        backspaceButton.interactable = false;
        postButton.gameObject.SetActive(false);
        returnStartSceneButton.gameObject.SetActive(false);
    }

    void OnNumberButtonClicked(int number)
    {
        GameManager.instance.PlaySE(pushButtonNum);

        // 現在のテキストの長さをチェックして、DIGIT_NUM未満の場合のみ追加
        if (guessNumberText.text.Length < DIGIT_NUM)
        {
            AddNumberToDisplay(number);
        }

        UpdatePostButtonState();
    }

    void OnBackspaceClicked()
    {
        GameManager.instance.PlaySE(pushButtonBS);

        if (buttonHistory.Count > 0)
        {
            RemoveLastNumberFromGuessNumber();
        }

        UpdatePostButtonState();
    }

    void OnPostButtonClick()
    {
        GameManager.instance.PlaySE(pushButtonNum);
        if (isGameOver) return; // ゲーム終了後は何も行わない

        string guessInput = guessNumberText.text;

        if (guessInput.Length != DIGIT_NUM || !int.TryParse(guessInput, out _))
        {
            explainText.text = "Please enter a valid 3 digit number.";
            return;
        }

        int[] guess = new int[DIGIT_NUM];
        for (int i = 0; i < guessInput.Length; i++)
        {
            guess[i] = int.Parse(guessInput[i].ToString());
        }

        var (hits, blows) = CheckGuess(guess);

        myHits = hits;
        myGuessListText.text += $"   {guessInput}      {hits}   {blows}\n";
        photonView.RPC("SyncResult", RpcTarget.OthersBuffered, guessInput, hits, blows); // 他のプレイヤーに結果を共有

        

        if (hits == 3)
        {
            opponentTurnText.text = "";
            explainText.text = "相手がターン待機";
            objectToHide.SetActive(false);
            DisableAllButtons();
            
        }

        int myGuessCount = myGuessListText.text.Split('\n').Length;
        int opponentGuessCount = opponentGuessListText.text.Split('\n').Length;

        

        ResetGuessNumber();

        // 自分の postButton を無効化し、他のプレイヤーの postButton を有効化
        postButton.gameObject.SetActive(false);
        opponentTurnText.text = "";
        photonView.RPC("EnableOtherPlayerPostButton", RpcTarget.Others); // 他プレイヤーのボタンを有効化する

        // 自分の答えを相手と同期する
        photonView.RPC("UpdateOpponentAnswer", RpcTarget.OthersBuffered, myAnswerText.text);

        if ((myHits == DIGIT_NUM || opponentHits == DIGIT_NUM) && myGuessCount == opponentGuessCount)
        {
            photonView.RPC("CheckWinCondition", RpcTarget.All);
        }
    }

    [PunRPC]
    void EnableOtherPlayerPostButton()
    {
        postButton.gameObject.SetActive(true);
        opponentTurnText.text = "";
    }

    [PunRPC]
    void SyncResult(string guessInput, int hits, int blows)
    {
        opponentGuessListText.text += $"   {guessInput}      {hits}   {blows}\n";
        opponentHits = hits;
    }

    [PunRPC]
    void GameEnd()
    {
        returnStartSceneButton.gameObject.SetActive(true);
    }

    [PunRPC]
    void DisableOpponentPostButton()
    {
        postButton.gameObject.SetActive(false);
    }

    [PunRPC]
    void CheckWinCondition()
    {
        DisableAllButtons();

        if (myHits > opponentHits)
        {
            explainText.text = "あなたの勝ち！";
            photonView.RPC("SetLoserText", RpcTarget.Others); // 相手に負けメッセージを表示
        }
        else if (myHits < opponentHits)
        {
            explainText.text = "あなたの負け！";
            photonView.RPC("SetWinnerText", RpcTarget.Others); // 相手に勝ちメッセージを表示
        }
        else
        {
            explainText.text = "引き分け！";
            photonView.RPC("SetDrawText", RpcTarget.Others); // 引き分けメッセージを表示
        }

        // ゲーム終了処理を呼び出す
        photonView.RPC("GameEnd", RpcTarget.All);
        
    }

    [PunRPC]
    void SetWinnerText()
    {
        // 自分が勝った場合
        explainText.text = "あなたの勝ち！";
    }

    [PunRPC]
    void SetLoserText()
    {
        // 相手が負けた場合
        explainText.text = "あなたの負け！";
    }

    [PunRPC]
    void SetDrawText()
    {
        // 引き分けの場合
        explainText.text = "引き分け！";
    }

    void DisableAllButtons() //ボタンを非活性にする
    {
        
        foreach (var button in numberButtons)
        {
            button.gameObject.SetActive(false);
        }

        backspaceButton.gameObject.SetActive(false);
        postButton.gameObject.SetActive(false);
    }

    

    void OnGenerateAnswerClick()
    {
        GameManager.instance.PlaySE(pushButtonNum);

        // myAnswerText.textに保存
        myAnswerText.text = guessNumberText.text;
        explainText.text = "相手の入力が終わるまで待機...";
        ResetGuessNumber();
        generateAnswerButton.gameObject.SetActive(false);

        // 自分の答えが生成されたことを相手に通知
        photonView.RPC("UpdateOpponentAnswer", RpcTarget.OthersBuffered, myAnswerText.text);


        //答えが両方のプレイヤーが作成したらpostButtonボタンを生成
        if (myAnswerText.text != "" && opponentAnswerText.text != "")
        {
            photonView.RPC("HandleButtonsAfterAnswerGenerated", RpcTarget.AllBuffered);
        }
    }

    void OnReturnStartSceneClick()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.NickName = "名無し";
        SceneManager.LoadScene("01_StartScene");
    }

    void ResetGuessNumber()
    {
        guessNumberText.text = "";
        foreach (var numberButton in numberButtons)
        {
            numberButton.interactable = true;
        }
        buttonHistory.Clear();

        UpdatePostButtonState();
    }

    [PunRPC]
    void UpdateOpponentAnswer(string answerText)
    {
        opponentAnswerText.text = answerText;
    }

    [PunRPC]
    void HandleButtonsAfterAnswerGenerated()
    {
        opponentTurnText.text = "";
        //postButtonを作成
        if (PhotonNetwork.IsMasterClient)
        {
            // 自分がマスターの時、自分のポストボタンを有効化、相手のポストボタンを無効化
            postButton.gameObject.SetActive(true);
        }

        postButton.interactable = false;

        explainText.text = "相手の数字を予想";
    }

    

    void AddNumberToDisplay(int number)
    {        
        guessNumberText.text += number.ToString();// ボタンがクリックされた時に数字を追加
        numberButtons[number].interactable = false;// クリックされたボタンを無効化
        buttonHistory.Add(number);// クリックされたボタンの履歴を保存
    }
        
    void RemoveLastNumberFromGuessNumber()
    {        
        int lastNumber = buttonHistory[buttonHistory.Count - 1];// 最後にクリックされたボタンの番号を取得

        guessNumberText.text = guessNumberText.text.Substring(0, guessNumberText.text.Length - 1);// テキストから最後の文字を削除
        numberButtons[lastNumber].interactable = true;// ボタンを再度活性化
        buttonHistory.RemoveAt(buttonHistory.Count - 1);// 履歴から最後の番号を削除
    }

    void UpdatePostButtonState()
    {
        // テキストを埋めたらpostボタン活性化
        if(guessNumberText.text.Length == DIGIT_NUM)
        {
            postButton.interactable = true;//活性
            generateAnswerButton.interactable = true;//活性
        }
        else
        {
            postButton.interactable = false;//非活性
            generateAnswerButton.interactable = false;//非活性
        }

        // テキストに数字が入っている場合ボタン活性化
        if (buttonHistory.Count > 0)
        {
            backspaceButton.interactable = true;
        }
        else
        {
            backspaceButton.interactable = false;
        }     
    }

    public (int hits, int blows) CheckGuess(int[] guess)
    {
        int hits = 0;
        int blows = 0;

        answer = new int[DIGIT_NUM];

        for (int i = 0; i < DIGIT_NUM; i++)
        {
            answer[i] = int.Parse(opponentAnswerText.text[i].ToString());
        }

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == answer[i])
            {
                hits++;
            }
            else if (answer.Contains(guess[i]))
            {
                blows++;
            }
        }

        guesses.Add(guess);
        return (hits, blows);
    }
}
