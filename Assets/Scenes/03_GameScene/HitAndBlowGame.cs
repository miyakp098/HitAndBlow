using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

public class HitAndBlowGame : MonoBehaviourPun
{
    private int[] answer;
    private List<int[]> guesses;

    public TextMeshProUGUI numberDisplay;
    public TextMeshProUGUI resultText;
    public Button postButton;

    public TextMeshProUGUI playerAnswerText; // �����̓���
    public TextMeshProUGUI opponentAnswerText; // ����̓���

    public TextMeshProUGUI playerGuessListText;
    public TextMeshProUGUI opponentGuessListText;

    private static int DIGIT_NUM = 3;
    private bool isGameOver = false; // �Q�[���I���t���O

    void Start()
    {
        guesses = new List<int[]>();
        postButton.onClick.AddListener(OnPostButtonClick);
        resultText.text = "";
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

    void OnPostButtonClick()
    {
        if (isGameOver) return; // �Q�[���I����͉����s��Ȃ�

        string guessInput = numberDisplay.text;

        if (guessInput.Length != DIGIT_NUM || !int.TryParse(guessInput, out _))
        {
            resultText.text = "Please enter a valid 3 digit number.";
            return;
        }

        

        int[] guess = new int[DIGIT_NUM];
        for (int i = 0; i < guessInput.Length; i++)
        {
            guess[i] = int.Parse(guessInput[i].ToString());
        }

        var (hits, blows) = CheckGuess(guess);
        resultText.text = $"Hits: {hits}, Blows: {blows}";

        playerGuessListText.text += $"{numberDisplay.text} : Hits:{hits}, Blows:{blows}\n";

        photonView.RPC("SyncResult", RpcTarget.OthersBuffered, guessInput, hits, blows); // ���̃v���C���[�Ɍ��ʂ����L

        if (hits == 3)
        {
            resultText.text += "\nIs the correct answer!";
            isGameOver = true; // �Q�[���I���t���O���Z�b�g
            photonView.RPC("GameOver", RpcTarget.All); // �Q�[���I���𑼂̃v���C���[�ɒʒm
        }
    }

    [PunRPC]
    void SyncResult(string guessInput, int hits, int blows)
    {
        opponentGuessListText.text += $"{guessInput} : Hits:{hits}, Blows:{blows}\n";
    }

    [PunRPC]
    void GameOver()
    {
        isGameOver = true;
        postButton.interactable = false; // �Q�[���I�����Ƀ{�^���𖳌���
    }

    public List<int[]> GetGuesses()
    {
        return guesses;
    }
}
