using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

[CustomEditor(typeof(GameSettingsXmlTester))]
public class GameSettingsXmlTesterEditor : Editor
{

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Create"))
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SuperAzurit64/AISettings");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
            GameSettings settings = new GameSettings();
            using (FileStream fs = new FileStream(Path.Combine(path, "test.xml"), FileMode.OpenOrCreate))
            {
                serializer.Serialize(fs, settings);
            }
            //NM
            using (FileStream fs = new FileStream(Path.Combine(path, "nmSettings.xml"), FileMode.OpenOrCreate))
            {
                settings.LoadRankedSettings();
                serializer.Serialize(fs, settings);
            }
            //HM
            using (FileStream fs = new FileStream(Path.Combine(path, "hmSettings.xml"), FileMode.OpenOrCreate))
            {
                settings.LoadHardModeSettings();
                serializer.Serialize(fs, settings);
            }
        }

    }
}
