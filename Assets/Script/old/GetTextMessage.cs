using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class GetTextMessage : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public float pollingInterval = 5f; // ポーリング間隔（秒）

    // サーバーのURL（適切に変更してください）
    private string serverURL = "http://localhost:8080/messages/latest";

    void Start()
    {
        // スタート時にポーリングを開始
        StartCoroutine(PollForMessages());
    }

    IEnumerator PollForMessages()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(serverURL);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Message fetched successfully");
                string jsonResponse = request.downloadHandler.text;
                Message message = JsonUtility.FromJson<Message>(jsonResponse);
                DisplayMessage(message);
            }

            // ポーリング間隔の待機
            yield return new WaitForSeconds(pollingInterval);
        }
    }

    void DisplayMessage(Message message)
    {
        if (message != null)
        {
            displayText.text = $"User: {message.user}\nMessage: {message.content}";
        }
        else
        {
            displayText.text = "Message not found.";
        }
    }

    [System.Serializable]
    public class Message
    {
        public long id;
        public string user;
        public string content;
    }
}
