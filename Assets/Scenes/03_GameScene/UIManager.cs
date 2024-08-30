using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class UIManager : MonoBehaviourPun
{
    public Button[] numberButtons; // �����{�^�����i�[����z��
    public Button backspaceButton;
    public Button postButton;
    public Button generateAnswerButton;

    public TextMeshProUGUI guessNumberText; // ������\������Text
    public TextMeshProUGUI myAnswerText; // �����̓���
    public TextMeshProUGUI opponentAnswerText; // ����̓���

    private List<int> buttonHistory = new List<int>(); // �N���b�N���ꂽ�{�^���̗���
    private const int DIGIT_NUM = 3;

    void Start()
    {
        guessNumberText.text = "";
        myAnswerText.text = "";
        opponentAnswerText.text = "";
        InitializeButtons();
    }

    void InitializeButtons()
    {        
        // �e�{�^���Ƀ��X�i�[��ǉ�
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int index = i; // ���[�J���ϐ��ɃR�s�[���ăN���[�W����h��
            numberButtons[i].onClick.AddListener(() => OnNumberButtonClicked(index));
        }

        // �C�x���g���X�i�[�̒ǉ�
        backspaceButton.onClick.AddListener(OnBackspaceClicked);
        postButton.onClick.AddListener(OnPostButtonClick);
        generateAnswerButton.onClick.AddListener(OnGenerateAnswerClick);

        // ������ԂŃo�b�N�X�y�[�X�{�^���ƃ|�X�g�{�^���𖳌���
        postButton.interactable = false;
        backspaceButton.interactable = false;
        postButton.gameObject.SetActive(false);
    }

    void OnNumberButtonClicked(int number)
    {
        // ���݂̃e�L�X�g�̒������`�F�b�N���āADIGIT_NUM�����̏ꍇ�̂ݒǉ�
        if (guessNumberText.text.Length < DIGIT_NUM)
        {
            AddNumberToDisplay(number);
        }

        UpdatePostButtonState();
    }

    void OnBackspaceClicked()
    {
        if (buttonHistory.Count > 0)
        {
            RemoveLastNumberFromGuessNumber();
        }

        UpdatePostButtonState();
    }

    void OnPostButtonClick()
    {
        ResetGuessNumber();

        // �����̓����𑊎�Ɠ�������
        photonView.RPC("UpdateOpponentAnswer", RpcTarget.OthersBuffered, myAnswerText.text);
    }

    void OnGenerateAnswerClick()
    {
        // myAnswerText.text�ɕۑ�
        myAnswerText.text = guessNumberText.text;

        ResetGuessNumber();
        generateAnswerButton.gameObject.SetActive(false);

        // �����̓������������ꂽ���Ƃ𑊎�ɒʒm
        photonView.RPC("UpdateOpponentAnswer", RpcTarget.OthersBuffered, myAnswerText.text);


        //�����������̃v���C���[���쐬������postButton�{�^���𐶐�
        if (myAnswerText.text != "" && opponentAnswerText.text != "")
        {
            photonView.RPC("HandleButtonsAfterAnswerGenerated", RpcTarget.AllBuffered);
        }
    }

    void ResetGuessNumber()
    {
        guessNumberText.text = "";
        foreach (var numberButton in numberButtons)
        {
            numberButton.interactable = true;
        }
        buttonHistory.Clear();

        UpdatePostButtonState();
    }

    [PunRPC]
    void UpdateOpponentAnswer(string answerText)
    {
        opponentAnswerText.text = answerText;
    }

    [PunRPC]
    void HandleButtonsAfterAnswerGenerated()
    {
        //postButton���쐬
        postButton.gameObject.SetActive(true);
        postButton.interactable = false;
    }

    void AddNumberToDisplay(int number)
    {        
        guessNumberText.text += number.ToString();// �{�^�����N���b�N���ꂽ���ɐ�����ǉ�
        numberButtons[number].interactable = false;// �N���b�N���ꂽ�{�^���𖳌���
        buttonHistory.Add(number);// �N���b�N���ꂽ�{�^���̗�����ۑ�
    }
        
    void RemoveLastNumberFromGuessNumber()
    {        
        int lastNumber = buttonHistory[buttonHistory.Count - 1];// �Ō�ɃN���b�N���ꂽ�{�^���̔ԍ����擾

        guessNumberText.text = guessNumberText.text.Substring(0, guessNumberText.text.Length - 1);// �e�L�X�g����Ō�̕������폜
        numberButtons[lastNumber].interactable = true;// �{�^�����ēx������
        buttonHistory.RemoveAt(buttonHistory.Count - 1);// ��������Ō�̔ԍ����폜
    }

    void UpdatePostButtonState()
    {
        // �e�L�X�g�𖄂߂���post�{�^��������
        if(guessNumberText.text.Length == DIGIT_NUM)
        {
            postButton.interactable = true;//����
            generateAnswerButton.interactable = true;//����
        }
        else
        {
            postButton.interactable = false;//�񊈐�
            generateAnswerButton.interactable = false;//�񊈐�
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
