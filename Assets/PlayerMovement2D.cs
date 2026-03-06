using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Stats")]
    public float maxSpeed = 5f;
    public float acceleration = 12f;
    public float deceleration = 24f;

    [Header("Ball Control Stats")]
    [Range(0f, 100f)] public float dribbling = 50f;
    [Range(0f, 100f)] public float passing = 50f;

    [Header("Control")]
    [SerializeField] private bool isControlled = false;

    private Rigidbody2D rb;

    private Vector2 rawMoveInput;
    private Vector2 moveInput;
    private Vector2 currentVelocity;

    private Vector2 facingDirection = Vector2.down;
    private bool shootRequested;
    private bool passRequested;
    private bool switchPlayerRequested;

    public Vector2 FacingDirection => facingDirection;
    public Vector2 MoveInput => moveInput;
    public Vector2 CurrentVelocity => currentVelocity;
    public bool IsControlled => isControlled;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetControlled(bool value)
    {
        isControlled = value;

        if (!isControlled)
        {
            moveInput = Vector2.zero;
            currentVelocity = Vector2.zero;
            shootRequested = false;
            passRequested = false;
            switchPlayerRequested = false;
        }
        else
        {
            moveInput = rawMoveInput;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                facingDirection = moveInput.normalized;
            }
        }
    }

    private void OnMove(InputValue value)
    {
        rawMoveInput = value.Get<Vector2>();

        if (!isControlled)
            return;

        moveInput = rawMoveInput;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            facingDirection = moveInput.normalized;
        }
    }

    private void OnShoot()
    {
        if (!isControlled)
            return;

        shootRequested = true;
    }

    private void OnPass()
    {
        if (!isControlled)
            return;

        passRequested = true;
    }

    private void OnSwitchPlayer()
    {
        if (!isControlled)
            return;

        switchPlayerRequested = true;
    }

    public bool ConsumeShootRequest()
    {
        if (shootRequested)
        {
            shootRequested = false;
            return true;
        }

        return false;
    }

    public bool ConsumePassRequest()
    {
        if (passRequested)
        {
            passRequested = false;
            return true;
        }

        return false;
    }

    public bool ConsumeSwitchPlayerRequest()
    {
        if (switchPlayerRequested)
        {
            switchPlayerRequested = false;
            return true;
        }

        return false;
    }

    private void FixedUpdate()
    {
        if (isControlled)
        {
            moveInput = rawMoveInput;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                facingDirection = moveInput.normalized;
            }
        }
        else
        {
            moveInput = Vector2.zero;
        }

        Vector2 targetVelocity = moveInput * maxSpeed;
        float rate = moveInput.sqrMagnitude > 0.01f ? acceleration : deceleration;

        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            rate * Time.fixedDeltaTime
        );

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}