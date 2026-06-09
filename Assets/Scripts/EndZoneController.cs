using UnityEngine;
using UnityEngine.UIElements;

public class EndZoneController : MonoBehaviour
{
    public UIDocument endUI;
    private VisualElement root;

    void Start()
    {
        root = endUI.rootVisualElement;
        root.style.display = DisplayStyle.None;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // open if all puzzles are done
        if (GameManager.Instance.AllPuzzlesComplete())
        {
            ShowEscapeUI();
        }
        else
        {
            ShowNotReadyUI();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        root.style.display = DisplayStyle.None;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void ShowEscapeUI()
    {
        root.Clear();
        root.style.display = DisplayStyle.Flex;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        var panel = new ScrollView();
        panel.style.backgroundColor = new Color(0.10f, 0.10f, 0.18f, 0.95f);
        panel.style.paddingTop = 24;
        panel.style.paddingBottom = 24;
        panel.style.paddingLeft = 28;
        panel.style.paddingRight = 28;
        panel.style.borderTopLeftRadius = 12;
        panel.style.borderTopRightRadius = 12;
        panel.style.borderBottomLeftRadius = 12;
        panel.style.borderBottomRightRadius = 12;
        panel.style.width = 340;
        panel.style.maxHeight = Length.Percent(70);
        panel.style.borderTopWidth = 2;
        panel.style.borderBottomWidth = 2;
        panel.style.borderLeftWidth = 2;
        panel.style.borderRightWidth = 2;
        panel.style.borderTopColor = new Color(0.9f, 0.85f, 0.3f);
        panel.style.borderBottomColor = new Color(0.9f, 0.85f, 0.3f);
        panel.style.borderLeftColor = new Color(0.9f, 0.85f, 0.3f);
        panel.style.borderRightColor = new Color(0.9f, 0.85f, 0.3f);
        root.Add(panel);

        var icon = new Label("🐾  THE QUAD");
        icon.style.fontSize = 20;
        icon.style.unityTextAlign = TextAnchor.MiddleCenter;
        icon.style.color = new Color(0.9f, 0.85f, 0.3f);
        icon.style.marginBottom = 8;
        panel.Add(icon);

        var title = new Label("Escape?");
        title.style.fontSize = 18;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.color = Color.white;
        title.style.unityTextAlign = TextAnchor.MiddleCenter;
        title.style.marginBottom = 8;
        panel.Add(title);

        var sub = new Label("You have all 3 puzzle pieces!\nTurn them in and escape the goose!");
        sub.style.fontSize = 13;
        sub.style.color = new Color(0.75f, 0.85f, 1f);
        sub.style.whiteSpace = WhiteSpace.Normal;
        sub.style.unityTextAlign = TextAnchor.MiddleCenter;
        sub.style.marginBottom = 20;
        panel.Add(sub);

        var escapeBtn = new Button(() => GameManager.Instance.WinGame());
        escapeBtn.text = "Turn in puzzle pieces & Escape!";
        escapeBtn.style.backgroundColor = new Color(0.15f, 0.55f, 0.25f);
        escapeBtn.style.color = Color.white;
        escapeBtn.style.fontSize = 14;
        escapeBtn.style.paddingTop = 10;
        escapeBtn.style.paddingBottom = 10;
        escapeBtn.style.marginBottom = 8;
        escapeBtn.style.borderTopLeftRadius = 6;
        escapeBtn.style.borderTopRightRadius = 6;
        escapeBtn.style.borderBottomLeftRadius = 6;
        escapeBtn.style.borderBottomRightRadius = 6;
        panel.Add(escapeBtn);
    }

    void ShowNotReadyUI()
    {
        root.Clear();
        root.style.display = DisplayStyle.Flex;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        var panel = new VisualElement();
        panel.style.backgroundColor = new Color(0.10f, 0.10f, 0.18f, 0.95f);
        panel.style.paddingTop = 18;
        panel.style.paddingBottom = 18;
        panel.style.paddingLeft = 22;
        panel.style.paddingRight = 22;
        panel.style.borderTopLeftRadius = 10;
        panel.style.borderTopRightRadius = 10;
        panel.style.borderBottomLeftRadius = 10;
        panel.style.borderBottomRightRadius = 10;
        panel.style.width = 320;
        root.Add(panel);

        var title = new Label("Not so fast! 🦢");
        title.style.fontSize = 16;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.color = Color.white;
        title.style.unityTextAlign = TextAnchor.MiddleCenter;
        title.style.marginBottom = 8;
        panel.Add(title);

        var sub = new Label($"You need all 3 puzzle pieces first!\nPieces collected: {GameManager.Instance.PuzzlesCompleted()}/3");
        sub.style.fontSize = 13;
        sub.style.color = new Color(0.75f, 0.85f, 1f);
        sub.style.whiteSpace = WhiteSpace.Normal;
        sub.style.unityTextAlign = TextAnchor.MiddleCenter;
        panel.Add(sub);
    }
}