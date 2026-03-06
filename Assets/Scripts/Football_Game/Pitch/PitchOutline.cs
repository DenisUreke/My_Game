using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PitchOutline : MonoBehaviour
{
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float minY = -6f;
    [SerializeField] private float maxY = 6f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLine();
        DrawOutline();
    }

    private void OnValidate()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            SetupLine();
            DrawOutline();
        }
    }

    private void SetupLine()
    {
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 4;
        lineRenderer.widthMultiplier = 0.08f;
    }

    private void DrawOutline()
    {
        Vector3 bottomLeft = new Vector3(minX, minY, 0f);
        Vector3 bottomRight = new Vector3(maxX, minY, 0f);
        Vector3 topRight = new Vector3(maxX, maxY, 0f);
        Vector3 topLeft = new Vector3(minX, maxY, 0f);

        lineRenderer.SetPosition(0, bottomLeft);
        lineRenderer.SetPosition(1, bottomRight);
        lineRenderer.SetPosition(2, topRight);
        lineRenderer.SetPosition(3, topLeft);
    }
}