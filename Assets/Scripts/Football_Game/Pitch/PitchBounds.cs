using UnityEngine;

public class PitchBounds : MonoBehaviour
{
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float minY = -6f;
    [SerializeField] private float maxY = 6f;
    [SerializeField] private float margin = 0.5f;

    public float MinX => minX;
    public float MaxX => maxX;
    public float MinY => minY;
    public float MaxY => maxY;
    public float Margin => margin;

    public Vector2 ClampInsidePitch(Vector2 position)
    {
        return new Vector2(
            Mathf.Clamp(position.x, minX + margin, maxX - margin),
            Mathf.Clamp(position.y, minY + margin, maxY - margin)
        );
    }

    public bool IsInsidePitch(Vector2 position)
    {
        return position.x >= minX &&
               position.x <= maxX &&
               position.y >= minY &&
               position.y <= maxY;
    }
}