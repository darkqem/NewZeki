using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform inventoryPanel; // Основная панель инвентаря
    public Button inventoryButton; // Кнопка открытия/закрытия инвентаря
    public RectTransform[] inventorySlots; // Массив слотов инвентаря

    [Header("Animation Settings")]
    public float animationDuration = 0.5f; // Длительность анимации
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Кривая анимации
    
    // Целевые координаты для открытой панели
    public Vector2 targetPosition = new Vector2(-250.6655f, -482.62f);
    
    private bool isInventoryOpen = false;
    private float currentAnimationTime;
    private bool isAnimating = false;
    private string saveFilePath;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Vector2 originalSize;
    private Vector2 collapsedSize;

    [Serializable]
    private class InventoryData
    {
        public bool isOpen;
    }

    void Start()
    {
        // Скрываем инвентарь изначально
        inventoryPanel.gameObject.SetActive(false);
        
        // Сохраняем оригинальный размер панели
        originalSize = inventoryPanel.sizeDelta;
        // Устанавливаем сжатый размер (равный размеру кнопки)
        collapsedSize = inventoryButton.GetComponent<RectTransform>().sizeDelta;
        
        // Добавляем слушатель на кнопку
        if (inventoryButton != null)
        {
            inventoryButton.onClick.AddListener(ToggleInventory);
        }

        // Путь для сохранения данных
        saveFilePath = Path.Combine(Application.persistentDataPath, "inventory.json");
        
        // Загружаем сохраненное состояние
        LoadInventoryState();

        // Рассчитываем позиции
        CalculatePositions();
    }

    private void CalculatePositions()
    {
        // Получаем позицию кнопки в координатах якоря
        startPosition = inventoryButton.GetComponent<RectTransform>().anchoredPosition;
        endPosition = targetPosition;
    }

    void Update()
    {
        if (isAnimating)
        {
            currentAnimationTime += Time.deltaTime;
            float normalizedTime = currentAnimationTime / animationDuration;
            float curveValue = animationCurve.Evaluate(normalizedTime);

            if (isInventoryOpen)
            {
                // Анимация открытия
                inventoryPanel.anchoredPosition = Vector2.Lerp(startPosition, endPosition, curveValue);
                // Анимация размера
                inventoryPanel.sizeDelta = Vector2.Lerp(collapsedSize, originalSize, curveValue);
            }
            else
            {
                // Анимация закрытия
                inventoryPanel.anchoredPosition = Vector2.Lerp(endPosition, startPosition, curveValue);
                // Анимация размера
                inventoryPanel.sizeDelta = Vector2.Lerp(originalSize, collapsedSize, curveValue);
            }

            // Проверяем завершение анимации
            if (normalizedTime >= 1f)
            {
                isAnimating = false;
                if (!isInventoryOpen)
                {
                    inventoryPanel.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        
        if (isInventoryOpen)
        {
            inventoryPanel.gameObject.SetActive(true);
            // Обновляем позиции на случай, если кнопка переместилась
            CalculatePositions();
            // Устанавливаем начальную позицию и размер
            inventoryPanel.anchoredPosition = startPosition;
            inventoryPanel.sizeDelta = collapsedSize;
        }

        // Запускаем анимацию
        currentAnimationTime = 0f;
        isAnimating = true;
        
        SaveInventoryState();
    }

    private void SaveInventoryState()
    {
        InventoryData data = new InventoryData
        {
            isOpen = isInventoryOpen
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadInventoryState()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);
            isInventoryOpen = data.isOpen;
            
            if (isInventoryOpen)
            {
                inventoryPanel.gameObject.SetActive(true);
                CalculatePositions();
                inventoryPanel.anchoredPosition = endPosition;
                inventoryPanel.sizeDelta = originalSize;
            }
        }
    }
} 