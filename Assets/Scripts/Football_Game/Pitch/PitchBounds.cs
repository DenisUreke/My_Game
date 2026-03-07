using UnityEngine;

public class PitchBounds : MonoBehaviour
{
    [SerializeField] private PitchOutline pitchOutline;
    [SerializeField] private float margin = 0.5f;

    // Properties to expose the pitch boundaries
    private float minX = 0f;
    private float maxX = 0f;
    private float minY = 0f;
    private float maxY = 0f;

    // Properties are for the different zones
    private float leftZoneMinX = 0f;
    private float leftZoneMaxX = 0f;
    private float leftZoneMinY = 0f;
    private float leftZoneMaxY = 0f;
    private float centerZoneMinX = 0f;
    private float centerZoneMaxX = 0f;
    private float centerZoneMinY = 0f;
    private float centerZoneMaxY = 0f;
    private float rightZoneMinX = 0f;
    private float rightZoneMaxX = 0f;
    private float rightZoneMinY = 0f;
    private float rightZoneMaxY = 0f;

    //pitch properties
    public float MinX => minX;
    public float MaxX => maxX;
    public float MinY => minY;
    public float MaxY => maxY;
    public float Margin => margin;

    // zone properties
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


    public void Awake()
    {
        if (pitchOutline != null)
        {
            // pitch
            minX = pitchOutline.MinX;
            maxX = pitchOutline.MaxX;
            minY = pitchOutline.MinY;
            maxY = pitchOutline.MaxY;

            // zones
            leftZoneMinX = pitchOutline.LeftZoneMinX;
            leftZoneMaxX = pitchOutline.LeftZoneMaxX;
            leftZoneMinY = pitchOutline.LeftZoneMinY;
            leftZoneMaxY = pitchOutline.LeftZoneMaxY;

            centerZoneMinX = pitchOutline.CenterZoneMinX;
            centerZoneMaxX = pitchOutline.CenterZoneMaxX;
            centerZoneMinY = pitchOutline.CenterZoneMinY;
            centerZoneMaxY = pitchOutline.CenterZoneMaxY;

            rightZoneMinX = pitchOutline.RightZoneMinX;
            rightZoneMaxX = pitchOutline.RightZoneMaxX;
            rightZoneMinY = pitchOutline.RightZoneMinY;
            rightZoneMaxY = pitchOutline.RightZoneMaxY;
        }
        else
        {
            Debug.LogWarning("PitchOutline reference is missing. Using default bounds.");
        }
    }
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