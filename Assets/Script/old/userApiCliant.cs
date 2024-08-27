using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class userApiCliant : MonoBehaviour
{
    private const string ApiUrl = "http://localhost:8080/users/find";

    public string username = "testuser"; // �e�X�g���郆�[�U�[��
    public bool isLogged = true; // �e�X�g���郍�O�C�����

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
