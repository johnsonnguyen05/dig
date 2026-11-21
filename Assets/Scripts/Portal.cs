using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "NextLevel";
    [SerializeField] private Color portalColor = new(0.35f, 0.1f, 1f, 0.9f);
    [SerializeField] private float visualScale = 1.5f;
    [SerializeField] private float pulseAmount = 0.12f;
    [SerializeField] private float pulseSpeed = 4f;

    private Transform visual;
    private bool isLoading;

    private void Awake()
    {
        BoxCollider2D trigger = GetComponent<BoxCollider2D>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<BoxCollider2D>();
        }

        trigger.isTrigger = true;
        trigger.size = Vector2.one;
        CreateVisual();
    }

    private void Update()
    {
        if (visual == null)
        {
            return;
        }

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        visual.localScale = Vector3.one * visualScale * pulse;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLoading || other.GetComponentInParent<PlayerMovement>() == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogError($"{nameof(Portal)} requires a target scene name.", this);
            return;
        }

        isLoading = true;
        SoundEffects.PlayPortal(transform.position);
        StartCoroutine(LoadTargetScene());
    }

    private System.Collections.IEnumerator LoadTargetScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(targetSceneName);
    }

    private void CreateVisual()
    {
        GameObject visualObject = new("PortalVisual");
        visualObject.transform.SetParent(transform, false);
        visualObject.transform.localPosition = new Vector3(0f, 0f, 0.1f);

        SpriteRenderer renderer = visualObject.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(CreatePortalTexture(), new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f), 32f);
        renderer.color = portalColor;
        renderer.sortingOrder = 2;
        visual = visualObject.transform;
    }

    private Texture2D CreatePortalTexture()
    {
        const int textureSize = 32;
        Texture2D texture = new(textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[textureSize * textureSize];
        Vector2 center = new(textureSize * 0.5f, textureSize * 0.5f);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / (textureSize * 0.5f);
                float edge = 1f - Mathf.SmoothStep(0.78f, 1f, distance);
                float ring = Mathf.Clamp01(1f - Mathf.Abs(distance - 0.68f) / 0.12f);
                float centerGlow = Mathf.Clamp01(1f - distance / 0.8f);
                float alpha = Mathf.Clamp01(edge * (0.65f + ring * 0.35f));
                Color pixelColor = Color.Lerp(portalColor, Color.white, Mathf.Clamp01(ring + centerGlow * 0.25f));
                pixels[y * textureSize + x] = new Color(pixelColor.r, pixelColor.g, pixelColor.b, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
