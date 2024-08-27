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

    public TextMeshProUGUI playerAnswerText; // プレイヤーの答えを表示するTextMeshProUGUI
    public TextMeshProUGUI opponentAnswerText; // 相手の答えを表示するTextMeshProUGUI

    private static int DIGIT_NUM = 3;
    private bool isGameOver = false; // ゲーム終了フラグ

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
        if (isGameOver) return; // ゲーム終了後は何も行わない

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

        photonView.RPC("SyncResult", RpcTarget.Others, hits, blows); // 他のプレイヤーに結果を共有

        if (hits == 3)
        {
            resultText.text += "\nIs the correct answer!";
            isGameOver = true; // ゲーム終了フラグをセット
            photonView.RPC("GameOver", RpcTarget.All); // ゲーム終了を他のプレイヤーに通知
        }
    }

    [PunRPC]
    void SyncResult(int hits, int blows)
    {
        resultText.text = $"Opponent's Guess: Hits: {hits}, Blows: {blows}";
    }

    [PunRPC]
    void GameOver()
    {
        isGameOver = true;
        postButton.interactable = false; // ゲーム終了時にボタンを無効化
    }

    public List<int[]> GetGuesses()
    {
        return guesses;
    }
}
