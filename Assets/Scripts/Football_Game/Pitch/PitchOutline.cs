using UnityEngine;

public class PitchOutline : MonoBehaviour
{
    [Header("Main Pitch")]
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float minY = -6f;
    [SerializeField] private float maxY = 6f;

    [Header("Left Offensive Zone")]
    [SerializeField] private float leftZoneMinX = -4f;
    [SerializeField] private float leftZoneMaxX = 0f;
    [SerializeField] private float leftZoneMinY = -6f;
    [SerializeField] private float leftZoneMaxY = 6f;

    [Header("Center Offensive Zone")]
    [SerializeField] private float centerZoneMinX = -2f;
    [SerializeField] private float centerZoneMaxX = 2f;
    [SerializeField] private float centerZoneMinY = -6f;
    [SerializeField] private float centerZoneMaxY = 6f;

    [Header("Right Offensive Zone")]
    [SerializeField] private float rightZoneMinX = 0f;
    [SerializeField] private float rightZoneMaxX = 4f;
    [SerializeField] private float rightZoneMinY = -6f;
    [SerializeField] private float rightZoneMaxY = 6f;

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer leftZoneLineRenderer;
    [SerializeField] private LineRenderer centerZoneLineRenderer;
    [SerializeField] private LineRenderer rightZoneLineRenderer;

    // Properties to expose the pitch boundaries
    public float MinX => minX;
    public float MaxX => maxX;
    public float MinY => minY;
    public float MaxY => maxY;

    // Properties to expose the offensive zone boundaries
    public float LeftZoneMinX => leftZoneMinX;
    public float LeftZoneMaxX => leftZoneMaxX;
    public float LeftZoneMinY => leftZoneMinY;
    public float LeftZoneMaxY => leftZoneMaxY;
    public float CenterZoneMinX => centerZoneMinX;
    public float CenterZoneMaxX => centerZoneMaxX;
    public float CenterZoneMinY => centerZoneMinY;
    public float CenterZoneMaxY => centerZoneMaxY;
    public float RightZoneMinX => rightZoneMinX;
    public float RightZoneMaxX => rightZoneMaxX;
    public float RightZoneMinY => rightZoneMinY;
    public float RightZoneMaxY => rightZoneMaxY;


    private void Awake()
    {
        SetupLine(lineRenderer, Color.green);
        SetupLine(leftZoneLineRenderer, Color.red);
        SetupLine(centerZoneLineRenderer, Color.yellow);
        SetupLine(rightZoneLineRenderer, Color.blue);

        DrawOutline();
        DrawLeftOffensiveZone();
        DrawCenterOffensiveZone();
        DrawRightOffensiveZone();
    }

    private void SetupLine(LineRenderer lr, Color color)
    {
        if (lr == null)
        {
            Debug.LogError("A LineRenderer reference is missing.");
            return;
        }

        lr.loop = true;
        lr.useWorldSpace = true;
        lr.positionCount = 4;
        lr.widthMultiplier = 0.08f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
    }

    private void DrawOutline()
    {
        if (lineRenderer == null) return;

        lineRenderer.SetPosition(0, new Vector3(minX, minY, 0f));
        lineRenderer.SetPosition(1, new Vector3(maxX, minY, 0f));
        lineRenderer.SetPosition(2, new Vector3(maxX, maxY, 0f));
        lineRenderer.SetPosition(3, new Vector3(minX, maxY, 0f));
    }

    private void DrawLeftOffensiveZone()
    {
        if (leftZoneLineRenderer == null) return;

        leftZoneLineRenderer.SetPosition(0, new Vector3(leftZoneMinX, leftZoneMinY, 0f));
        leftZoneLineRenderer.SetPosition(1, new Vector3(leftZoneMaxX, leftZoneMinY, 0f));
        leftZoneLineRenderer.SetPosition(2, new Vector3(leftZoneMaxX, leftZoneMaxY, 0f));
        leftZoneLineRenderer.SetPosition(3, new Vector3(leftZoneMinX, leftZoneMaxY, 0f));
    }

    private void DrawCenterOffensiveZone()
    {
        if (centerZoneLineRenderer == null) return;

        centerZoneLineRenderer.SetPosition(0, new Vector3(centerZoneMinX, centerZoneMinY, 0f));
        centerZoneLineRenderer.SetPosition(1, new Vector3(centerZoneMaxX, centerZoneMinY, 0f));
        centerZoneLineRenderer.SetPosition(2, new Vector3(centerZoneMaxX, centerZoneMaxY, 0f));
        centerZoneLineRenderer.SetPosition(3, new Vector3(centerZoneMinX, centerZoneMaxY, 0f));
    }

    private void DrawRightOffensiveZone()
    {
        if (rightZoneLineRenderer == null) return;

        rightZoneLineRenderer.SetPosition(0, new Vector3(rightZoneMinX, rightZoneMinY, 0f));
        rightZoneLineRenderer.SetPosition(1, new Vector3(rightZoneMaxX, rightZoneMinY, 0f));
        rightZoneLineRenderer.SetPosition(2, new Vector3(rightZoneMaxX, rightZoneMaxY, 0f));
        rightZoneLineRenderer.SetPosition(3, new Vector3(rightZoneMinX, rightZoneMaxY, 0f));
    }
}