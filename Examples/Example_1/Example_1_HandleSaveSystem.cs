using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Example_1_HandleSaveSystem : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _saveLocalText;
    [SerializeField]
    private TextMeshProUGUI _lastSaveTimeText;
    
    [SerializeField]
    private Button _saveButton;
    [SerializeField]
    private Button _loadButton;
    [SerializeField]
    private Button _deleteButton;

    private void Awake()
    {
        _saveButton.onClick.AddListener(HandleSaveButtonClicked);
        _loadButton.onClick.AddListener(HandleLoadButtonClicked);
        _deleteButton.onClick.AddListener(HandleDeleteButtonClicked);

        SaveSystem.OnSaveLoaded += HandleOnSaveLoaded;
    }
    

    private void HandleOnSaveLoaded(SaveData savedata)
    {
        _saveLocalText.text = savedata.saveFileLocation;
        _lastSaveTimeText.text = savedata.lastSavedTime;
    }

    private void HandleDeleteButtonClicked()
    {
        SaveSystem.ClearSavedData();
    }

    private void HandleLoadButtonClicked()
    {
        SaveSystem.LoadGame();
        
    }

    private void HandleSaveButtonClicked()
    {
        SaveSystem.SaveGame();
    }

    private void OnDisable()
    {
        _saveButton.onClick.RemoveListener(HandleSaveButtonClicked);
        _loadButton.onClick.RemoveListener(HandleLoadButtonClicked);
        _deleteButton.onClick.RemoveListener(HandleDeleteButtonClicked);
    }
}
