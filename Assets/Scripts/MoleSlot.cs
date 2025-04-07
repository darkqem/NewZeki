using UnityEngine;

public class MoleSlot : MonoBehaviour
{
    public Light roomLight; // Назначается вручную в инспекторе
    private int hp = 0;
    public bool lightActivated = false;
    public bool wasLightOffAfterActivation = false;

    public bool IsActive => hp > 0;

    void Start()
    {
        // Получаем дочерний объект с компонентом Light
        roomLight = GetComponentInChildren<Light>();

        if (roomLight != null)
            roomLight.enabled = false;
    }

    public void SpawnMole()
    {
        if (!IsActive && !lightActivated) // Проверка, не активирован ли свет
        {
            hp = 1; // Случайное количество HP от 3 до 10
            roomLight.enabled = true;
            lightActivated = true; // Помечаем, что свет активирован
        }
    }

    public void Hit()
    {
        if (!IsActive) return;

        hp--;
        if (hp <= 0)
        {
            roomLight.enabled = false;
            wasLightOffAfterActivation = true;
        }
    }
}
