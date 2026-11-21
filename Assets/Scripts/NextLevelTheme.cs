using UnityEngine;
using UnityEngine.UI;

public class NextLevelTheme : MonoBehaviour
{
    [SerializeField] private Color backgroundColor = new(0.035f, 0.02f, 0.1f, 1f);
    [SerializeField] private Color crystalColor = new(0.15f, 0.8f, 1f, 0.9f);

    private readonly Transform[] crystals = new Transform[4];

    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.backgroundColor = backgroundColor;
        }

        CreateLevelTitle();
        CreateCrystals();

        SpriteRenderer playerRenderer = GameObject.Find("Player")?.GetComponent<SpriteRenderer>();
        if (playerRenderer != null)
        {
            playerRenderer.color = new Color(0.75f, 0.95f, 1f, 1f);
        }
    }

    private void Update()
    {
        for (int i = 0; i < crystals.Length; i++)
        {
            if (crystals[i] == null)
            {
                continue;
            }

            float pulse = 1f + Mathf.Sin(Time.time * 2.5f + i) * 0.1f;
            crystals[i].localScale = Vector3.one * pulse;
            crystals[i].localRotation = Quaternion.Euler(0f, 0f, Time.time * 35f + i * 90f);
        }
    }

    private void CreateLevelTitle()
    {
        GameObject canvasObject = new("NextLevelTitle");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 3;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        GameObject textObject = new("TitleText");
        textObject.transform.SetParent(canvasObject.transform, false);
        textObject.transform.localPosition = new Vector3(0f, 3.25f, -0.2f);
        textObject.transform.localScale = Vector3.one * 0.01f;

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1000f, 120f);

        Text text = textObject.AddComponent<Text>();
        text.text = "LEVEL 2  //  THE LOWER VAULT";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 42;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.color = crystalColor;
    }

    private void CreateCrystals()
    {
        Vector3[] positions =
        {
            new(-8f, 3.5f, -0.4f),
            new(8f, 3.5f, -0.4f),
            new(-8f, -3.5f, -0.4f),
            new(8f, -3.5f, -0.4f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject crystal = new($"Level2Crystal_{i + 1}");
            crystal.transform.position = positions[i];

            SpriteRenderer renderer = crystal.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(CreateCrystalTexture(), new Rect(0f, 0f, 24f, 24f), new Vector2(0.5f, 0.5f), 24f);
            renderer.color = crystalColor;
            renderer.sortingOrder = 2;
            crystals[i] = crystal.transform;
        }
    }

    private Texture2D CreateCrystalTexture()
    {
        const int size = 24;
        Texture2D texture = new(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float horizontal = Mathf.Abs(x - (size - 1) * 0.5f) / (size * 0.5f);
                float vertical = Mathf.Abs(y - (size - 1) * 0.5f) / (size * 0.5f);
                float alpha = Mathf.Clamp01(1f - horizontal - vertical);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
