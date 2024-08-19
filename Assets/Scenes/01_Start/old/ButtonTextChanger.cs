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
        // �e�{�^����TextMeshProUGUI�R���|�[�l���g���擾
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

            // �e�L�X�g����̏ꍇ�̂݃{�^����L����
            if (string.IsNullOrEmpty(buttonTexts[i].text))
            {
                buttons[i].interactable = true;
            }
            else
            {
                buttons[i].interactable = false;
            }

            // �{�^���̃N���b�N�C�x���g�Ƀ��X�i�[��ǉ�
            int index = i;  // �L���v�`���ϐ��̂��߂Ƀ��[�J���ϐ����g�p
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

        // �L�����Z���{�^���̃N���b�N�C�x���g�Ƀ��X�i�[��ǉ�
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
        Debug.Log($"Button {buttonIndex} clicked");  // �ǉ�: �f�o�b�O���O

        // PlayerPrefs���烆�[�U�[�����擾
        string username = PlayerPrefs.GetString("Username", "Guest");

        // ���݂̃{�^���̃e�L�X�g�����[�U�[���ɕύX
        buttonTexts[buttonIndex].text = username;

        // ���[�U�[�������͂��ꂽ�{�^���̃C���f�b�N�X���L�^
        usernameButtonIndex = buttonIndex;

        // ���ׂẴ{�^���𖳌���
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }

    void OnCancelButtonClicked()
    {
        Debug.Log("Cancel button clicked");  // �ǉ�: �f�o�b�O���O

        if (usernameButtonIndex != -1)
        {
            // ���[�U�[�������͂���Ă���{�^���̃e�L�X�g����ɂ���
            buttonTexts[usernameButtonIndex].text = "";

            // ���ׂẴ{�^�����ēx�L����
            for (int i = 0; i < buttons.Length; i++)
            {
                if (string.IsNullOrEmpty(buttonTexts[i].text))
                {
                    buttons[i].interactable = true;
                }
            }

            // ���[�U�[�������͂���Ă���{�^���̃C���f�b�N�X�����Z�b�g
            usernameButtonIndex = -1;
        }
    }
}
