using UnityEngine;

public class SpikeTrapSpawner : MonoBehaviour
{
    [SerializeField] private Sprite spikeSprite;
    [SerializeField] private Vector3[] spikePositions =
    {
        new(-29f, -2f, -0.5f),
        new(-21f, -2f, -0.5f),
        new(-13f, -2f, -0.5f),
        new(37f, -32f, -0.5f),
        new(45f, -32f, -0.5f),
        new(53f, -32f, -0.5f),
        new(61f, -32f, -0.5f),
        new(69f, -32f, -0.5f),
        new(-51f, 8f, -0.5f),
        new(-43f, 8f, -0.5f),
        new(-35f, 8f, -0.5f),
        new(-29f, 8f, -0.5f)
    };

    private void Awake()
    {
        if (spikeSprite == null)
        {
            Debug.LogError($"{nameof(SpikeTrapSpawner)} requires a spike sprite.", this);
            return;
        }

        for (int i = 0; i < spikePositions.Length; i++)
        {
            CreateSpike(i, spikePositions[i]);
        }
    }

    private void CreateSpike(int index, Vector3 position)
    {
        GameObject spike = new($"Spike_Extra_{index + 1:00}");
        spike.transform.SetParent(transform, false);
        spike.transform.localPosition = position;

        SpriteRenderer renderer = spike.AddComponent<SpriteRenderer>();
        renderer.sprite = spikeSprite;
        renderer.sortingOrder = 1;

        BoxCollider2D collider = spike.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.8f, 0.8f);

        spike.AddComponent<SpikeTrap>();
    }
}
