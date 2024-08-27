using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class userApiCliant : MonoBehaviour
{
    private const string ApiUrl = "http://localhost:8080/users/find";

    public string username = "testuser"; // テストするユーザー名
    public bool isLogged = true; // テストするログイン状態

    void Start()
    {
        StartCoroutine(GetUser(username, isLogged));
    }

    IEnumerator GetUser(string username, bool isLogged)
    {
        string url = $"{ApiUrl}?username={username}&isLogged={isLogged.ToString().ToLower()}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
        }
        else
        {
            Debug.Log($"Response: {request.downloadHandler.text}");
        }
    }
}
