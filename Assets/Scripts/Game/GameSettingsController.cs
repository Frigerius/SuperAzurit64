using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

class GameSettingsController
{
    private static object syncRoot = new object();
    private static volatile GameSettingsController itsMe;
    private GameSettings _userSettings;
    private GameSettings _currentSettings;
    private bool _isRanked;
    private bool _isHardMode;
    private int _version = 2;
    private int _level;

    private GameSettingsController()
    {
        _userSettings = new GameSettings();
        _userSettings = GetRessourceSettings("nmSettings");
    }

    public static GameSettingsController Instance
    {
        get
        {
            if (itsMe == null)
            {
                lock (syncRoot)
                {
                    if (itsMe == null) itsMe = new GameSettingsController();
                }
            }
            return itsMe;
        }
    }

    public void LoadUserSettings(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
        if (File.Exists(path))
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                try
                {
                    _userSettings = serializer.Deserialize(fs) as GameSettings;
                }catch(Exception)
                {
                    _userSettings = new GameSettings();
                }
            }
        }
        else
        {
            _userSettings = new GameSettings();
        }
    }

    public void LoadRessourceSettings(string path)
    {
        _userSettings = GetRessourceSettings(path);
    }


    private GameSettings GetRessourceSettings(string filename)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
        TextAsset hmFile = Resources.Load(filename) as TextAsset;
        GameSettings settings;
        using (TextReader reader = new StringReader(hmFile.text))
        {
            settings = serializer.Deserialize(reader) as GameSettings;
        }
        return settings;
    }

    public void SaveUserSettings(string name)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SuperAzurit64/AISettings");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
        using (FileStream fs = new FileStream(Path.Combine(path, name + ".xml"), FileMode.OpenOrCreate))
        {
            serializer.Serialize(fs, _userSettings);
        }
    }

    public GameSettings CurrentSettings
    {
        get { return _currentSettings; }
    }

    public GameSettings UserSettings
    {
        get { return _userSettings; }
    }

    public bool IsHardMode
    {
        get { return _isHardMode; }
        set { _isHardMode = value; }
    }

    public bool IsRanked
    {
        get { return _isRanked; }
        set { _isRanked = value; }
    }

    public string ScoreboardId
    {
        get { return string.Format("{0}_{1:000}_{2:000}", _isHardMode ? "HM" : "NM", _version, _level); }
    }

    public void RefreshCurrentSettings(int level)
    {
        _level = level;
        if (_isHardMode)
        {
            _currentSettings = GetRessourceSettings("hmSettings");
        }
        else if (_isRanked)
        {
            _currentSettings = GetRessourceSettings("nmSettings");
        }
        _currentSettings = _userSettings;
    }
}

