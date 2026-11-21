using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private Image[] healthIcons;
    [SerializeField] private float damageCooldown = 0.75f;
    [SerializeField] private float hurtDuration = 0.2f;

    private int currentHealth;
    private float nextDamageTime;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private Color originalSpriteColor;
    private Coroutine hurtCoroutine;

    public bool IsInvulnerable => playerMovement != null && playerMovement.IsRolling;

    private void Awake()
    {
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSpriteColor = spriteRenderer.color;
        }

        RefreshHealthIcons();
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 ||
            IsInvulnerable ||
            Time.time < nextDamageTime ||
            currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        nextDamageTime = Time.time + damageCooldown;
        SoundEffects.PlayPlayerHurt(transform.position);
        RefreshHealthIcons();
        PlayHurtAnimation();

        if (currentHealth == 0)
        {
            SoundEffects.PlayPlayerDeath(transform.position);
            playerMovement.enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            GameOverUI gameOver = FindAnyObjectByType<GameOverUI>();
            if (gameOver != null)
            {
                gameOver.Show();
            }
        }
    }

    private void RefreshHealthIcons()
    {
        if (healthIcons == null)
        {
            return;
        }

        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] != null)
            {
                healthIcons[i].enabled = i < currentHealth;
            }
        }
    }

    private void PlayHurtAnimation()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (hurtCoroutine != null)
        {
            StopCoroutine(hurtCoroutine);
        }

        hurtCoroutine = StartCoroutine(HurtAnimation());
    }

    private System.Collections.IEnumerator HurtAnimation()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hurtDuration);
        spriteRenderer.color = originalSpriteColor;
        hurtCoroutine = null;
    }
}
