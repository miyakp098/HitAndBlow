using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using System.Linq;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviourPun
{
    public Button[] numberButtons; // �����{�^�����i�[����z��
    public Button backspaceButton;
    public Button postButton;
    public Button generateAnswerButton;
    public Button returnStartSceneButton;

    public TextMeshProUGUI guessNumberText; // ������\������Text
    public TextMeshProUGUI myAnswerText; // �����̓���
    public TextMeshProUGUI opponentAnswerText; // ����̓���

    public TextMeshProUGUI explainText;
    public TextMeshProUGUI myGuessListText;
    public TextMeshProUGUI opponentGuessListText;
    public TextMeshProUGUI opponentTurnText;
    public GameObject objectToHide;

    public AudioClip pushButtonNum;
    public AudioClip pushButtonBS;

    private List<int> buttonHistory = new List<int>(); // �N���b�N���ꂽ�{�^���̗���
    private const int DIGIT_NUM = 3;
    private int myHits = 0;
    private int opponentHits = 0;

    private int[] answer;
    private List<int[]> guesses;
    public List<int[]> GetGuesses()
    {
        return guesses;
    }

    private bool isGameOver = false; // �Q�[���I���t���O

    void Start()
    {
        guessNumberText.text = "";
        myAnswerText.text = "";
        opponentAnswerText.text = "";
        opponentTurnText.text = "";
        explainText.text = "������3���̐�����ݒ�";

        InitializeButtons();

        guesses = new List<int[]>();
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
        returnStartSceneButton.onClick.AddListener(OnReturnStartSceneClick);
        
        // ������ԂŃo�b�N�X�y�[�X�{�^���ƃ|�X�g�{�^���𖳌���
        postButton.interactable = false;
        backspaceButton.interactable = false;
        postButton.gameObject.SetActive(false);
        returnStartSceneButton.gameObject.SetActive(false);
    }

    void OnNumberButtonClicked(int number)
    {
        GameManager.instance.PlaySE(pushButtonNum);

        // ���݂̃e�L�X�g�̒������`�F�b�N���āADIGIT_NUM�����̏ꍇ�̂ݒǉ�
        if (guessNumberText.text.Length < DIGIT_NUM)
        {
            AddNumberToDisplay(number);
        }

        UpdatePostButtonState();
    }

    void OnBackspaceClicked()
    {
        GameManager.instance.PlaySE(pushButtonBS);

        if (buttonHistory.Count > 0)
        {
            RemoveLastNumberFromGuessNumber();
        }

        UpdatePostButtonState();
    }

    void OnPostButtonClick()
    {
        GameManager.instance.PlaySE(pushButtonNum);
        if (isGameOver) return; // �Q�[���I����͉����s��Ȃ�

        string guessInput = guessNumberText.text;

        if (guessInput.Length != DIGIT_NUM || !int.TryParse(guessInput, out _))
        {
            explainText.text = "Please enter a valid 3 digit number.";
            return;
        }

        int[] guess = new int[DIGIT_NUM];
        for (int i = 0; i < guessInput.Length; i++)
        {
            guess[i] = int.Parse(guessInput[i].ToString());
        }

        var (hits, blows) = CheckGuess(guess);

        myHits = hits;
        myGuessListText.text += $"   {guessInput}      {hits}   {blows}\n";
        photonView.RPC("SyncResult", RpcTarget.OthersBuffered, guessInput, hits, blows); // ���̃v���C���[�Ɍ��ʂ����L

        

        if (hits == 3)
        {
            opponentTurnText.text = "";
            explainText.text = "���肪�^�[���ҋ@";
            objectToHide.SetActive(false);
            DisableAllButtons();
            
        }

        int myGuessCount = myGuessListText.text.Split('\n').Length;
        int opponentGuessCount = opponentGuessListText.text.Split('\n').Length;

        

        ResetGuessNumber();

        // ������ postButton �𖳌������A���̃v���C���[�� postButton ��L����
        postButton.gameObject.SetActive(false);
        opponentTurnText.text = "";
        photonView.RPC("EnableOtherPlayerPostButton", RpcTarget.Others); // ���v���C���[�̃{�^����L��������

        // �����̓����𑊎�Ɠ�������
        photonView.RPC("UpdateOpponentAnswer", RpcTarget.OthersBuffered, myAnswerText.text);

        if ((myHits == DIGIT_NUM || opponentHits == DIGIT_NUM) && myGuessCount == opponentGuessCount)
        {
            photonView.RPC("CheckWinCondition", RpcTarget.All);
        }
    }

    [PunRPC]
    void EnableOtherPlayerPostButton()
    {
        postButton.gameObject.SetActive(true);
        opponentTurnText.text = "";
    }

    [PunRPC]
    void SyncResult(string guessInput, int hits, int blows)
    {
        opponentGuessListText.text += $"   {guessInput}      {hits}   {blows}\n";
        opponentHits = hits;
    }

    [PunRPC]
    void GameEnd()
    {
        returnStartSceneButton.gameObject.SetActive(true);
    }

    [PunRPC]
    void DisableOpponentPostButton()
    {
        postButton.gameObject.SetActive(false);
    }

    [PunRPC]
    void CheckWinCondition()
    {
        DisableAllButtons();

        if (myHits > opponentHits)
        {
            explainText.text = "���Ȃ��̏����I";
            photonView.RPC("SetLoserText", RpcTarget.Others); // ����ɕ������b�Z�[�W��\��
        }
        else if (myHits < opponentHits)
        {
            explainText.text = "���Ȃ��̕����I";
            photonView.RPC("SetWinnerText", RpcTarget.Others); // ����ɏ������b�Z�[�W��\��
        }
        else
        {
            explainText.text = "���������I";
            photonView.RPC("SetDrawText", RpcTarget.Others); // �����������b�Z�[�W��\��
        }

        // �Q�[���I���������Ăяo��
        photonView.RPC("GameEnd", RpcTarget.All);
        
    }

    [PunRPC]
    void SetWinnerText()
    {
        // �������������ꍇ
        explainText.text = "���Ȃ��̏����I";
    }

    [PunRPC]
    void SetLoserText()
    {
        // ���肪�������ꍇ
        explainText.text = "���Ȃ��̕����I";
    }

    [PunRPC]
    void SetDrawText()
    {
        // ���������̏ꍇ
        explainText.text = "���������I";
    }

    void DisableAllButtons() //�{�^����񊈐��ɂ���
    {
        
        foreach (var button in numberButtons)
        {
            button.gameObject.SetActive(false);
        }

        backspaceButton.gameObject.SetActive(false);
        postButton.gameObject.SetActive(false);
    }

    

    void OnGenerateAnswerClick()
    {
        GameManager.instance.PlaySE(pushButtonNum);

        // myAnswerText.text�ɕۑ�
        myAnswerText.text = guessNumberText.text;
        explainText.text = "����̓��͂��I���܂őҋ@...";
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

    void OnReturnStartSceneClick()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.NickName = "������";
        SceneManager.LoadScene("01_StartScene");
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
        opponentTurnText.text = "";
        //postButton���쐬
        if (PhotonNetwork.IsMasterClient)
        {
            // �������}�X�^�[�̎��A�����̃|�X�g�{�^����L�����A����̃|�X�g�{�^���𖳌���
            postButton.gameObject.SetActive(true);
        }

        postButton.interactable = false;

        explainText.text = "����̐�����\�z";
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

    public (int hits, int blows) CheckGuess(int[] guess)
    {
        int hits = 0;
        int blows = 0;

        answer = new int[DIGIT_NUM];

        for (int i = 0; i < DIGIT_NUM; i++)
        {
            answer[i] = int.Parse(opponentAnswerText.text[i].ToString());
        }

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == answer[i])
            {
                hits++;
            }
            else if (answer.Contains(guess[i]))
            {
                blows++;
            }
        }

        guesses.Add(guess);
        return (hits, blows);
    }
}
