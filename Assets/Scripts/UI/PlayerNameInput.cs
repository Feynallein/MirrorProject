using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour {
    [Header("UI")]
    [SerializeField] TMP_InputField _nameInputField;
    [SerializeField] Button _continueButton;

    public static string DisplayName { get; private set; }

    const string _PlayerPrefsNameKey = "PlayerName";

    void Start() {
        SetUpInputField();
    }

    void SetUpInputField() {
        if (!PlayerPrefs.HasKey(_PlayerPrefsNameKey)) return;
        string defaultName = PlayerPrefs.GetString(_PlayerPrefsNameKey);
        _nameInputField.text = defaultName;
        SetPlayerName(defaultName);
    }
    
    public void SetPlayerName(string name) {
        _continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName() {
        DisplayName = _nameInputField.text;
        PlayerPrefs.SetString(_PlayerPrefsNameKey, DisplayName);
    }
}
