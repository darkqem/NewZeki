using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WireLine : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
    }

    public void SetStartPoint(Vector3 startPoint)
    {
        lineRenderer.SetPosition(0, startPoint);
    }

    public void SetEndPoint(Vector3 endPoint)
    {
        lineRenderer.SetPosition(1, endPoint);
    }
} 