using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking; // UnityWebRequest���g�p���邽�߂ɒǉ�

public class InputTextMessage : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI displayText;
    public Button fetchButton;

    // �T�[�o�[��URL�i�K�؂ɕύX���Ă��������j
    private string serverURL = "http://localhost:8080/messages/";

    void Start()
    {
        fetchButton.onClick.AddListener(OnFetchButtonClicked);
    }

    void OnFetchButtonClicked()
    {
        if (long.TryParse(inputField.text, out long id))
        {
            StartCoroutine(FetchMessageFromServer(id));
        }
        else
        {
            Debug.LogError("Invalid ID");
        }
    }

    IEnumerator FetchMessageFromServer(long id)
    {
        string url = serverURL + id;
        UnityWebRequest request = UnityWebRequest.Get(url);
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
