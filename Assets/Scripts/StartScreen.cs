using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private float fireFramesPerSecond = 12f;

    private const string GameplaySceneName = "SampleScene";
    private readonly List<Image> fireImages = new();
    private Sprite[] fireFrames;
    private float fireAnimationTimer;
    private int fireFrameIndex;

    private void Awake()
    {
        LoadFireFrames();
        CreateEventSystem();
        CreateMenu();
    }

    private void Update()
    {
        if (fireFrames == null || fireFrames.Length == 0 || fireImages.Count == 0)
        {
            return;
        }

        fireAnimationTimer += Time.unscaledDeltaTime;
        if (fireAnimationTimer < 1f / fireFramesPerSecond)
        {
            return;
        }

        fireAnimationTimer = 0f;
        fireFrameIndex = (fireFrameIndex + 1) % fireFrames.Length;
        foreach (Image fireImage in fireImages)
        {
            fireImage.sprite = fireFrames[fireFrameIndex];
        }
    }

    private void LoadFireFrames()
    {
        fireFrames = Resources.LoadAll<Sprite>("MenuFire");
        Array.Sort(fireFrames, (left, right) => GetFrameNumber(left.name).CompareTo(GetFrameNumber(right.name)));

        if (fireFrames.Length == 0 && fireSprite != null)
        {
            fireFrames = new[] { fireSprite };
        }
    }

    private static int GetFrameNumber(string spriteName)
    {
        int separatorIndex = spriteName.LastIndexOf('_');
        return int.TryParse(spriteName[(separatorIndex + 1)..], out int frameNumber) ? frameNumber : 0;
    }

    private void CreateEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        GameObject eventSystemObject = new("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private void CreateMenu()
    {
        GameObject canvasObject = new("StartScreenCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        Image background = canvasObject.AddComponent<Image>();
        background.color = new Color(0.04f, 0.025f, 0.02f, 1f);

        CreateFireArt(canvasObject.transform, new Vector2(0.18f, 0.22f), new Vector2(280f, 560f), 0.8f);
        CreateFireArt(canvasObject.transform, new Vector2(0.82f, 0.22f), new Vector2(280f, 560f), 0.8f);
        CreateFireArt(canvasObject.transform, new Vector2(0.5f, 0.08f), new Vector2(420f, 260f), 0.55f);
        CreateText(canvasObject.transform, "EMBER", new Vector2(0.5f, 0.7f), new Vector2(700f, 150f), 86);
        CreateButton(canvasObject.transform, "START", new Vector2(0.5f, 0.48f), StartGame);
        CreateButton(canvasObject.transform, "EXIT GAME", new Vector2(0.5f, 0.34f), ExitGame);
    }

    private static Text CreateText(Transform parent, string textValue, Vector2 anchor, Vector2 size, int fontSize)
    {
        GameObject textObject = new(textValue);
        RectTransform textTransform = textObject.AddComponent<RectTransform>();
        textTransform.SetParent(parent, false);
        textTransform.anchorMin = anchor;
        textTransform.anchorMax = anchor;
        textTransform.anchoredPosition = Vector2.zero;
        textTransform.sizeDelta = size;

        Text text = textObject.AddComponent<Text>();
        text.text = textValue;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.BoldAndItalic;
        text.color = new Color(1f, 0.72f, 0.32f, 1f);
        Shadow shadow = textObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
        shadow.effectDistance = new Vector2(5f, -5f);
        Outline outline = textObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.2f, 0.04f, 0.01f, 1f);
        outline.effectDistance = new Vector2(2f, 2f);
        return text;
    }

    private void CreateFireArt(Transform parent, Vector2 anchor, Vector2 size, float alpha)
    {
        if (fireSprite == null)
        {
            Debug.LogError("StartScreen requires a fire sprite.", this);
            return;
        }

        GameObject fireObject = new("FireArt");
        RectTransform fireTransform = fireObject.AddComponent<RectTransform>();
        fireTransform.SetParent(parent, false);
        fireTransform.anchorMin = anchor;
        fireTransform.anchorMax = anchor;
        fireTransform.anchoredPosition = Vector2.zero;
        fireTransform.sizeDelta = size;

        Image fireImage = fireObject.AddComponent<Image>();
        fireImage.sprite = fireFrames.Length > 0 ? fireFrames[0] : fireSprite;
        fireImage.preserveAspect = true;
        fireImage.color = new Color(1f, 0.38f, 0.08f, alpha);
        fireImage.raycastTarget = false;
        fireImages.Add(fireImage);
    }

    private static void CreateButton(Transform parent, string label, Vector2 anchor, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new(label);
        RectTransform buttonTransform = buttonObject.AddComponent<RectTransform>();
        buttonTransform.SetParent(parent, false);
        buttonTransform.anchorMin = anchor;
        buttonTransform.anchorMax = anchor;
        buttonTransform.anchoredPosition = Vector2.zero;
        buttonTransform.sizeDelta = new Vector2(360f, 84f);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.45f, 0.18f, 0.08f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(action);

        CreateText(buttonTransform, label, new Vector2(0.5f, 0.5f), new Vector2(320f, 70f), 30);
    }

    private static void StartGame()
    {
        SceneManager.LoadScene(GameplaySceneName);
    }

    private static void ExitGame()
    {
        Application.Quit();
    }
}
