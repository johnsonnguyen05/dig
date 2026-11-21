using UnityEngine;

public class RoomLockedCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float roomWidth = 19f;
    [SerializeField] private float roomHeight = 10f;
    [SerializeField] private Vector2 roomOrigin = new(-9.5f, -5f);
    [SerializeField] private float transitionDuration = 0.35f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector2Int currentRoom;
    private Coroutine transitionCoroutine;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("RoomLockedCamera requires a player Transform.", this);
            enabled = false;
            return;
        }

        FitRoomToCamera();
        currentRoom = GetRoomCoordinates(player.position);
        transform.position = GetRoomCenter(currentRoom);
    }

    private void LateUpdate()
    {
        Vector2Int playerRoom = GetRoomCoordinates(player.position);
        if (playerRoom == currentRoom)
        {
            return;
        }

        currentRoom = playerRoom;
        StartRoomTransition(currentRoom);
    }

    private Vector2Int GetRoomCoordinates(Vector3 position)
    {
        int roomX = Mathf.FloorToInt((position.x - roomOrigin.x) / roomWidth);
        int roomY = Mathf.FloorToInt((position.y - roomOrigin.y) / roomHeight);
        return new Vector2Int(roomX, roomY);
    }

    private void StartRoomTransition(Vector2Int room)
    {
        Vector3 targetPosition = GetRoomCenter(room);

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        if (transitionDuration <= 0f)
        {
            transform.position = targetPosition;
            return;
        }

        transitionCoroutine = StartCoroutine(TransitionToRoom(targetPosition));
    }

    private Vector3 GetRoomCenter(Vector2Int room)
    {
        return new(
            roomOrigin.x + (room.x * roomWidth) + (roomWidth * 0.5f),
            roomOrigin.y + (room.y * roomHeight) + (roomHeight * 0.5f),
            transform.position.z);
    }

    private System.Collections.IEnumerator TransitionToRoom(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / transitionDuration);
            float easedProgress = transitionCurve.Evaluate(progress);
            transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, easedProgress);
            yield return null;
        }

        transform.position = targetPosition;
        transitionCoroutine = null;
    }

    private void FitRoomToCamera()
    {
        Camera camera = GetComponent<Camera>();
        if (camera == null || !camera.orthographic)
        {
            Debug.LogError("RoomLockedCamera requires an orthographic Camera.", this);
            enabled = false;
            return;
        }

        float verticalSize = roomHeight * 0.5f;
        float horizontalSize = roomWidth / (2f * camera.aspect);
        camera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }
}
