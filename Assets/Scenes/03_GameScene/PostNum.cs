using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PostNum : MonoBehaviourPun
{
    public Button[] numberButtons; // ボタンを格納する配列
    public TextMeshProUGUI numberDisplay; // 数字を表示するText
    public Button backspaceButton; // バックスペースボタン
    public Button postButton; // ポストボタン
    public Button generateAnswerButton; // answer生成ボタン

    public TextMeshProUGUI playerAnswerText; // プレイヤーの答えを表示するTextMeshProUGUI
    public TextMeshProUGUI opponentAnswerText; // 相手の答えを表示するTextMeshProUGUI

    private List<int> buttonHistory = new List<int>(); // クリックされたボタンの履歴
    private const int DIGIT_NUM = 3;

    void Start()
    {
        numberDisplay.text = "";
        playerAnswerText.text = "";
        opponentAnswerText.text = "";
        InitializeButtons();

        // 初期状態でバックスペースボタンとポストボタンを無効化
        postButton.interactable = false;
        backspaceButton.interactable = false;        
        postButton.gameObject.SetActive(false);
    }

    void InitializeButtons()
    {        
        // 各ボタンにリスナーを追加
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int index = i; // ローカル変数にコピーしてクロージャを防ぐ
            numberButtons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

        backspaceButton.onClick.AddListener(OnBackspaceClicked);
        postButton.onClick.AddListener(OnPostButtonClick);
        generateAnswerButton.onClick.AddListener(OnGenerateAnswerClick);
    }

    void OnButtonClicked(int number)
    {
        // 現在のテキストの長さをチェックして、DIGIT_NUM未満の場合のみ追加
        if (numberDisplay.text.Length < DIGIT_NUM)
        {
            AddNumberToDisplay(number);
        }

        // ポストボタンの活性状態を更新
        UpdatePostButtonState();
    }

    void OnBackspaceClicked()
    {
        if (buttonHistory.Count > 0)
        {
            RemoveLastNumberFromDisplay();
        }

        // ポストボタンの活性状態を更新
        UpdatePostButtonState();
    }

    void OnPostButtonClick()
    {
        ResetDisplay();

        // 自分の答えを相手と同期する
        photonView.RPC("UpdateOpponentAnswer", RpcTarget.OthersBuffered, playerAnswerText.text);
    }

    void OnGenerateAnswerClick()
    {
        int[] answer = new int[DIGIT_NUM];

        // `numberDisplay.text` を `int[]` に変換
        for (int i = 0; i < DIGIT_NUM; i++)
        {
            answer[i] = int.Parse(numberDisplay.text[i].ToString());
        }

        // 自分の答えを入力したnumberDisplay.textをplayerAnswerText.textに保存、表示
        playerAnswerText.text = numberDisplay.text;

        ResetDisplay();
        
        generateAnswerButton.gameObject.SetActive(false);

        // 自分の答えが生成されたことを相手に通知
        photonView.RPC("UpdateOpponentAnswer", RpcTarget.OthersBuffered, playerAnswerText.text);


        //答えが両方のプレイヤーが作成したらボタンを生成
        if (playerAnswerText.text != "" && opponentAnswerText.text != "")
        {
            photonView.RPC("HandleButtonsAfterAnswerGenerated", RpcTarget.AllBuffered);
        }


    }

    void ResetDisplay()
    {
        numberDisplay.text = "";
        foreach (var button in numberButtons)
        {
            button.interactable = true;
        }
        buttonHistory.Clear();
        // ポストボタンの活性状態を更新
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
        numberDisplay.text += number.ToString();// ボタンがクリックされた時に数字を追加
        numberButtons[number].interactable = false;// クリックされたボタンを無効化
        buttonHistory.Add(number);// クリックされたボタンの履歴を保存
    }
        
    void RemoveLastNumberFromDisplay()
    {        
        int lastNumber = buttonHistory[buttonHistory.Count - 1];// 最後にクリックされたボタンの番号を取得

        numberDisplay.text = numberDisplay.text.Substring(0, numberDisplay.text.Length - 1);// テキストから最後の文字を削除
        numberButtons[lastNumber].interactable = true;// ボタンを再度活性化
        buttonHistory.RemoveAt(buttonHistory.Count - 1);// 履歴から最後の番号を削除
    }

    void UpdatePostButtonState()
    {
        // テキストを埋めたらpostボタン活性化
        if(numberDisplay.text.Length == DIGIT_NUM)
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
