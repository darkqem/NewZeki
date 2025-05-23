using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WireTask : MonoBehaviour
{
    [System.Serializable]
    public class WireConnection
    {
        public Image leftConnector;
        public Image rightConnector;
        public Color wireColor;
        public bool isConnected;
    }

    [Header("Wire Settings")]
    [SerializeField] private List<WireConnection> wireConnections = new List<WireConnection>();
    [SerializeField] private LineRenderer wirePrefab;
    [SerializeField] private float wireWidth = 0.2f;
    
    [Header("Game Settings")]
    [SerializeField] private bool randomizeColors = true;
    
    private WireConnection selectedWire;
    private LineRenderer currentLine;
    private Camera mainCamera;
    private bool isDragging = false;
    private int completedConnections = 0;

    private void Start()
    {
        mainCamera = Camera.main;
        // Устанавливаем начальные цвета с полной непрозрачностью
        foreach (var wire in wireConnections)
        {
            wire.wireColor = new Color(wire.wireColor.r, wire.wireColor.g, wire.wireColor.b, 1f);
        }
        
        if (randomizeColors)
        {
            ShuffleWireColors();
        }
        InitializeWires();
    }

    private void InitializeWires()
    {
        foreach (var wire in wireConnections)
        {
            wire.isConnected = false;
            // Устанавливаем цвет с полной непрозрачностью
            Color leftColor = new Color(wire.wireColor.r, wire.wireColor.g, wire.wireColor.b, 1f);
            Color rightColor = wire.rightConnector.color;
            rightColor.a = 1f;
            
            wire.leftConnector.color = leftColor;
            wire.rightConnector.color = rightColor;
        }
    }

    private void ShuffleWireColors()
    {
        List<Color> colors = new List<Color>();
        foreach (var wire in wireConnections)
        {
            // Добавляем цвета с полной непрозрачностью
            colors.Add(new Color(wire.wireColor.r, wire.wireColor.g, wire.wireColor.b, 1f));
        }

        for (int i = colors.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Color temp = colors[i];
            colors[i] = colors[randomIndex];
            colors[randomIndex] = temp;
        }

        for (int i = 0; i < wireConnections.Count; i++)
        {
            // Устанавливаем цвет правого коннектора с полной непрозрачностью
            Color newColor = new Color(colors[i].r, colors[i].g, colors[i].b, 1f);
            wireConnections[i].rightConnector.color = newColor;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnStartDrag();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            OnDrag();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            OnEndDrag();
        }
    }

    private void OnStartDrag()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Image connector = hit.collider.GetComponent<Image>();
            if (connector != null)
            {
                selectedWire = wireConnections.Find(w => w.leftConnector == connector && !w.isConnected);
                if (selectedWire != null)
                {
                    isDragging = true;
                    currentLine = Instantiate(wirePrefab, transform);
                    currentLine.startColor = selectedWire.wireColor;
                    currentLine.endColor = selectedWire.wireColor;
                    currentLine.startWidth = wireWidth;
                    currentLine.endWidth = wireWidth;
                    currentLine.sortingOrder = 100; // Устанавливаем высокий sorting order
                    
                    // Устанавливаем начальную позицию
                    Vector3 startPos = selectedWire.leftConnector.transform.position;
                    startPos.z = 0; // Убеждаемся, что Z-координата равна 0
                    currentLine.SetPosition(0, startPos);
                    currentLine.SetPosition(1, startPos);
                }
            }
        }
    }

    private void OnDrag()
    {
        if (currentLine != null)
        {
            Vector3 startPos = selectedWire.leftConnector.transform.position;
            Vector3 endPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            startPos.z = 0;
            endPos.z = 0;
            
            currentLine.SetPosition(0, startPos);
            currentLine.SetPosition(1, endPos);
        }
    }

    private void OnEndDrag()
    {
        if (currentLine != null)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Image connector = hit.collider.GetComponent<Image>();
                if (connector != null && connector.color == selectedWire.wireColor)
                {
                    selectedWire.isConnected = true;
                    currentLine.SetPosition(1, connector.transform.position);
                    completedConnections++;
                    CheckGameCompletion();
                }
                else
                {
                    Destroy(currentLine.gameObject);
                }
            }
            else
            {
                Destroy(currentLine.gameObject);
            }
        }

        isDragging = false;
        selectedWire = null;
        currentLine = null;
    }

    private void CheckGameCompletion()
    {
        if (completedConnections >= wireConnections.Count)
        {
            Debug.Log("All wires connected! Task completed!");
            // Here you can add your completion logic
        }
    }
} 