using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;

[Serializable]
public class InventoryItem
{
    public string itemId;
    public string itemName;
    public Sprite itemIcon;
    
    public InventoryItem(string id, string name, Sprite icon)
    {
        itemId = id;
        itemName = name;
        itemIcon = icon;
    }
}

public class InventoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform inventoryPanel; // Основная панель инвентаря
    public Button inventoryButton; // Кнопка открытия/закрытия инвентаря
    public RectTransform[] inventorySlots; // Массив слотов инвентаря

    [Header("Inventory Settings")]
    public int maxSlots = 20; // Максимальное количество слотов
    public float itemScale = 1f; // Масштаб для спрайтов предметов
    
    [Header("Animation Settings")]
    public float animationDuration = 0.5f; // Длительность анимации
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Кривая анимации
    
    // Целевые координаты для открытой панели
    public Vector2 targetPosition = new Vector2(-250.6655f, -482.62f);
    
    private Dictionary<int, InventoryItem> inventoryItems; // Словарь для хранения предметов
    private Dictionary<int, GameObject> itemObjects; // Хранение GameObject'ов предметов
    private bool isInventoryOpen = false;
    private float currentAnimationTime;
    private bool isAnimating = false;
    private string saveFilePath;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Vector2 originalSize;
    private Vector2 collapsedSize;

    // Флаг, указывающий, есть ли UI инвентаря на текущей сцене
    private bool hasInventoryUI = false;

    // Singleton instance для легкого доступа из других скриптов
    public static InventoryManager Instance { get; private set; }

    [Serializable]
    private class InventoryData
    {
        public bool isOpen;
        public List<SerializableInventoryItem> items = new List<SerializableInventoryItem>();
    }

    [Serializable]
    private class SerializableInventoryItem
    {
        public string itemId;
        public string itemName;
        public int slotIndex;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            inventoryItems = new Dictionary<int, InventoryItem>();
            itemObjects = new Dictionary<int, GameObject>();
            saveFilePath = Path.Combine(Application.persistentDataPath, "inventory.json");
            LoadInventoryState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupInventoryUI();
    }

    // Вызывать при загрузке сцены с UI инвентаря
    public void SetupInventoryUI()
    {
        // Сбрасываем флаг UI перед проверкой
        hasInventoryUI = false;

        // Очищаем старые GameObject'ы
        foreach (var itemObj in itemObjects.Values)
        {
            if (itemObj != null)
            {
                Destroy(itemObj);
            }
        }
        itemObjects.Clear();

        // Проверяем, что все UI элементы существуют и действительны
        if (inventoryPanel == null || inventoryButton == null || inventorySlots == null)
        {
            return;
        }

        // Проверяем, что панель и кнопка все еще существуют (не были уничтожены)
        if (!inventoryPanel.gameObject || !inventoryButton.gameObject)
        {
            return;
        }

        // Проверяем слоты
        bool allSlotsValid = true;
        foreach (var slot in inventorySlots)
        {
            if (slot == null || !slot.gameObject)
            {
                allSlotsValid = false;
                break;
            }
        }

        if (!allSlotsValid || inventorySlots.Length == 0)
        {
            return;
        }

        // Если все проверки пройдены, включаем UI
        hasInventoryUI = true;
        
        // Настройка UI
        inventoryPanel.gameObject.SetActive(false);
        originalSize = inventoryPanel.sizeDelta;
        collapsedSize = inventoryButton.GetComponent<RectTransform>().sizeDelta;
        
        inventoryButton.onClick.RemoveAllListeners(); // Очищаем старые слушатели
        inventoryButton.onClick.AddListener(ToggleInventory);

        // Обновляем отображение всех предметов
        foreach (var kvp in inventoryItems)
        {
            UpdateSlotUI(kvp.Key);
        }

        CalculatePositions();
    }

    private void CalculatePositions()
    {
        if (!hasInventoryUI || inventoryButton == null || !inventoryButton.gameObject)
        {
            return;
        }

        startPosition = inventoryButton.GetComponent<RectTransform>().anchoredPosition;
        endPosition = targetPosition;
    }

    void Update()
    {
        if (!hasInventoryUI || !isAnimating || inventoryPanel == null || !inventoryPanel.gameObject)
        {
            return;
        }

        currentAnimationTime += Time.deltaTime;
        float normalizedTime = currentAnimationTime / animationDuration;
        float curveValue = animationCurve.Evaluate(normalizedTime);

        if (isInventoryOpen)
        {
            inventoryPanel.anchoredPosition = Vector2.Lerp(startPosition, endPosition, curveValue);
            inventoryPanel.sizeDelta = Vector2.Lerp(collapsedSize, originalSize, curveValue);
        }
        else
        {
            inventoryPanel.anchoredPosition = Vector2.Lerp(endPosition, startPosition, curveValue);
            inventoryPanel.sizeDelta = Vector2.Lerp(originalSize, collapsedSize, curveValue);
        }

        if (normalizedTime >= 1f)
        {
            isAnimating = false;
            if (!isInventoryOpen && inventoryPanel != null && inventoryPanel.gameObject)
            {
                inventoryPanel.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleInventory()
    {
        // Проверяем наличие UI перед выполнением операций
        if (!hasInventoryUI || inventoryPanel == null || !inventoryPanel.gameObject)
        {
            return;
        }

        isInventoryOpen = !isInventoryOpen;
        
        if (isInventoryOpen)
        {
            inventoryPanel.gameObject.SetActive(true);
            CalculatePositions();
            inventoryPanel.anchoredPosition = startPosition;
            inventoryPanel.sizeDelta = collapsedSize;
        }

        currentAnimationTime = 0f;
        isAnimating = true;
        
        SaveInventoryState();
    }

    // Метод для добавления предмета в инвентарь
    public bool AddItem(InventoryItem item)
    {
        // Ищем первый свободный слот
        for (int i = 0; i < maxSlots; i++)
        {
            if (!inventoryItems.ContainsKey(i))
            {
                inventoryItems[i] = item;
                if (hasInventoryUI)
                {
                    UpdateSlotUI(i);
                }
                SaveInventoryState();
                return true;
            }
        }

        Debug.LogWarning("Inventory is full!");
        return false;
    }

    // Метод для удаления предмета из инвентаря
    public bool RemoveItem(string itemId)
    {
        foreach (var kvp in inventoryItems)
        {
            if (kvp.Value.itemId == itemId)
            {
                inventoryItems.Remove(kvp.Key);
                UpdateSlotUI(kvp.Key);
                SaveInventoryState();
                return true;
            }
        }
        return false;
    }

    // Метод для обновления UI слота
    private void UpdateSlotUI(int slotIndex)
    {
        // Проверяем наличие UI и валидность индекса
        if (!hasInventoryUI || slotIndex >= inventorySlots.Length)
        {
            return;
        }

        // Проверяем существование слота
        RectTransform slotTransform = inventorySlots[slotIndex];
        if (slotTransform == null || !slotTransform.gameObject)
        {
            return;
        }

        // Удаляем старый GameObject предмета, если он существует
        if (itemObjects.ContainsKey(slotIndex))
        {
            if (itemObjects[slotIndex] != null)
            {
                Destroy(itemObjects[slotIndex]);
            }
            itemObjects.Remove(slotIndex);
        }

        if (inventoryItems.TryGetValue(slotIndex, out InventoryItem item))
        {
            // Создаем новый GameObject для предмета
            GameObject itemObject = new GameObject($"Item_{item.itemId}");
            itemObject.transform.SetParent(slotTransform);
            
            // Добавляем SpriteRenderer
            SpriteRenderer spriteRenderer = itemObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.itemIcon;
            spriteRenderer.sortingOrder = 1; // Чтобы быть поверх слота
            
            // Настраиваем позицию и масштаб
            itemObject.transform.localPosition = Vector3.zero;
            
            // Подгоняем размер спрайта под размер слота
            float slotWidth = slotTransform.rect.width;
            float slotHeight = slotTransform.rect.height;
            Vector2 spriteSize = item.itemIcon.bounds.size;
            
            float scaleX = slotWidth / spriteSize.x;
            float scaleY = slotHeight / spriteSize.y;
            float scale = Mathf.Min(scaleX, scaleY) * itemScale;
            
            itemObject.transform.localScale = new Vector3(scale, scale, 1f);
            
            // Сохраняем ссылку на GameObject
            itemObjects[slotIndex] = itemObject;
        }
    }

    private void SaveInventoryState()
    {
        InventoryData data = new InventoryData
        {
            isOpen = isInventoryOpen
        };

        foreach (var kvp in inventoryItems)
        {
            data.items.Add(new SerializableInventoryItem
            {
                itemId = kvp.Value.itemId,
                itemName = kvp.Value.itemName,
                slotIndex = kvp.Key
            });
        }

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
            
            foreach (var itemData in data.items)
            {
                // Загружаем спрайт предмета только из папки Items
                Sprite itemSprite = Resources.Load<Sprite>("Items/" + itemData.itemId);
                
                // Пробуем загрузить спрайт с разными расширениями
                if (itemSprite == null)
                {
                    string[] extensions = { "", ".png", ".jpg", ".jpeg" };
                    foreach (string ext in extensions)
                    {
                        itemSprite = Resources.Load<Sprite>("Items/" + itemData.itemId + ext);
                        if (itemSprite != null) break;
                    }

                    if (itemSprite == null)
                    {
                        Debug.LogWarning($"Не удалось найти спрайт для предмета {itemData.itemId} в папке Resources/Items/");
                        continue; // Пропускаем этот предмет
                    }
                }

                InventoryItem item = new InventoryItem(itemData.itemId, itemData.itemName, itemSprite);
                inventoryItems[itemData.slotIndex] = item;
                if (hasInventoryUI)
                {
                    UpdateSlotUI(itemData.slotIndex);
                }
            }
        }
    }

    // Метод для добавления предмета через диалоговую систему
    public void AddItemFromDialog(string itemId, string itemName)
    {
        // Загружаем спрайт предмета только из папки Items
        Sprite itemSprite = Resources.Load<Sprite>("Items/" + itemId);
        
        if (itemSprite == null)
        {
            // Пробуем загрузить спрайт с разными расширениями
            string[] extensions = { "", ".png", ".jpg", ".jpeg" };
            foreach (string ext in extensions)
            {
                itemSprite = Resources.Load<Sprite>("Items/" + itemId + ext);
                if (itemSprite != null) break;
            }

            if (itemSprite == null)
            {
                Debug.LogError($"Не удалось найти спрайт для предмета {itemId} в папке Resources/Items/");
                return;
            }
        }

        InventoryItem newItem = new InventoryItem(itemId, itemName, itemSprite);
        
        if (AddItem(newItem))
        {
            Debug.Log($"Предмет {itemName} успешно добавлен в инвентарь. Используется спрайт: {itemSprite.name}");
        }
        else
        {
            Debug.LogWarning($"Не удалось добавить предмет {itemName} - инвентарь полон");
        }
    }

    private void OnDestroy()
    {
        // Очищаем все созданные GameObject'ы при уничтожении менеджера
        foreach (var itemObj in itemObjects.Values)
        {
            if (itemObj != null)
            {
                Destroy(itemObj);
            }
        }
        itemObjects.Clear();
    }
} 