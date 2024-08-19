using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking; // UnityWebRequestを使用するために追加

public class InputFieldToText : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button displayButton;

    // サーバーのURL（適切に変更してください）
    private string serverURL = "http://localhost:8080/messages";

    void Start()
    {
        displayButton.onClick.AddListener(OnDisplayButtonClicked);
    }

    void OnDisplayButtonClicked()
    {
        StartCoroutine(SendTextToServer(inputField.text));

    }

    IEnumerator SendTextToServer(string textToSend)
    {
        // userパラメータを適切に設定する
        string user = "testUser";

        // URLエンコードされたフォームデータを準備
        WWWForm form = new WWWForm();
        form.AddField("user", user);
        form.AddField("message", textToSend);

        // POSTリクエストの準備
        UnityWebRequest request = UnityWebRequest.Post(serverURL, form);

        // リクエスト送信
        yield return request.SendWebRequest();

        // エラーチェック
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Text sent successfully");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
}
