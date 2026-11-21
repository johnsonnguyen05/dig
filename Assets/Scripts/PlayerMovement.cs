using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
<<<<<<< HEAD
    [SerializeField] private float rollSpeed = 10f;
    [SerializeField] private float rollDuration = 0.35f;
    [SerializeField] private float rollCooldown = 0.8f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;
    private float rollTimer;
    private float nextRollTime;
    private Color originalSpriteColor;
    private float originalRotation;
    private float rollRotation;

    public bool IsRolling => rollTimer > 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpriteColor = spriteRenderer.color;
        originalRotation = transform.eulerAngles.z;
    }

    private void Update()
=======

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
>>>>>>> fc8d515ec449e4a03a43e95f21448b4e0d8567e3
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

<<<<<<< HEAD
        if (rollTimer > 0f)
        {
            rollTimer -= Time.deltaTime;

            float rollProgress = 1f - Mathf.Clamp01(rollTimer / rollDuration);
            rollRotation = Mathf.Lerp(0f, 360f, rollProgress);
            transform.rotation = Quaternion.Euler(0f, 0f, originalRotation + rollRotation);

            bool whiteFlash = Mathf.FloorToInt(rollProgress * 12f) % 2 == 0;
            spriteRenderer.color = whiteFlash ? Color.white : new Color(1f, 1f, 1f, 0.25f);

            if (rollTimer <= 0f)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, originalRotation);
                spriteRenderer.color = originalSpriteColor;
            }
        }

=======
>>>>>>> fc8d515ec449e4a03a43e95f21448b4e0d8567e3
        moveInput = Vector2.zero;

        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) moveInput.y += 1;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) moveInput.y -= 1;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveInput.x += 1;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) moveInput.x -= 1;

        moveInput = moveInput.normalized;
<<<<<<< HEAD

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
        }

        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }

        if (kb.spaceKey.wasPressedThisFrame &&
            !IsRolling &&
            Time.time >= nextRollTime)
        {
            originalRotation = transform.eulerAngles.z;
            rollRotation = 0f;
            rollTimer = rollDuration;
            nextRollTime = Time.time + rollCooldown;
            spriteRenderer.color = Color.white;
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = moveInput != Vector2.zero ? moveInput : lastMoveDirection;
        float speed = IsRolling ? rollSpeed : moveSpeed;
        rb.linearVelocity = IsRolling ? direction * speed : moveInput * speed;
=======
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed; // use rb.velocity on older Unity versions
>>>>>>> fc8d515ec449e4a03a43e95f21448b4e0d8567e3
    }
}