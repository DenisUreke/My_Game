using System.Collections.Generic;
using UnityEngine;

public class TeammateSupportManager : MonoBehaviour
{
    [SerializeField] private PlayerControlManager playerControlManager;
    [SerializeField] private BallController ballController;
    [SerializeField] private PitchBounds pitchBounds;

    [Header("Support Offsets")]
    [SerializeField] private Vector2 leftSupportOffset = new Vector2(-2f, 2f);
    [SerializeField] private Vector2 rightSupportOffset = new Vector2(2f, 2f);

    [Header("Live Offset Variation")]
    [SerializeField] private float offsetRangeX = 0.75f;
    [SerializeField] private float offsetRangeY = 0.5f;
    [SerializeField] private float minOffsetChangeInterval = 1.5f;
    [SerializeField] private float maxOffsetChangeInterval = 3f;
    [SerializeField] private float offsetLerpSpeed = 2f;

    private Dictionary<PlayerMovement2D, SupportOffsetData> supportOffsets =
        new Dictionary<PlayerMovement2D, SupportOffsetData>();

    private void FixedUpdate()
    {
        if (playerControlManager == null || ballController == null || pitchBounds == null)
            return;

        PlayerMovement2D ballCarrier = GetFriendlyBallCarrier();

        if (ballCarrier == null)
        {
            ClearAllAiTargets();
            return;
        }

        PlayerMovement2D[] players = playerControlManager.ControllablePlayers;

        if (players == null || players.Length == 0)
            return;

        UpdateLiveOffsets(players);

        Vector2 carrierPosition = ballCarrier.transform.position;

        Vector2 leftSupportPosition = carrierPosition + leftSupportOffset;
        Vector2 rightSupportPosition = carrierPosition + rightSupportOffset;

        int supportIndex = 0;

        for (int i = 0; i < players.Length; i++)
        {
            PlayerMovement2D player = players[i];

            if (player == null)
                continue;

            if (player == ballCarrier)
            {
                player.ClearAiTarget();
                continue;
            }

            if (player == playerControlManager.CurrentControlledPlayer)
            {
                player.ClearAiTarget();
                continue;
            }

            Vector2 baseSupportPosition;

            if (supportIndex == 0)
            {
                baseSupportPosition = leftSupportPosition;
            }
            else if (supportIndex == 1)
            {
                baseSupportPosition = rightSupportPosition;
            }
            else
            {
                player.ClearAiTarget();
                supportIndex++;
                continue;
            }

            Vector2 liveOffset = GetCurrentLiveOffset(player);
            Vector2 finalSupportPosition = baseSupportPosition + liveOffset;
            finalSupportPosition = pitchBounds.ClampInsidePitch(finalSupportPosition);

            player.SetAiTarget(finalSupportPosition);

            supportIndex++;
        }
    }

    private void UpdateLiveOffsets(PlayerMovement2D[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            PlayerMovement2D player = players[i];

            if (player == null)
                continue;

            if (!supportOffsets.ContainsKey(player))
            {
                SupportOffsetData newData = new SupportOffsetData();
                newData.currentOffset = Vector2.zero;
                newData.targetOffset = GetRandomOffset();
                newData.timeUntilNextChange = Random.Range(
                    minOffsetChangeInterval,
                    maxOffsetChangeInterval
                );

                supportOffsets.Add(player, newData);
            }

            SupportOffsetData data = supportOffsets[player];

            data.timeUntilNextChange -= Time.fixedDeltaTime;

            if (data.timeUntilNextChange <= 0f)
            {
                data.targetOffset = GetRandomOffset();
                data.timeUntilNextChange = Random.Range(
                    minOffsetChangeInterval,
                    maxOffsetChangeInterval
                );
            }

            data.currentOffset = Vector2.Lerp(
                data.currentOffset,
                data.targetOffset,
                offsetLerpSpeed * Time.fixedDeltaTime
            );

            supportOffsets[player] = data;
        }
    }

    private Vector2 GetCurrentLiveOffset(PlayerMovement2D player)
    {
        if (player == null)
            return Vector2.zero;

        if (!supportOffsets.ContainsKey(player))
            return Vector2.zero;

        return supportOffsets[player].currentOffset;
    }

    private Vector2 GetRandomOffset()
    {
        float offsetX = Random.Range(-offsetRangeX, offsetRangeX);
        float offsetY = Random.Range(-offsetRangeY, offsetRangeY);

        return new Vector2(offsetX, offsetY);
    }

    private PlayerMovement2D GetFriendlyBallCarrier()
    {
        if (ballController.CurrentState != BallState.Controlled)
            return null;

        Transform owner = ballController.CurrentOwner;

        if (owner == null)
            return null;

        PlayerMovement2D ownerMovement = owner.GetComponent<PlayerMovement2D>();

        if (ownerMovement == null)
            return null;

        PlayerMovement2D[] players = playerControlManager.ControllablePlayers;

        if (players == null)
            return null;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == ownerMovement)
                return ownerMovement;
        }

        return null;
    }

    private void ClearAllAiTargets()
    {
        if (playerControlManager == null)
            return;

        PlayerMovement2D[] players = playerControlManager.ControllablePlayers;

        if (players == null)
            return;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                players[i].ClearAiTarget();
            }
        }
    }

    private struct SupportOffsetData
    {
        public Vector2 currentOffset;
        public Vector2 targetOffset;
        public float timeUntilNextChange;
    }
}