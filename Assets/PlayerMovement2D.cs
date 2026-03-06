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

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;

    private Vector2 facingDirection = Vector2.down;
    private bool shootRequested;

    public Vector2 FacingDirection => facingDirection;
    public Vector2 MoveInput => moveInput;
    public Vector2 CurrentVelocity => currentVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (moveInput.sqrMagnitude > 0.01f)
        {
            facingDirection = moveInput.normalized;
        }
    }

    private void OnShoot()
    {
        shootRequested = true;
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

    private void FixedUpdate()
    {
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