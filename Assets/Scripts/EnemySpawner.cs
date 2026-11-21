using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Sprite enemySprite;
    [SerializeField] private Sprite bossSprite;
    [SerializeField] private Vector2Int bossRoom = new(3, -3);
    [SerializeField] private float bossScale = 2f;
    [SerializeField] private float roomWidth = 22f;
    [SerializeField] private float roomHeight = 10f;
    [SerializeField] private Vector2 roomOrigin = new(-11f, -5f);
    [SerializeField] private float entrancePadding = 3.5f;
    [SerializeField] private float entranceClearance = 2.5f;
    [SerializeField] private Vector2Int startingRoom = Vector2Int.zero;
    [SerializeField] private float sameRoomSpacing = 2f;
    [SerializeField] private Vector3[] enemyPositions =
    {
        new(-25f, -1f, -0.6f),
        new(-25f, 1f, -0.6f),
        new(25f, 1f, -0.6f),
        new(19f, -1f, -0.6f),
        new(-43f, -20f, -0.6f),
        new(45f, -20f, -0.6f),
        new(39f, -30f, -0.6f),
        new(49f, -30f, -0.6f),
        new(69f, -10f, -0.6f),
        new(61f, -10f, -0.6f),
        new(-47f, 10f, -0.6f),
        new(-39f, 10f, -0.6f),
        new(-25f, 10f, -0.6f),
        new(69f, 1f, -0.6f),
        new(-19f, 1f, -0.6f),
        new(28f, -1f, -0.6f),
        new(-25f, -10f, -0.6f),
        new(-3f, 10f, -0.6f),
        new(25f, 10f, -0.6f),
        new(3f, -20f, -0.6f),
        new(25f, -20f, -0.6f),
        new(-47f, -10f, -0.6f),
        new(69f, -20f, -0.6f),
        new(47f, -10f, -0.6f)
    };

    private void Awake()
    {
        if (enemySprite == null)
        {
            Debug.LogError($"{nameof(EnemySpawner)} requires an enemy sprite.", this);
            return;
        }

        Dictionary<Vector2Int, int> enemiesPerRoom = new();

        for (int i = 0; i < enemyPositions.Length; i++)
        {
            if (!IsSafeSpawnPosition(enemyPositions[i]))
            {
                Debug.LogWarning($"Skipping enemy spawn near a room entrance at {enemyPositions[i]}.", this);
                continue;
            }

            Vector2Int room = GetRoomCoordinates(enemyPositions[i]);
            enemiesPerRoom.TryGetValue(room, out int roomEnemyIndex);
            enemiesPerRoom[room] = roomEnemyIndex + 1;

            Vector3 spawnPosition = GetRoomCenter(room);
            if (roomEnemyIndex > 0)
            {
                spawnPosition.x += (roomEnemyIndex % 2 == 1 ? 1f : -1f) *
                                   Mathf.Ceil(roomEnemyIndex / 2f) *
                                   sameRoomSpacing;
            }

            CreateEnemy(i, spawnPosition);
        }

        if (bossSprite == null)
        {
            Debug.LogError($"{nameof(EnemySpawner)} requires a boss sprite.", this);
            return;
        }

        CreateBoss(GetRoomCenter(bossRoom));
    }

    private bool IsSafeSpawnPosition(Vector3 position)
    {
        Vector2Int room = GetRoomCoordinates(position);

        if (room == startingRoom)
        {
            return false;
        }

        Vector2 roomMinimum = new(
            roomOrigin.x + room.x * roomWidth + entrancePadding,
            roomOrigin.y + room.y * roomHeight + entrancePadding);
        Vector2 roomMaximum = roomMinimum + new Vector2(
            roomWidth - entrancePadding * 2f,
            roomHeight - entrancePadding * 2f);

        if (position.x <= roomMinimum.x ||
            position.x >= roomMaximum.x ||
            position.y <= roomMinimum.y ||
            position.y >= roomMaximum.y)
        {
            return false;
        }

        Vector2 roomCenter = (roomMinimum + roomMaximum) * 0.5f;
        bool nearHorizontalEntrance = Mathf.Abs(position.x - roomCenter.x) < entranceClearance &&
                                      (Mathf.Abs(position.y - roomMinimum.y) < entranceClearance ||
                                       Mathf.Abs(position.y - roomMaximum.y) < entranceClearance);
        bool nearVerticalEntrance = Mathf.Abs(position.y - roomCenter.y) < entranceClearance &&
                                    (Mathf.Abs(position.x - roomMinimum.x) < entranceClearance ||
                                     Mathf.Abs(position.x - roomMaximum.x) < entranceClearance);

        return !nearHorizontalEntrance && !nearVerticalEntrance;
    }

    private Vector2Int GetRoomCoordinates(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt((position.x - roomOrigin.x) / roomWidth),
            Mathf.FloorToInt((position.y - roomOrigin.y) / roomHeight));
    }

    private Vector3 GetRoomCenter(Vector2Int room)
    {
        return new Vector3(
            roomOrigin.x + room.x * roomWidth + roomWidth * 0.5f,
            roomOrigin.y + room.y * roomHeight + roomHeight * 0.5f,
            -0.6f);
    }

    private void CreateEnemy(int index, Vector3 position)
    {
        GameObject enemy = new($"Enemy_{index + 1:00}");
        enemy.transform.SetParent(transform, false);
        enemy.transform.localPosition = position;

        SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
        renderer.sprite = enemySprite;
        renderer.sortingOrder = 1;

        CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
        collider.radius = 0.4f;

        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        enemy.AddComponent<Enemy>();
    }

    private void CreateBoss(Vector3 position)
    {
        GameObject boss = new("Boss_Vampire");
        boss.transform.SetParent(transform, false);
        boss.transform.localPosition = position;
        boss.transform.localScale = Vector3.one * bossScale;

        SpriteRenderer renderer = boss.AddComponent<SpriteRenderer>();
        renderer.sprite = bossSprite;
        renderer.sortingOrder = 2;

        CircleCollider2D collider = boss.AddComponent<CircleCollider2D>();
        collider.radius = 0.45f;

        Rigidbody2D body = boss.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        boss.AddComponent<BossEnemy>();
    }
}
