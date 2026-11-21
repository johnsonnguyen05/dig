using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private Sprite fixedSpikeSprite;
    [SerializeField] private TileBase fixedSpikeTile;
    [SerializeField] private int damage = 1;

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;
    private Renderer trapRenderer;

    private static readonly Dictionary<SpriteRenderer, int> OriginalPlayerSortingOrders = new();
    private static readonly Dictionary<SpriteRenderer, int> PlayerSpikeContacts = new();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
        trapRenderer = GetComponent<Renderer>();

        if (tilemap != null)
        {
            if (fixedSpikeTile != null)
            {
                Vector3Int tilePosition = tilemap.WorldToCell(transform.position);
                tilemap.SetTile(tilePosition, fixedSpikeTile);
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            return;
        }

        if (spriteRenderer != null)
        {
            if (fixedSpikeSprite == null)
            {
                fixedSpikeSprite = spriteRenderer.sprite;
            }

            spriteRenderer.sprite = fixedSpikeSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null && !health.IsInvulnerable)
        {
            SoundEffects.PlaySpike(transform.position);
            health.TakeDamage(damage);
        }

        SpriteRenderer playerRenderer = other.GetComponent<SpriteRenderer>();
        if (health != null && playerRenderer != null)
        {
            RegisterPlayerContact(playerRenderer);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SpriteRenderer playerRenderer = other.GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            UnregisterPlayerContact(playerRenderer);
        }
    }

    private void RegisterPlayerContact(SpriteRenderer playerRenderer)
    {
        if (!PlayerSpikeContacts.ContainsKey(playerRenderer))
        {
            OriginalPlayerSortingOrders[playerRenderer] = playerRenderer.sortingOrder;
            PlayerSpikeContacts[playerRenderer] = 0;
        }

        PlayerSpikeContacts[playerRenderer]++;
        playerRenderer.sortingOrder = Mathf.Max(
            playerRenderer.sortingOrder,
            (trapRenderer != null ? trapRenderer.sortingOrder : 0) + 1);
    }

    private static void UnregisterPlayerContact(SpriteRenderer playerRenderer)
    {
        if (!PlayerSpikeContacts.TryGetValue(playerRenderer, out int contactCount))
        {
            return;
        }

        contactCount--;
        if (contactCount > 0)
        {
            PlayerSpikeContacts[playerRenderer] = contactCount;
            return;
        }

        if (OriginalPlayerSortingOrders.TryGetValue(playerRenderer, out int originalSortingOrder))
        {
            playerRenderer.sortingOrder = originalSortingOrder;
            OriginalPlayerSortingOrders.Remove(playerRenderer);
        }

        PlayerSpikeContacts.Remove(playerRenderer);
    }
}
