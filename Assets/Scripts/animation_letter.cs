using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Не забудьте подключить UI для работы с кнопками

public class Animation_letter : MonoBehaviour
{
    private int currentFrame = 0; // Текущий кадр анимации
    private float frameRate = 0.1f; // Скорость смены кадров
    private float frameTimer = 0f; // Таймер для анимации
    public Sprite[] idleSprites;
    public SpriteRenderer spriteRenderer;
    public float animationSpeed = 1f;

    private bool isAnimationPlaying = true; // Флаг для контроля состояния анимации

    public Button actionButton; // Ссылка на кнопку, которая будет появляться

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Изначально скрываем кнопку
        if (actionButton != null)
        {
            actionButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isAnimationPlaying)
        {
            AnimateIdle();
        }
    }

    public void AnimateIdle()
    {
        frameTimer += Time.deltaTime * animationSpeed;

        // Меняем кадр, если прошло достаточно времени
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f; // Сбрасываем таймер
            currentFrame = (currentFrame + 1) % idleSprites.Length; // Переходим к следующему кадру

            // Если достигли последнего кадра, останавливаем анимацию и показываем кнопку
            if (currentFrame == idleSprites.Length - 1)
            {
                isAnimationPlaying = false; // Останавливаем анимацию
                ShowButton(); // Показываем кнопку
            }

            spriteRenderer.sprite = idleSprites[currentFrame]; // Устанавливаем текущий кадр
        }
    }

    // Функция для показа кнопки
    public void ShowButton()
    {
        if (actionButton != null)
        {
            actionButton.gameObject.SetActive(true); // Включаем кнопку
        }
    }
}
