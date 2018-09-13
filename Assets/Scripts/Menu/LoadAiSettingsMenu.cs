using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadAiSettingsMenu : MonoBehaviour
{

    List<Button> _buttonList;
    [SerializeField]
    Button _buttonPrefab;
    [SerializeField]
    MainMenu _mainMenu;
    [SerializeField]
    RectTransform _buttonRoot;
    [SerializeField]
    InputField _saveSettingsName;
    [SerializeField]
    Canvas _fileExistsDialog;
    Canvas _canvas;
    string _defaultPath;

    // Use this for initialization
    void Start()
    {
        _buttonList = new List<Button>();
        _canvas = GetComponent<Canvas>();
        _defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SuperAzurit64/AISettings");

    }

    public void RefreshList()
    {
        foreach (Button b in _buttonList)
        {
            Destroy(b.gameObject);
        }
        _buttonList.Clear();
        //Add static Settings
        CreateRessourceButton("NormalMode", "nmSettings");
        CreateRessourceButton("HardMode", "hmSettings");
        if (Directory.Exists(_defaultPath))
        {
            foreach (string fileDir in Directory.GetFiles(_defaultPath))
            {
                CreateUserSettingsButton(Path.GetFileNameWithoutExtension(fileDir), fileDir);
            }
        }
    }

    private void CreateUserSettingsButton(string name, string path)
    {
        Button button = Instantiate(_buttonPrefab);
        _buttonList.Add(button);
        button.GetComponentInChildren<Text>().text = name;
        button.onClick.AddListener(() =>
        {
            GameSettingsController.Instance.LoadUserSettings(path);
            _mainMenu.LoadSettings();
            _canvas.enabled = false;
        });
        button.GetComponent<RectTransform>().SetParent(_buttonRoot);
    }

    private void CreateRessourceButton(string name, string path)
    {
        Button button = Instantiate(_buttonPrefab);
        _buttonList.Add(button);
        button.GetComponentInChildren<Text>().text = name;
        button.onClick.AddListener(() =>
        {
            GameSettingsController.Instance.LoadRessourceSettings(path);
            _mainMenu.LoadSettings();
            _canvas.enabled = false;
        });
        button.GetComponent<RectTransform>().SetParent(_buttonRoot);
    }

    public void SaveSettings(bool force = false)
    {
        string fileName = _saveSettingsName.text;
        if (!force && File.Exists(Path.Combine(_defaultPath, fileName + ".xml")))
        {
            _fileExistsDialog.enabled = true;
        }
        else
        {
            GameSettingsController.Instance.SaveUserSettings(fileName);
        }
    }
}
