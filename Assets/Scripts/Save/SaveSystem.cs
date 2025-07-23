using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
public static class SaveSystem
{
    public static void NewSaveSlot()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.dataPath + "/Kronos/save.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData();

        binaryFormatter.Serialize(stream, data);
        stream.Close();
    }

    public static void LoadSave()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.dataPath + "/Kronos/save.save";
        FileStream stream = new FileStream(path, FileMode.Open);

        SaveData data = binaryFormatter.Deserialize(stream) as SaveData;
        stream.Close();
    }
}
