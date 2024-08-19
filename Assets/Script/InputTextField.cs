using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking; // UnityWebRequest���g�p���邽�߂ɒǉ�

public class InputFieldToText : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button displayButton;

    // �T�[�o�[��URL�i�K�؂ɕύX���Ă��������j
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
        // user�p�����[�^��K�؂ɐݒ肷��
        string user = "testUser";

        // URL�G���R�[�h���ꂽ�t�H�[���f�[�^������
        WWWForm form = new WWWForm();
        form.AddField("user", user);
        form.AddField("message", textToSend);

        // POST���N�G�X�g�̏���
        UnityWebRequest request = UnityWebRequest.Post(serverURL, form);

        // ���N�G�X�g���M
        yield return request.SendWebRequest();

        // �G���[�`�F�b�N
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
