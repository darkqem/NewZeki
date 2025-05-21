using System;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int currentDay;
    public bool dialogShownToday;
    public int lastShownDialogIndex;

    public GameSaveData()
    {
        currentDay = 1;
        dialogShownToday = false;
        lastShownDialogIndex = -1;
    }
} 