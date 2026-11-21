using UnityEngine;
using UnityEngine.UI;

public class StartingRoomInstructions : MonoBehaviour
{
    [SerializeField] private Vector3 textPosition = new(0f, -3.2f, -0.2f);
    [SerializeField] private string instructions = "WASD / Arrow Keys: Move\nSPACE: Roll";

    private void Awake()
    {
        GameObject canvasObject = new("StartingRoomInstructions");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 2;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        GameObject textObject = new("InstructionsText");
        textObject.transform.SetParent(canvasObject.transform, false);
        textObject.transform.localPosition = textPosition;
        textObject.transform.localScale = Vector3.one * 0.01f;

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(900f, 160f);

        Text text = textObject.AddComponent<Text>();
        text.text = instructions;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 42;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.color = Color.white;
    }
}
