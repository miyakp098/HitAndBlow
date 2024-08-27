using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class UserLogin : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public Button loginButton;
    private const string ApiUrl = "http://localhost:8080/users/update";

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    void OnLoginButtonClicked()
    {
        string username = usernameText.text;
        StartCoroutine(LoginUser(username));
    }

    IEnumerator LoginUser(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username cannot be empty.");
            yield break;
        }

        string url = $"{ApiUrl}?username={username}&isLogged=true";
        UnityWebRequest request = UnityWebRequest.Put(url, "");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
        }
        else
        {
            Debug.Log($"Response: {request.downloadHandler.text}");

            // ユーザー名をPlayerPrefsに保存
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();

            // ログイン成功後に画面を遷移
            SceneManager.LoadScene("02_Match"); // 遷移先のシーン名に置き換えてください
        }
    }
}
