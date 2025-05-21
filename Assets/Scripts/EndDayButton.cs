using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EndDayButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnEndDayClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnEndDayClick);
    }

    private void OnEndDayClick()
    {
        GameManager.Instance.EndDay();
    }
} 