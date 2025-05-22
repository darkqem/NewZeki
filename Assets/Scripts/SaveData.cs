using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int currentDay;
    public DateTime lastSaveTime;

    public SaveData()
    {
        currentDay = 1;
        lastSaveTime = DateTime.Now;
    }
} 