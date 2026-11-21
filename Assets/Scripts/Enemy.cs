using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float damageCooldown = 0.75f;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private float roomWidth = 22f;
    [SerializeField] private float roomHeight = 10f;
    [SerializeField] private Vector2 roomOrigin = new(-11f, -5f);
    [SerializeField] private float roomPadding = 0.5f;

    private Rigidbody2D rb;
    private Transform player;
    private float nextDamageTime;
    private Vector2 roomMinimum;
    private Vector2 roomMaximum;
    private Vector2Int enemyRoom;
    private int currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        CalculateRoomBounds();
        enemyRoom = GetRoomCoordinates(transform.position);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                player = playerHealth.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (GetRoomCoordinates(player.position) != enemyRoom)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetPosition = new(
            Mathf.Clamp(player.position.x, roomMinimum.x, roomMaximum.x),
            Mathf.Clamp(player.position.y, roomMinimum.y, roomMaximum.y));
        Vector2 direction = (targetPosition - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        rb.position = new(
            Mathf.Clamp(rb.position.x, roomMinimum.x, roomMaximum.x),
            Mathf.Clamp(rb.position.y, roomMinimum.y, roomMaximum.y));

        if (direction.x != 0f)
        {
            GetComponent<SpriteRenderer>().flipX = direction.x < 0f;
        }
    }

    private void CalculateRoomBounds()
    {
        Vector2Int room = GetRoomCoordinates(transform.position);

        Vector2 roomMinimumWithoutPadding = new(
            roomOrigin.x + room.x * roomWidth,
            roomOrigin.y + room.y * roomHeight);
        Vector2 roomMaximumWithoutPadding = roomMinimumWithoutPadding + new Vector2(roomWidth, roomHeight);

        roomMinimum = roomMinimumWithoutPadding + Vector2.one * roomPadding;
        roomMaximum = roomMaximumWithoutPadding - Vector2.one * roomPadding;
    }

    private Vector2Int GetRoomCoordinates(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt((position.x - roomOrigin.x) / roomWidth),
            Mathf.FloorToInt((position.y - roomOrigin.y) / roomHeight));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DamagePlayer(collision.collider);
    }

    private void DamagePlayer(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null && Time.time >= nextDamageTime)
        {
            health.TakeDamage(contactDamage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth == 0)
        {
            SoundEffects.PlayEnemyDeath(transform.position);
            Destroy(gameObject);
        }
    }
}
