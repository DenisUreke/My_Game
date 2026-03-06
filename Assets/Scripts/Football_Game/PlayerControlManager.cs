using UnityEngine;

public class PlayerControlManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement2D startingPlayer;
    [SerializeField] private PlayerMovement2D[] controllablePlayers;
    [SerializeField] private BallController ballController;

    private PlayerMovement2D currentControlledPlayer;

    public PlayerMovement2D CurrentControlledPlayer => currentControlledPlayer;
    public PlayerMovement2D[] ControllablePlayers => controllablePlayers;

    private void Start()
    {
        if (controllablePlayers != null)
        {
            for (int i = 0; i < controllablePlayers.Length; i++)
            {
                if (controllablePlayers[i] != null)
                {
                    controllablePlayers[i].SetControlled(false);
                }
            }
        }

        if (startingPlayer != null)
        {
            SetControlledPlayer(startingPlayer);
        }
    }

    private void Update()
    {
        if (currentControlledPlayer == null)
            return;

        if (!currentControlledPlayer.ConsumeSwitchPlayerRequest())
            return;

        if (ballController == null)
            return;

        if (ballController.CurrentState != BallState.Free)
            return;

        if (!SwitchToClosestPlayerNearBall())
        {
            SwitchToNextPlayer();
        }
    }

    public void SetControlledPlayer(PlayerMovement2D newPlayer)
    {
        if (newPlayer == null)
            return;

        if (currentControlledPlayer == newPlayer)
            return;

        if (currentControlledPlayer != null)
        {
            currentControlledPlayer.SetControlled(false);
        }

        currentControlledPlayer = newPlayer;
        currentControlledPlayer.SetControlled(true);
    }

    private bool SwitchToClosestPlayerNearBall()
    {
        if (controllablePlayers == null || controllablePlayers.Length == 0)
            return false;

        Vector2 ballPosition = ballController.transform.position;

        PlayerMovement2D closestPlayer = null;
        PlayerMovement2D secondClosestPlayer = null;

        float closestDistance = float.MaxValue;
        float secondClosestDistance = float.MaxValue;

        for (int i = 0; i < controllablePlayers.Length; i++)
        {
            PlayerMovement2D player = controllablePlayers[i];

            if (player == null)
                continue;

            float distance = Vector2.Distance(player.transform.position, ballPosition);

            if (distance < closestDistance)
            {
                secondClosestDistance = closestDistance;
                secondClosestPlayer = closestPlayer;

                closestDistance = distance;
                closestPlayer = player;
            }
            else if (distance < secondClosestDistance)
            {
                secondClosestDistance = distance;
                secondClosestPlayer = player;
            }
        }

        if (closestPlayer == null)
            return false;

        if (currentControlledPlayer != closestPlayer)
        {
            SetControlledPlayer(closestPlayer);
            return true;
        }

        if (secondClosestPlayer != null)
        {
            SetControlledPlayer(secondClosestPlayer);
            return true;
        }

        return false;
    }

    private void SwitchToNextPlayer()
    {
        if (controllablePlayers == null || controllablePlayers.Length == 0)
            return;

        int currentIndex = -1;

        for (int i = 0; i < controllablePlayers.Length; i++)
        {
            if (controllablePlayers[i] == currentControlledPlayer)
            {
                currentIndex = i;
                break;
            }
        }

        if (currentIndex == -1)
        {
            SetControlledPlayer(controllablePlayers[0]);
            return;
        }

        for (int offset = 1; offset <= controllablePlayers.Length; offset++)
        {
            int nextIndex = (currentIndex + offset) % controllablePlayers.Length;
            PlayerMovement2D nextPlayer = controllablePlayers[nextIndex];

            if (nextPlayer != null)
            {
                SetControlledPlayer(nextPlayer);
                return;
            }
        }
    }
}