using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class UIManager : MonoBehaviourPun
{
    public Button[] numberButtons; // 数字ボタンを格納する配列
    public Button backspaceButton;
    public Button postButton;
    public Button generateAnswerButton;

    public TextMeshProUGUI guessNumberText; // 数字を表示するText
    public TextMeshProUGUI myAnswerText; // 自分の答え
    public TextMeshProUGUI opponentAnswerText; // 相手の答え

    private List<int> buttonHistory = new List<int>(); // クリックされたボタンの履歴
    private const int DIGIT_NUM = 3;

    void Start()
    {
        guessNumberText.text = "";
        myAnswerText.text = "";
        opponentAnswerText.text = "";
        InitializeButtons();
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

        // 初期状態でバックスペースボタンとポストボタンを無効化
        postButton.interactable = false;
        backspaceButton.interactable = false;
        postButton.gameObject.SetActive(false);
    }

    void OnNumberButtonClicked(int number)
    {
        // 現在のテキストの長さをチェックして、DIGIT_NUM未満の場合のみ追加
        if (guessNumberText.text.Length < DIGIT_NUM)
        {
            AddNumberToDisplay(number);
        }

        UpdatePostButtonState();
    }

    void OnBackspaceClicked()
    {
        if (buttonHistory.Count > 0)
        {
            RemoveLastNumberFromGuessNumber();
        }

        UpdatePostButtonState();
    }

    void OnPostButtonClick()
    {
        ResetGuessNumber();

        // 自分の答えを相手と同期する
        photonView.RPC("UpdateOpponentAnswer", RpcTarget.OthersBuffered, myAnswerText.text);
    }

    void OnGenerateAnswerClick()
    {
        // myAnswerText.textに保存
        myAnswerText.text = guessNumberText.text;

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
        //postButtonを作成
        postButton.gameObject.SetActive(true);
        postButton.interactable = false;
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
}
