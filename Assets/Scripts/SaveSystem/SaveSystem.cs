using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string Path => Application.persistentDataPath + "/savedprogress.json";

    public static void Save(Data data)
    {
        Debug.Log(Path);
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(Path, json);
    }

    public static Data Load()
    {
        if (!File.Exists(Path))
        {
            Debug.Log("Save file not found");
            return null;
        }

        string json = File.ReadAllText(Path);

        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("Save file is empty");
            return null;
        }

        return JsonUtility.FromJson<Data>(json);
    }
}
