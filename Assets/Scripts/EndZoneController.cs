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

        var panel = new VisualElement();
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
        panel.style.borderTopWidth = 2;
        panel.style.borderBottomWidth = 2;
        panel.style.borderLeftWidth = 2;
        panel.style.borderRightWidth = 2;
        panel.style.borderTopColor = new Color(0.9f, 0.85f, 0.3f);
        panel.style.borderBottomColor = new Color(0.9f, 0.85f, 0.3f);
        panel.style.borderLeftColor = new Color(0.9f, 0.85f, 0.3f);
        panel.style.borderRightColor = new Color(0.9f, 0.85f, 0.3f);
        root.Add(panel);

        var title = new Label("🐾 You have all 3 pieces! Escape?");
        title.style.fontSize = 16;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.color = Color.white;
        title.style.unityTextAlign = TextAnchor.MiddleCenter;
        title.style.whiteSpace = WhiteSpace.Normal;
        title.style.marginBottom = 16;
        panel.Add(title);

        var escapeBtn = new Button(() => {
            UnityEngine.Debug.Log("[EndZone] Escape button clicked!");
            root.style.display = DisplayStyle.None;
            GameManager.Instance.WinGame();
        });
        escapeBtn.text = "Turn in puzzle pieces & Escape!";
        escapeBtn.style.backgroundColor = new Color(0.15f, 0.55f, 0.25f);
        escapeBtn.style.color = Color.white;
        escapeBtn.style.fontSize = 14;
        escapeBtn.style.paddingTop = 10;
        escapeBtn.style.paddingBottom = 10;
        escapeBtn.style.borderTopLeftRadius = 6;
        escapeBtn.style.borderTopRightRadius = 6;
        escapeBtn.style.borderBottomLeftRadius = 6;
        escapeBtn.style.borderBottomRightRadius = 6;
        escapeBtn.style.borderTopWidth = 0;
        escapeBtn.style.borderBottomWidth = 0;
        escapeBtn.style.borderLeftWidth = 0;
        escapeBtn.style.borderRightWidth = 0;
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