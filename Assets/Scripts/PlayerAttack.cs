using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRadius = 1.25f;
    [SerializeField] private float attackCooldown = 0.45f;
    [SerializeField] private float swingDuration = 0.25f;
    [SerializeField] private float swingAngle = 90f;

    private PlayerMovement playerMovement;
    private float nextAttackTime;
    private bool isAttacking;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null ||
            !mouse.leftButton.wasPressedThisFrame ||
            isAttacking ||
            Time.time < nextAttackTime ||
            playerMovement.IsRolling)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        SoundEffects.PlayAttack(transform.position);
        StartCoroutine(SwingAttack());
    }

    private System.Collections.IEnumerator SwingAttack()
    {
        isAttacking = true;
        Quaternion originalRotation = transform.rotation;
        float startAngle = -swingAngle * 0.5f;
        float endAngle = swingAngle * 0.5f;
        bool hasHit = false;
        float elapsed = 0f;

        while (elapsed < swingDuration)
        {
            float progress = elapsed / swingDuration;
            float angle = Mathf.Lerp(startAngle, endAngle, progress);
            transform.rotation = originalRotation * Quaternion.Euler(0f, 0f, angle);

            if (!hasHit && progress >= 0.5f)
            {
                DamageEnemies();
                hasHit = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = originalRotation;
        isAttacking = false;
    }

    private void DamageEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                SoundEffects.PlayHit(hit.transform.position);
                continue;
            }

            BossEnemy boss = hit.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                SoundEffects.PlayHit(hit.transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
