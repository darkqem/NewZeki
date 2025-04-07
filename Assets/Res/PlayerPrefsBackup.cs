using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerPrefsBackup : MonoBehaviour
{
    [System.Serializable]
    public class PrefData
    {
        public string key;
        public string type;
        public string value;
    }

    [System.Serializable]
    public class PrefDataList
    {
        public List<PrefData> prefs = new List<PrefData>();
    }

    // Здесь укажи все свои ключи
    private string[] allKeys = new string[]
    {
        "playerName", "score", "volume", "isSoundOn"
    };

    [ContextMenu("Backup PlayerPrefs to File")]
    public void BackupPlayerPrefs()
    {
        var dataList = new PrefDataList();

        foreach (string key in allKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PrefData data = new PrefData();
                data.key = key;

                // Пробуем понять тип значения
                if (PlayerPrefs.GetInt(key, int.MinValue) != int.MinValue)
                {
                    data.type = "int";
                    data.value = PlayerPrefs.GetInt(key).ToString();
                }
                else if (PlayerPrefs.GetFloat(key, float.MinValue + 1) != float.MinValue + 1)
                {
                    data.type = "float";
                    data.value = PlayerPrefs.GetFloat(key).ToString();
                }
                else
                {
                    data.type = "string";
                    data.value = PlayerPrefs.GetString(key);
                }

                dataList.prefs.Add(data);
            }
        }

        string json = JsonUtility.ToJson(dataList, true);
        string path = Path.Combine(Application.dataPath, "PlayerPrefsBackup.json");
        File.WriteAllText(path, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log($"PlayerPrefs backup saved to: {path}");
    }
}
