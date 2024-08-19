using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class RoomCreate : MonoBehaviour
{
    public TMP_InputField userNameInputField;
    public Button createRoomButton;
    public TextMeshProUGUI responseText;

    private string serverURL = "http://localhost:8080/rooms/create";

    void Start()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
    }

    void OnCreateRoomButtonClicked()
    {
        string userName = userNameInputField.text;
        if (!string.IsNullOrEmpty(userName))
        {
            StartCoroutine(CreateRoom(userName));
        }
        else
        {
            responseText.text = "���[�U������͂��Ă��������B";
        }
    }

    IEnumerator CreateRoom(string userName)
    {
        WWWForm form = new WWWForm();
        form.AddField("userName", userName);

        UnityWebRequest request = UnityWebRequest.Post(serverURL, form);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            responseText.text = "�G���[: " + request.error;
        }
        else
        {
            responseText.text = "�����𗧂Ă܂���: " + request.downloadHandler.text;
        }
    }
}
