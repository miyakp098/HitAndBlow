using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Matching : MonoBehaviour
{
    public TextMeshProUGUI welcomeText;

    void Start()
    {
        // PlayerPrefs���烆�[�U�[�����擾
        string username = PlayerPrefs.GetString("Username", "Guest");
        welcomeText.text = $"Welcome, {username}!";
    }
}
