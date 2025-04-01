using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Idle : MonoBehaviour
{
    private int currentFrame = 0; // Текущий кадр анимации
    public float frameRate = 0.1f; // Скорость смены кадров
    private float frameTimer = 0f; // Таймер для анимации
    private float timefreezed = 0.5f;
    public Sprite[] idleSprites;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateIdle();
    }

    public void AnimateIdle()
    {
        frameTimer += Time.deltaTime;

        // Меняем кадр, если прошло достаточно времени
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f; // Сбрасываем таймер
            currentFrame = (currentFrame + 1) % idleSprites.Length; // Переходим к следующему кадру
            spriteRenderer.sprite = idleSprites[currentFrame]; // Устанавливаем текущий кадр
        }
    }
}
