using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HitAndBlowGame : MonoBehaviour
{
    private int[] answer;
    private List<int[]> guesses;
    private System.Random random = new System.Random();

    public TextMeshProUGUI numberDisplay;
    public TextMeshProUGUI resultText;
    public Button postButton;

    private static int DIGIT_NUM = 3;

    void Start()
    {
        GenerateAnswer();
        guesses = new List<int[]>();
        postButton.onClick.AddListener(OnPostButtonClick);
        resultText.text = "";
    }

    void GenerateAnswer()
    {
        answer = new int[DIGIT_NUM];
        List<int> availableNumbers = new List<int>();

        // 0����9�܂ł̐��������X�g�ɒǉ�
        for (int i = 0; i <= 9; i++)
        {
            availableNumbers.Add(i);
        }

        // DIGIT_NUM ��J��Ԃ��ďd�����Ȃ�������I��
        for (int i = 0; i < answer.Length; i++)
        {
            int index = random.Next(availableNumbers.Count); // �g�p�\�Ȑ����̃��X�g���烉���_���ɃC���f�b�N�X���擾
            answer[i] = availableNumbers[index]; // �����_���ɑI�΂ꂽ�����𓚂��ɒǉ�
            availableNumbers.RemoveAt(index); // �I�΂ꂽ���������X�g����폜
        }

        Debug.Log("Answer: " + string.Join("", answer));
    }

    public (int hits, int blows) CheckGuess(int[] guess)
    {
        int hits = 0;
        int blows = 0;

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
        string guessInput = numberDisplay.text;

        //������DIGIT_NUM�łȂ��Ƃ��܂���int�^�ɕϊ��ł��Ȃ��Ƃ�
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

        if (hits == 3)
        {
            resultText.text += "\nIs the correct answer!";
        }
    }


    //���ݎg���Ă��Ȃ�
    public List<int[]> GetGuesses()
    {
        return guesses;
    }
}
