using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Matching : MonoBehaviour
{
    public TextMeshProUGUI welcomeText;

    void Start()
    {
        // PlayerPrefs‚©‚çƒ†[ƒU[–¼‚ğæ“¾
        string username = PlayerPrefs.GetString("Username", "Guest");
        welcomeText.text = $"Welcome, {username}!";
    }
}
