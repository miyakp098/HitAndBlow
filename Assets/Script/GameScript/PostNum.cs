using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostNum : MonoBehaviour
{
    public Button[] numberButtons; // ボタンを格納する配列
    public TextMeshProUGUI numberDisplay; // 数字を表示するText
    public Button backspaceButton; // バックスペースボタン
    public Button postButton; // ポストボタン

    private List<int> buttonHistory = new List<int>(); // クリックされたボタンの履歴
    private const int DIGIT_NUM = 3;

    void Start()
    {
        numberDisplay.text = "";
        InitializeButtons();

        // 初期状態でバックスペースボタンとポストボタンを無効化
        backspaceButton.interactable = false;
        postButton.interactable = false;
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
        numberDisplay.text = "";

        foreach (var button in numberButtons)
        {
            button.interactable = true;
        }

        buttonHistory.Clear();

        // ポストボタンの活性状態を更新
        UpdatePostButtonState();
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
        }
        else
        {
            postButton.interactable = false;//非活性
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
