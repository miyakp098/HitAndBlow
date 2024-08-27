using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class GetTextMessage : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public float pollingInterval = 5f; // �|�[�����O�Ԋu�i�b�j

    // �T�[�o�[��URL�i�K�؂ɕύX���Ă��������j
    private string serverURL = "http://localhost:8080/messages/latest";

    void Start()
    {
        // �X�^�[�g���Ƀ|�[�����O���J�n
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

            // �|�[�����O�Ԋu�̑ҋ@
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
