using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [SerializeField] private BallState currentState = BallState.Free;

    [Header("Control")]
    [SerializeField] private float controlRadius = 1f;
    [SerializeField] private float baseControlOffset = 0.75f;
    [SerializeField] private float maxExtraOffsetFromMovement = 0.15f;
    [SerializeField] private float maxExtraOffsetFromTurn = 0.2f;
    [SerializeField] private float offsetSmoothSpeed = 10f;

    [Header("Turn Loss")]
    [SerializeField] private float sharpTurnDotThreshold = -0.7f;
    [SerializeField] private float minimumSpeedForTurnCheck = 2f;
    [SerializeField] private float looseBallForce = 2.5f;

    [Header("Shooting")]
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private float recaptureCooldown = 0.2f;

    [Header("Passing")]
    [SerializeField] private float passForce = 8f;
    [SerializeField] private float maxPassMissAngle = 25f;

    [Header("References")]
    [SerializeField] private PlayerControlManager controlManager;
    [SerializeField] private PlayerMovement2D[] players;

    private Rigidbody2D rb;
    private Transform currentOwner;
    private PlayerMovement2D currentOwnerMovement;
    private float recaptureTimer;

    private float currentControlOffset;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool turnCheckReady;

    public BallState CurrentState => currentState;
    public Transform CurrentOwner => currentOwner;
    public Vector2 BallPosition => transform.position;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentControlOffset = baseControlOffset;
    }

    private void Update()
    {
        if (recaptureTimer > 0f)
        {
            recaptureTimer -= Time.deltaTime;
        }

        if (currentState == BallState.Free)
        {
            TryCaptureBall();
        }
        else if (currentState == BallState.Controlled)
        {
            CheckForTurnLoss();
            CheckForShoot();
            CheckForPass();
        }
    }

    private void LateUpdate()
    {
        if (currentState == BallState.Controlled)
        {
            UpdateControlledBallPosition();
        }
    }

    private void TryCaptureBall()
    {
        if (recaptureTimer > 0f)
            return;

        if (players == null || players.Length == 0)
            return;

        PlayerMovement2D bestCandidate = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < players.Length; i++)
        {
            PlayerMovement2D player = players[i];

            if (player == null)
                continue;

            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance <= controlRadius && distance < bestDistance)
            {
                bestDistance = distance;
                bestCandidate = player;
            }
        }

        if (bestCandidate != null)
        {
            CaptureBall(bestCandidate.transform);
        }
    }

    private void CaptureBall(Transform owner)
    {
        currentOwner = owner;
        currentOwnerMovement = owner.GetComponent<PlayerMovement2D>();
        currentState = BallState.Controlled;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        if (currentOwnerMovement != null)
        {
            Vector2 moveInput = currentOwnerMovement.MoveInput;

            if (moveInput.sqrMagnitude > 0.01f)
                lastMoveDirection = moveInput.normalized;
            else
                lastMoveDirection = currentOwnerMovement.FacingDirection.normalized;

            if (controlManager != null &&
                controlManager.CurrentControlledPlayer != currentOwnerMovement)
            {
                controlManager.SetControlledPlayer(currentOwnerMovement);
            }
        }

        currentControlOffset = baseControlOffset;
        turnCheckReady = true;
    }

    private void UpdateControlledBallPosition()
    {
        if (currentOwner == null || currentOwnerMovement == null)
            return;

        Vector2 moveInput = currentOwnerMovement.MoveInput;
        Vector2 facingDirection = currentOwnerMovement.FacingDirection;

        float moveAmount = moveInput.magnitude;
        float movementExtraOffset = moveAmount * maxExtraOffsetFromMovement;

        float turnExtraOffset = 0f;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector2 moveDirection = moveInput.normalized;
            float turnDot = Vector2.Dot(lastMoveDirection, moveDirection);
            float turnAmount = (1f - turnDot) * 0.5f;
            turnExtraOffset = turnAmount * maxExtraOffsetFromTurn;
        }

        float targetOffset = baseControlOffset + movementExtraOffset + turnExtraOffset;

        currentControlOffset = Mathf.Lerp(
            currentControlOffset,
            targetOffset,
            offsetSmoothSpeed * Time.deltaTime
        );

        Vector2 targetPosition = (Vector2)currentOwner.position + facingDirection * currentControlOffset;
        transform.position = targetPosition;
    }

    private void CheckForTurnLoss()
    {
        if (currentOwner == null || currentOwnerMovement == null)
            return;

        Vector2 moveInput = currentOwnerMovement.MoveInput;

        if (moveInput.sqrMagnitude < 0.01f)
            return;

        Vector2 currentMoveDirection = moveInput.normalized;
        float currentSpeed = currentOwnerMovement.CurrentVelocity.magnitude;

        if (!turnCheckReady)
        {
            lastMoveDirection = currentMoveDirection;
            turnCheckReady = true;
            return;
        }

        float turnDot = Vector2.Dot(lastMoveDirection, currentMoveDirection);

        if (currentSpeed >= minimumSpeedForTurnCheck && turnDot <= sharpTurnDotThreshold)
        {
            float dribbling = currentOwnerMovement.dribbling;
            float maxSpeed = currentOwnerMovement.maxSpeed;

            float speedFactor = maxSpeed > 0f ? currentSpeed / maxSpeed : 0f;
            float turnSeverity = Mathf.InverseLerp(-0.7f, -1f, turnDot);

            float difficulty = (speedFactor * 60f) + (turnSeverity * 40f);

            if (difficulty > dribbling)
            {
                LoseControlFromBadTurn(lastMoveDirection);
                return;
            }
        }

        lastMoveDirection = currentMoveDirection;
    }

    private void LoseControlFromBadTurn(Vector2 previousDirection)
    {
        currentOwner = null;
        currentOwnerMovement = null;
        currentState = BallState.Free;

        rb.simulated = true;
        rb.linearVelocity = previousDirection.normalized * looseBallForce;

        recaptureTimer = recaptureCooldown;
        turnCheckReady = false;
    }

    private void CheckForShoot()
    {
        if (currentOwnerMovement == null)
            return;

        if (currentOwnerMovement.ConsumeShootRequest())
        {
            ShootBall();
        }
    }

    private void ShootBall()
    {
        if (currentOwnerMovement == null)
            return;

        Vector2 shootDirection = currentOwnerMovement.FacingDirection.normalized;

        currentOwner = null;
        currentOwnerMovement = null;
        currentState = BallState.Free;

        rb.simulated = true;
        rb.linearVelocity = shootDirection * shootForce;

        recaptureTimer = recaptureCooldown;
        turnCheckReady = false;
    }

    private void CheckForPass()
    {
        if (currentOwnerMovement == null)
            return;

        if (currentOwnerMovement.ConsumePassRequest())
        {
            PassBall();
        }
    }

    private void PassBall()
    {
        if (currentOwnerMovement == null)
            return;

        Vector2 passDirection = currentOwnerMovement.FacingDirection.normalized;
        Vector2 finalDirection = ApplyPassInaccuracy(passDirection, currentOwnerMovement.passing);

        ReleaseBall(finalDirection * passForce);
    }

    private Vector2 ApplyPassInaccuracy(Vector2 idealDirection, float passingStat)
    {
        float accuracy01 = Mathf.Clamp01(passingStat / 100f);
        float missAngle = Mathf.Lerp(maxPassMissAngle, 0f, accuracy01);

        float randomAngle = Random.Range(-missAngle, missAngle);
        float radians = randomAngle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        Vector2 rotated = new Vector2(
            idealDirection.x * cos - idealDirection.y * sin,
            idealDirection.x * sin + idealDirection.y * cos
        );

        return rotated.normalized;
    }

    private void ReleaseBall(Vector2 velocity)
    {
        currentOwner = null;
        currentOwnerMovement = null;
        currentState = BallState.Free;

        rb.simulated = true;
        rb.linearVelocity = velocity;

        recaptureTimer = recaptureCooldown;
        turnCheckReady = false;
    }
}