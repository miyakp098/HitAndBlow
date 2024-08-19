using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonTextChanger : MonoBehaviour
{
    public Button[] buttons;
    public Button cancelButton;
    private TextMeshProUGUI[] buttonTexts;
    private int usernameButtonIndex = -1;

    void Start()
    {
        // 各ボタンのTextMeshProUGUIコンポーネントを取得
        buttonTexts = new TextMeshProUGUI[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError($"Button at index {i} is not assigned.");
                continue;
            }

            buttonTexts[i] = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonTexts[i] == null)
            {
                Debug.LogError($"No TextMeshProUGUI component found on button at index {i}.");
                continue;
            }

            // テキストが空の場合のみボタンを有効化
            if (string.IsNullOrEmpty(buttonTexts[i].text))
            {
                buttons[i].interactable = true;
            }
            else
            {
                buttons[i].interactable = false;
            }

            // ボタンのクリックイベントにリスナーを追加
            int index = i;  // キャプチャ変数のためにローカル変数を使用
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

        // キャンセルボタンのクリックイベントにリスナーを追加
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
        else
        {
            Debug.LogError("Cancel button is not assigned.");
        }
    }

    void OnButtonClicked(int buttonIndex)
    {
        Debug.Log($"Button {buttonIndex} clicked");  // 追加: デバッグログ

        // PlayerPrefsからユーザー名を取得
        string username = PlayerPrefs.GetString("Username", "Guest");

        // 現在のボタンのテキストをユーザー名に変更
        buttonTexts[buttonIndex].text = username;

        // ユーザー名が入力されたボタンのインデックスを記録
        usernameButtonIndex = buttonIndex;

        // すべてのボタンを無効化
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }

    void OnCancelButtonClicked()
    {
        Debug.Log("Cancel button clicked");  // 追加: デバッグログ

        if (usernameButtonIndex != -1)
        {
            // ユーザー名が入力されているボタンのテキストを空にする
            buttonTexts[usernameButtonIndex].text = "";

            // すべてのボタンを再度有効化
            for (int i = 0; i < buttons.Length; i++)
            {
                if (string.IsNullOrEmpty(buttonTexts[i].text))
                {
                    buttons[i].interactable = true;
                }
            }

            // ユーザー名が入力されているボタンのインデックスをリセット
            usernameButtonIndex = -1;
        }
    }
}
