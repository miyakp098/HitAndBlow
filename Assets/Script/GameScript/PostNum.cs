using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostNum : MonoBehaviour
{
    public Button[] numberButtons; // �{�^�����i�[����z��
    public TextMeshProUGUI numberDisplay; // ������\������Text
    public Button backspaceButton; // �o�b�N�X�y�[�X�{�^��
    public Button postButton; // �|�X�g�{�^��

    private List<int> buttonHistory = new List<int>(); // �N���b�N���ꂽ�{�^���̗���
    private const int DIGIT_NUM = 3;

    void Start()
    {
        numberDisplay.text = "";
        InitializeButtons();

        // ������ԂŃo�b�N�X�y�[�X�{�^���ƃ|�X�g�{�^���𖳌���
        backspaceButton.interactable = false;
        postButton.interactable = false;
    }

    void InitializeButtons()
    {        
        // �e�{�^���Ƀ��X�i�[��ǉ�
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int index = i; // ���[�J���ϐ��ɃR�s�[���ăN���[�W����h��
            numberButtons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

        backspaceButton.onClick.AddListener(OnBackspaceClicked);
        postButton.onClick.AddListener(OnPostButtonClick);
    }

    void OnButtonClicked(int number)
    {
        // ���݂̃e�L�X�g�̒������`�F�b�N���āADIGIT_NUM�����̏ꍇ�̂ݒǉ�
        if (numberDisplay.text.Length < DIGIT_NUM)
        {
            AddNumberToDisplay(number);
        }

        // �|�X�g�{�^���̊�����Ԃ��X�V
        UpdatePostButtonState();
    }

    void OnBackspaceClicked()
    {
        if (buttonHistory.Count > 0)
        {
            RemoveLastNumberFromDisplay();
        }

        // �|�X�g�{�^���̊�����Ԃ��X�V
        UpdatePostButtonState();
    }

    void OnPostButtonClick()
    {
        numberDisplay.text = "";

        foreach (var button in numberButtons)
        {
            button.interactable = true;
        }

        buttonHistory.Clear();

        // �|�X�g�{�^���̊�����Ԃ��X�V
        UpdatePostButtonState();
    }

    void AddNumberToDisplay(int number)
    {        
        numberDisplay.text += number.ToString();// �{�^�����N���b�N���ꂽ���ɐ�����ǉ�
        numberButtons[number].interactable = false;// �N���b�N���ꂽ�{�^���𖳌���
        buttonHistory.Add(number);// �N���b�N���ꂽ�{�^���̗�����ۑ�
    }
        
    void RemoveLastNumberFromDisplay()
    {        
        int lastNumber = buttonHistory[buttonHistory.Count - 1];// �Ō�ɃN���b�N���ꂽ�{�^���̔ԍ����擾

        numberDisplay.text = numberDisplay.text.Substring(0, numberDisplay.text.Length - 1);// �e�L�X�g����Ō�̕������폜
        numberButtons[lastNumber].interactable = true;// �{�^�����ēx������
        buttonHistory.RemoveAt(buttonHistory.Count - 1);// ��������Ō�̔ԍ����폜
    }

    void UpdatePostButtonState()
    {
        // �e�L�X�g�𖄂߂���post�{�^��������
        if(numberDisplay.text.Length == DIGIT_NUM)
        {
            postButton.interactable = true;//����
        }
        else
        {
            postButton.interactable = false;//�񊈐�
        }

        // �e�L�X�g�ɐ����������Ă���ꍇ�{�^��������
        if (buttonHistory.Count > 0)
        {
            backspaceButton.interactable = true;
        }
        else
        {
            backspaceButton.interactable = false;
        }     
    }
}
