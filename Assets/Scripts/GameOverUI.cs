using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private GameObject overlay;
    private RectTransform retryButtonTransform;
    private const string RetryKey = "Space";

    private void Awake()
    {
        CreateOverlay();
    }

    private void Update()
    {
        if (overlay == null || !overlay.activeSelf)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            Retry();
        }
    }

    public void Show()
    {
        if (overlay == null)
        {
            CreateOverlay();
        }

        overlay.SetActive(true);
        Time.timeScale = 0f;
    }

    private void CreateOverlay()
    {
        if (overlay != null)
        {
            return;
        }

        overlay = new GameObject("GameOverOverlay");
        RectTransform overlayTransform = overlay.AddComponent<RectTransform>();
        overlayTransform.SetParent(transform, false);
        overlayTransform.anchorMin = Vector2.zero;
        overlayTransform.anchorMax = Vector2.one;
        overlayTransform.offsetMin = Vector2.zero;
        overlayTransform.offsetMax = Vector2.zero;

        Image background = overlay.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.82f);
        background.raycastTarget = true;

        GameObject titleObject = new GameObject("GameOverTitle");
        RectTransform titleTransform = titleObject.AddComponent<RectTransform>();
        titleTransform.SetParent(overlayTransform, false);
        titleTransform.anchorMin = new Vector2(0.5f, 0.6f);
        titleTransform.anchorMax = new Vector2(0.5f, 0.6f);
        titleTransform.sizeDelta = new Vector2(500f, 90f);

        Text title = titleObject.AddComponent<Text>();
        title.text = "GAME OVER";
        title.alignment = TextAnchor.MiddleCenter;
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 48;
        title.fontStyle = FontStyle.Bold;
        title.color = Color.white;

        GameObject buttonObject = new GameObject("TryAgainButton");
        RectTransform buttonTransform = buttonObject.AddComponent<RectTransform>();
        buttonTransform.SetParent(overlayTransform, false);
        buttonTransform.anchorMin = new Vector2(0.5f, 0.4f);
        buttonTransform.anchorMax = new Vector2(0.5f, 0.4f);
        buttonTransform.sizeDelta = new Vector2(240f, 70f);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.35f, 0.18f, 0.08f, 1f);

        GameObject labelObject = new GameObject("Label");
        RectTransform labelTransform = labelObject.AddComponent<RectTransform>();
        labelTransform.SetParent(buttonTransform, false);
        labelTransform.anchorMin = Vector2.zero;
        labelTransform.anchorMax = Vector2.one;
        labelTransform.offsetMin = Vector2.zero;
        labelTransform.offsetMax = Vector2.zero;

        Text label = labelObject.AddComponent<Text>();
        label.text = "TRY AGAIN";
        label.alignment = TextAnchor.MiddleCenter;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 26;
        label.fontStyle = FontStyle.Bold;
        label.color = Color.white;

        overlay.SetActive(false);
    }

    private void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
