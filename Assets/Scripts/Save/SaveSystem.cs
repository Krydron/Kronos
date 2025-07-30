using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
public static class SaveSystem
{
    public static void NewSaveSlot(GameObject gameManager)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.dataPath + "/save.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(gameManager.GetComponent<LoopTracker>(), gameManager.GetComponent<MapSave>(), gameManager.GetComponent<SceneLoader>(), gameManager.GetComponent<NoteSave>());

        binaryFormatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadSave()
    {
        string path = Application.dataPath + "/save.save";
        if (!File.Exists(path)) { Debug.LogError("Save not found: "+path); return null; }
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = binaryFormatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        catch { return null; }
    }

    public static void SettingsSave(GameObject gameManager)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.dataPath + "/settings.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        SettingsData data = new SettingsData(gameManager.GetComponent<SettimgsTracker>());

        binaryFormatter.Serialize(stream, data);
        stream.Close();
    }
    public static SettingsData SettingsLoad()
    {
        string path = Application.dataPath + "/settings.save";
        if (!File.Exists(path)) { Debug.LogError("Save not found: " + path); return null; }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        SettingsData data = binaryFormatter.Deserialize(stream) as SettingsData;
        stream.Close();

        return data;
    }
}
