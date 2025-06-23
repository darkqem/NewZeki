using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idle_car : MonoBehaviour
{
    private int currentFrame = 0; // Текущий кадр анимации
    private float frameRate = 0.1f; // Скорость смены кадров
    private float frameTimer = 0f; // Таймер для анимации
    public Sprite[] idleSprites; // Массив для хранения спрайтов
    public SpriteRenderer spriteRenderer;
    public float animationSpeed = 1f;

    public int totalFrames = 76; // Количество кадров (зависит от того, сколько спрайтов у вас есть)
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Загружаем спрайты с названием "car_" и цифрой (например, car_1, car_2, ...)
        idleSprites = new Sprite[totalFrames];
        for (int i = 0; i < totalFrames; i++)
        {
            idleSprites[i] = Resources.Load<Sprite>($"sprites assets/car/52b39bc212244c2799c362fb03c746e1FgTxvkWXhpRxSV5R-{i}");
        }
    }

    void Update()
    {
        AnimateIdle();
    }

    public void AnimateIdle()
    {
        frameTimer += Time.deltaTime * animationSpeed;

        // Меняем кадр, если прошло достаточно времени
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f; // Сбрасываем таймер
            currentFrame = (currentFrame + 1) % idleSprites.Length; // Переходим к следующему кадру
            spriteRenderer.sprite = idleSprites[currentFrame]; // Устанавливаем текущий кадр
        }
    }
}
