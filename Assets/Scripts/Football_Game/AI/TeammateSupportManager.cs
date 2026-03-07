using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TeammateSupportManager : MonoBehaviour
{
    [SerializeField] private PlayerControlManager playerControlManager;
    [SerializeField] private BallController ballController;
    [SerializeField] private PitchBounds pitchBounds;

    [Header("Players")]
    [SerializeField] private PlayerMovement2D leftSupportPlayer;
    [SerializeField] private PlayerMovement2D rightSupportPlayer;
    [SerializeField] private PlayerMovement2D centerSupportPlayer;

    [Header("Offsetrules")]
    [Header("Left")]
    [SerializeField] private Vector2 leftSideCenterHoldsBall = new Vector2(0f, 0f);
    [SerializeField] private Vector2 lefSidetRightHoldsBall = new Vector2(0f, 0f);
    [Header("Center")]
    [SerializeField] private Vector2 centerSideLeftHoldsBall = new Vector2(0f, 0f);
    [SerializeField] private Vector2 centerSideRightHoldsBall = new Vector2(2f, 2f);
    [Header("Right")]
    [SerializeField] private Vector2 rightSideCenterHoldsBall = new Vector2(0f, 0f);
    [SerializeField] private Vector2 rightSideLeftHoldsBall = new Vector2(0f, 0f);

    [Header("Allowed Offset")]
    [SerializeField] private float allowedOffset = 1.5f;
    [Header("Offset Timer")]
    [SerializeField] private float offsetInterval = 1f;

    private BallState ballstate;
    private PlayerMovement2D ballCarrier;
    private Vector2 ballCarrierPosition;

    // Pitch dimensions
    private ZoneSize pitchSize;

    public enum SupportPosition
    {
        Left,
        Center,
        Right
    }

    public struct ZoneSize { 
         public float minX;
         public float maxX;
         public float minY;
         public float maxY;
         public ZoneSize(float minX, float maxX, float minY, float maxY)
         {
             this.minX = minX;
             this.maxX = maxX;
             this.minY = minY;
             this.maxY = maxY;
         }
    }
    // Current offset for each player to add some randomness to their positions
    private Dictionary<PlayerMovement2D, Vector2> currentPlayerOffset = new Dictionary<PlayerMovement2D, Vector2>();
    // Timer to track how long until next offset can be applied for each player
    private Dictionary<PlayerMovement2D, float> currentPlayerTimer = new Dictionary<PlayerMovement2D, float>();

    // Zone dimensions left, right, center
    private Dictionary<SupportPosition, ZoneSize> zones = new Dictionary<SupportPosition, ZoneSize>();
    // Player assignments for each zone
    private Dictionary<SupportPosition, PlayerMovement2D> assignments = new Dictionary<SupportPosition, PlayerMovement2D>();

    private void Start()
    {
        if (playerControlManager == null || ballController == null || pitchBounds == null)
        {
            Debug.LogError("TeammateSupportManager: Missing references. Disabling script.");
            enabled = false;
            return;
        }
        AssignSupportPlayers();
        AssignZones();
        AssignOffsetDictionaries();
    }

    private void FixedUpdate()
    {
        // check state of the ball
        ballstate = ballController.CurrentState;
        if (ballstate != BallState.Controlled)
        {
            return;
        }

        // get the ball carrier
        ballCarrier = ballController.CurrentOwner?.GetComponent<PlayerMovement2D>();
        if (ballCarrier == null)
        {
            return;
        }

        // get the position of the ball carrier and his current zone
        ballCarrierPosition = ballCarrier.transform.position;
        SupportPosition currentZone = GetCurrentZone(ballCarrierPosition);
        bool hasChangedZone = HasChangedZones(currentZone, ballCarrier);
        if (hasChangedZone)
        {
            UpdateAssignments(currentZone, ballCarrier);
        }

        UpdatePositions(ballCarrier, currentZone);


        //assignments[SupportPosition.Left].SetAiTarget(new Vector2(3f, 2f));

    }
    void AssignSupportPlayers()
    { 
        assignments.Add(SupportPosition.Left, leftSupportPlayer);
        assignments.Add(SupportPosition.Center, centerSupportPlayer);
        assignments.Add(SupportPosition.Right, rightSupportPlayer);
    }

    void AssignZones()
    {
        pitchSize = new ZoneSize(pitchBounds.MinX, pitchBounds.MaxX, pitchBounds.MinY, pitchBounds.MaxY);
        ZoneSize leftZoneSize = new ZoneSize(pitchBounds.LeftZoneMinX, pitchBounds.LeftZoneMaxX, pitchBounds.LeftZoneMinY, pitchBounds.LeftZoneMaxY);
        ZoneSize centerZoneSize = new ZoneSize(pitchBounds.CenterZoneMinX, pitchBounds.CenterZoneMaxX, pitchBounds.CenterZoneMinY, pitchBounds.CenterZoneMaxY);
        ZoneSize rightZoneSize = new ZoneSize(pitchBounds.RightZoneMinX, pitchBounds.RightZoneMaxX, pitchBounds.RightZoneMinY, pitchBounds.RightZoneMaxY);

        zones.Add(SupportPosition.Left, leftZoneSize);
        zones.Add(SupportPosition.Center, centerZoneSize);
        zones.Add(SupportPosition.Right, rightZoneSize);
    }
    void AssignOffsetDictionaries()
    {
        currentPlayerOffset.Add(leftSupportPlayer, Vector2.zero);
        currentPlayerOffset.Add(centerSupportPlayer, Vector2.zero);
        currentPlayerOffset.Add(rightSupportPlayer, Vector2.zero);
        currentPlayerTimer.Add(leftSupportPlayer, 0f);
        currentPlayerTimer.Add(centerSupportPlayer, 0f);
        currentPlayerTimer.Add(rightSupportPlayer, 0f);
    }

    SupportPosition GetCurrentZone(Vector2 ballholder)
    {

        foreach ( var zone in zones )
        {
            if (ballholder.x >= zone.Value.minX && ballholder.x <= zone.Value.maxX &&
                ballholder.y >= zone.Value.minY && ballholder.y <= zone.Value.maxY)
            {
                return zone.Key;
            }
        }
        return SupportPosition.Center; // Default to center if not found
    }

    bool HasChangedZones(SupportPosition currentZone, PlayerMovement2D ballcarrier )
    {
        foreach (var assignment in assignments)
        {
            if (ballcarrier == assignment.Value)
            {
                if (assignment.Key != currentZone)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
    void UpdateAssignments(SupportPosition currentZone, PlayerMovement2D ballcarrier)
    {
        PlayerMovement2D playerToMove = assignments[currentZone];
        SupportPosition currentBallholderPosition = SupportPosition.Center; // Default value, will be updated in the loop

        foreach (var assignment in assignments)
        {
            if (assignment.Value == ballcarrier)
            {
                currentBallholderPosition = assignment.Key;
                assignments[currentBallholderPosition] = playerToMove;
                assignments[currentZone] = ballcarrier;
                return;
            }
        }
    }

    Vector2 GetVectorWithSupportPosition(SupportPosition currentBallposition, PlayerMovement2D caller)
    {
        switch (currentBallposition)
        {
            case SupportPosition.Center:
                if (caller == assignments[SupportPosition.Left])
                {
                    return leftSideCenterHoldsBall;
                }
                else if (caller == assignments[SupportPosition.Right])
                {
                    return rightSideCenterHoldsBall;
                }
                break;
            case SupportPosition.Left:
                if (caller == assignments[SupportPosition.Center])
                {
                    return centerSideLeftHoldsBall;
                }
                else if (caller == assignments[SupportPosition.Right])
                {
                    return rightSideLeftHoldsBall;
                }
                break;
            case SupportPosition.Right:
                if (caller == assignments[SupportPosition.Center])
                {
                    return centerSideRightHoldsBall;
                }
                else if (caller == assignments[SupportPosition.Left])
                {
                    return lefSidetRightHoldsBall;
                }
                break;
        }

        return Vector2.zero;
    }

    Vector2 GetAppliedRandomOffset()
    {
        float offsetX = Random.Range(-allowedOffset, allowedOffset);
        float offsetY = Random.Range(-allowedOffset, allowedOffset);
        Vector2 randomOffset = new Vector2(offsetX, offsetY);
        return randomOffset;
    }

    float GetRandomOffsetInterval()
    {
        return Random.Range(0, offsetInterval);
    }
    void UpdatePositions(PlayerMovement2D ballcarrier, SupportPosition currentzone)
    {
        foreach (var assignment in assignments)
        {
            if (assignment.Value != ballcarrier)
            {
                Vector2 offset = GetVectorWithSupportPosition(currentzone, assignment.Value);
                if (Time.time > currentPlayerTimer[assignment.Value])
                {
                    currentPlayerOffset[assignment.Value] = GetAppliedRandomOffset(); // Update the offset for the player
                    currentPlayerTimer[assignment.Value] = Time.time + GetRandomOffsetInterval(); // Reset the timer for the next offset change
                }
                Vector2 targetPosition = (Vector2)ballcarrier.transform.position + offset + currentPlayerOffset[assignment.Value];
                targetPosition = pitchBounds.ClampInsidePitch(targetPosition);
                assignment.Value.SetAiTarget(targetPosition);
            }
        }
    }
}