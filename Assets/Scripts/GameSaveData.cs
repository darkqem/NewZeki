using System;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    // Данные о днях
    public int currentDay = 1;
    public bool dialogShownToday = false;
    public int lastShownDialogIndex = -1;
    
    // Конструктор для создания нового сохранения с начальными значениями
    public GameSaveData()
    {
        currentDay = 1;
        dialogShownToday = false;
        lastShownDialogIndex = -1;
    }
} 