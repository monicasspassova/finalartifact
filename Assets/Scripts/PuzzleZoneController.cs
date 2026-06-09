using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Attach to PuzzleZone1 (alongside a Collider set to IsTrigger).
/// Assign puzzleUI to the UIDocument on the same object (using PuzzleZoneUI.uxml).
/// Randomly picks a puzzle each time the player enters; re-rolls on wrong answer.
/// Triggers 30-second goose aggro on every submit or give-up.
/// </summary>
public class PuzzleZoneController : MonoBehaviour
{
    public UIDocument puzzleUI;
    public int puzzleID = 1;
    public enum PuzzleType { Random, Matching, Trivia, Scramble }
    public PuzzleType puzzleType = PuzzleType.Random;

    public bool completed = false;

    private VisualElement root;
    private VisualElement panel;
    private int lastPuzzleShown = -1;

    // ---- Matching puzzle data ----
    // Left column (fixed order)
    private static readonly string[] LeftItems = { "Suzzallo", "Oak Hall", "Local Point", "Kane Hall" };
    // Right column displayed in shuffled order
    private static readonly string[] RightDisplayItems = { "Cafeteria", "Library", "Lecture Hall", "Dorm" };
    // correctRightDisplayIdx[leftIdx] = the rightDisplayItems index that pairs correctly
    // Suzzallo(0)->Library(rightDisplay[1]), OakHall(1)->Dorm(rightDisplay[3]),
    // LocalPoint(2)->Cafeteria(rightDisplay[0]), KaneHall(3)->LectureHall(rightDisplay[2])
    private static readonly int[] CorrectRightIdx = { 1, 3, 0, 2 };

    private int selectedLeft = -1;
    private readonly Dictionary<int, int> pairings = new Dictionary<int, int>(); // leftIdx -> rightDisplayIdx

    // ---- Trivia data ----
    private struct TriviaQ
    {
        public string question;
        public string[] options;
        public int correctIndex;
    }

    private static readonly TriviaQ[] TriviaPool = new TriviaQ[]
    {
        new TriviaQ {
            question = "What is the official street name for \"The Ave\"?",
            options = new[] { "University Avenue", "University Village", "The Avenue", "University Way" },
            correctIndex = 3
        },
        new TriviaQ {
            question = "What kind of dog is Dubs II, the current live mascot of UW?",
            options = new[] { "Shih Tzu", "Husky", "Golden Retriever", "Alaskan Malamute", "Corgi" },
            correctIndex = 3
        }
    };

    // ---- Style helpers ----
    private static readonly Color ColorDefault   = new Color(0.25f, 0.25f, 0.35f);
    private static readonly Color ColorSelected  = new Color(0.20f, 0.50f, 0.90f);
    private static readonly Color ColorPaired    = new Color(0.20f, 0.65f, 0.30f);
    private static readonly Color ColorCorrect   = new Color(0.15f, 0.75f, 0.25f);
    private static readonly Color ColorWrong     = new Color(0.80f, 0.20f, 0.20f);

    void Start()
    {
        root = puzzleUI.rootVisualElement;
        root.style.display = DisplayStyle.None;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || completed) return;
        ShowRandomPuzzle();
    }

    void ShowRandomPuzzle()
    {
        root = puzzleUI.rootVisualElement;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        if (puzzleType == PuzzleType.Matching) { BuildMatchingUI(); return; }
        if (puzzleType == PuzzleType.Trivia)   { BuildTriviaUI(Random.Range(0, TriviaPool.Length)); return; }
        if (puzzleType == PuzzleType.Scramble) { BuildScrambleUI(); return; }

        // random fallback
        int pick;
        do { pick = Random.Range(0, 3); } while (pick == lastPuzzleShown);
        lastPuzzleShown = pick;
        if (pick == 0)      BuildMatchingUI();
        else if (pick == 1) BuildTriviaUI(0);
        else                BuildTriviaUI(1);
    }

    // ================================================================
    //  MATCHING PUZZLE
    // ================================================================

    void BuildMatchingUI()
    {
        selectedLeft = -1;
        pairings.Clear();

        root.Clear();
        root.style.display = DisplayStyle.Flex;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;

        panel = MakePanel();
        root.Add(panel);

        AddLabel(panel, "Connect the buildings to their descriptions!", 15, FontStyle.Bold, Color.white, 0, 10);
        AddLabel(panel, "Click a building, then click its match.", 11, FontStyle.Normal, new Color(0.8f, 0.8f, 0.8f), 0, 8);

        var feedback = new Label("");
        feedback.name = "feedback";
        feedback.style.fontSize = 14;
        feedback.style.color = Color.yellow;
        feedback.style.marginBottom = 10;
        feedback.style.whiteSpace = WhiteSpace.Normal;
        panel.Add(feedback);

        // Two-column row
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.justifyContent = Justify.SpaceBetween;
        row.style.marginBottom = 16;
        panel.Add(row);

        var leftCol = MakeColumn();
        var rightCol = MakeColumn();
        row.Add(leftCol);
        row.Add(rightCol);

        for (int i = 0; i < LeftItems.Length; i++)
        {
            int idx = i;
            var btn = MakeBtn(LeftItems[i], () => OnLeftClick(idx));
            btn.name = $"left-{i}";
            leftCol.Add(btn);
        }

        for (int i = 0; i < RightDisplayItems.Length; i++)
        {
            int idx = i;
            var btn = MakeBtn(RightDisplayItems[i], () => OnRightClick(idx));
            btn.name = $"right-{i}";
            rightCol.Add(btn);
        }

        var submitBtn = MakeBtn("Submit Answers", OnMatchingSubmit);
        submitBtn.style.marginTop = 8;
        panel.Add(submitBtn);

        var giveUpBtn = MakeBtn("Give Up  (Goose incoming!)", OnGiveUp);
        StyleAsSecondary(giveUpBtn);
        panel.Add(giveUpBtn);
    }

    void OnLeftClick(int leftIdx)
    {
        selectedLeft = leftIdx;
        RefreshMatchingColors();
    }

    void OnRightClick(int rightIdx)
    {
        if (selectedLeft < 0) return;
        pairings[selectedLeft] = rightIdx;
        selectedLeft = -1;
        RefreshMatchingColors();
    }

    void RefreshMatchingColors()
    {
        var usedRight = new HashSet<int>(pairings.Values);

        for (int i = 0; i < LeftItems.Length; i++)
        {
            var btn = panel.Q<Button>($"left-{i}");
            if (btn == null) continue;
            if (i == selectedLeft)          SetBtnColor(btn, ColorSelected);
            else if (pairings.ContainsKey(i)) SetBtnColor(btn, ColorPaired);
            else                             SetBtnColor(btn, ColorDefault);
        }

        for (int i = 0; i < RightDisplayItems.Length; i++)
        {
            var btn = panel.Q<Button>($"right-{i}");
            if (btn == null) continue;
            SetBtnColor(btn, usedRight.Contains(i) ? ColorPaired : ColorDefault);
        }
    }

    void OnMatchingSubmit()
    {
        bool allCorrect = pairings.Count == LeftItems.Length;
        if (allCorrect)
        {
            for (int i = 0; i < LeftItems.Length; i++)
            {
                if (!pairings.ContainsKey(i) || pairings[i] != CorrectRightIdx[i])
                {
                    allCorrect = false;
                    break;
                }
            }
        }

        if (allCorrect)
        {
            ShowPuzzlePiece();
        }
        else
        {
            GameManager.Instance.TriggerGooseAggro(15f);

            // Show which pairs were wrong before closing
            for (int i = 0; i < LeftItems.Length; i++)
            {
                var leftBtn = panel.Q<Button>($"left-{i}");
                if (leftBtn == null) continue;
                bool correct = pairings.ContainsKey(i) && pairings[i] == CorrectRightIdx[i];
                SetBtnColor(leftBtn, correct ? ColorCorrect : ColorWrong);
            }

            var feedback = panel.Q<Label>("feedback");
            if (feedback != null)
                feedback.text = "Not quite — the goose is coming! Re-enter the zone to try again.";

            var submitBtn = panel.Q<Button>("submit-btn");
            if (submitBtn != null) submitBtn.SetEnabled(false);

            StartCoroutine(CloseAfterDelay(2.5f));
        }
    }

    // ================================================================
    //  TRIVIA PUZZLE
    // ================================================================

    void BuildTriviaUI(int questionIndex)
    {
        var q = TriviaPool[questionIndex];

        root.Clear();
        root.style.display = DisplayStyle.Flex;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;

        panel = MakePanel();
        root.Add(panel);

        AddLabel(panel, "UW Trivia", 17, FontStyle.Bold, Color.white, 0, 5);
        AddLabel(panel, q.question, 13, FontStyle.Normal, new Color(0.95f, 0.95f, 0.95f), 0, 12);

        var feedback = new Label("");
        feedback.name = "feedback";
        feedback.style.fontSize = 14;
        feedback.style.color = Color.yellow;
        feedback.style.marginBottom = 10;
        feedback.style.whiteSpace = WhiteSpace.Normal;
        panel.Add(feedback);

        for (int i = 0; i < q.options.Length; i++)
        {
            int idx = i;
            var btn = MakeBtn(q.options[i], () => OnTriviaAnswer(idx, q.correctIndex));
            btn.name = $"answer-{i}";
            panel.Add(btn);
        }

        var giveUpBtn = MakeBtn("Give Up  (Goose incoming!)", OnGiveUp);
        StyleAsSecondary(giveUpBtn);
        giveUpBtn.style.marginTop = 14;
        panel.Add(giveUpBtn);
    }

    void OnTriviaAnswer(int chosen, int correct)
    {
        UnityEngine.Debug.Log("[Puzzle] Trivia answered. chosen=" + chosen + " correct=" + correct);

        // Highlight correct / wrong
        for (int i = 0; i < 5; i++)
        {
            var btn = panel.Q<Button>($"answer-{i}");
            if (btn == null) continue;
            if (i == correct)       SetBtnColor(btn, ColorCorrect);
            else if (i == chosen)   SetBtnColor(btn, ColorWrong);
            btn.SetEnabled(false);
        }

        var feedback = panel.Q<Label>("feedback");
        if (chosen == correct)
        {
            if (feedback != null) feedback.text = "Correct!";
            UnityEngine.Debug.Log("[Puzzle] Correct! Starting puzzle piece delay.");
            StartCoroutine(ShowPuzzlePieceAfterDelay(1.0f));
        }
        else
        {
            GameManager.Instance.TriggerGooseAggro(15f);
            if (feedback != null) feedback.text = "Wrong! The goose is coming — re-enter to try again.";
            StartCoroutine(CloseAfterDelay(2.5f));
        }
    }

    // ================================================================
//  SCRAMBLE PUZZLE
// ================================================================

private string scrambleAnswer = "";

void BuildScrambleUI()
{
    string[] words = { "SUZZALLO", "HUSKY", "DAWGS", "QUAD", "KANE" };
    string correct = words[Random.Range(0, words.Length)];
    scrambleAnswer = correct;
    string scrambled = ScrambleWord(correct);

    root.Clear();
    root.style.display = DisplayStyle.Flex;
    root.style.alignItems = Align.Center;
    root.style.justifyContent = Justify.Center;

    panel = MakePanel();
    root.Add(panel);

    AddLabel(panel, "Unscramble the UW word!", 17, FontStyle.Bold, Color.white, 0, 8);
    AddLabel(panel, scrambled, 28, FontStyle.Bold, new Color(0.9f, 0.85f, 0.3f), 0, 16);

    var feedback = new Label("");
    feedback.name = "feedback";
    feedback.style.fontSize = 13;
    feedback.style.color = Color.yellow;
    feedback.style.marginBottom = 8;
    feedback.style.whiteSpace = WhiteSpace.Normal;
    panel.Add(feedback);

    var inputField = new TextField();
    inputField.name = "scramble-input";
    inputField.style.marginBottom = 10;
    inputField.style.color = Color.white;
    inputField.style.fontSize = 14;
    panel.Add(inputField);

    var submitBtn = MakeBtn("Submit", () =>
    {
        string answer = inputField.value.Trim().ToUpper();
        var fb = panel.Q<Label>("feedback");
        if (answer == scrambleAnswer)
        {
            if (fb != null) fb.text = "Correct! 🎉";
            StartCoroutine(ShowPuzzlePieceAfterDelay(1.0f));
        }
        else
        {
            GameManager.Instance.TriggerGooseAggro(15f);
            if (fb != null) fb.text = "Wrong! The goose is coming!";
            StartCoroutine(CloseAfterDelay(2.5f));
        }
    });
    panel.Add(submitBtn);

    var giveUpBtn = MakeBtn("Give Up  (Goose incoming!)", OnGiveUp);
    StyleAsSecondary(giveUpBtn);
    panel.Add(giveUpBtn);
}

string ScrambleWord(string word)
{
    char[] chars = word.ToCharArray();
    for (int i = chars.Length - 1; i > 0; i--)
    {
        int j = Random.Range(0, i + 1);
        (chars[i], chars[j]) = (chars[j], chars[i]);
    }
    // Make sure it's not the same as the original
    if (new string(chars) == word) return ScrambleWord(word);
    return new string(chars);
}

    // ================================================================
    //  SHARED
    // ================================================================

    void OnGiveUp()
    {
        GameManager.Instance.TriggerGooseAggro(15f);
        CloseUI();
    }

    void OnCorrect()
    {
        CloseUI();
        completed = true;
        GameManager.Instance.PuzzleCompleted(puzzleID);
    }

    void ShowPuzzlePiece()
    {
        UnityEngine.Debug.Log("[Puzzle] ShowPuzzlePiece called. root=" + root);
        root.Clear();
        root.style.display = DisplayStyle.Flex;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;
        root.style.position = Position.Absolute;
        root.style.left   = 0; root.style.right  = 0;
        root.style.top    = 0; root.style.bottom = 0;
        root.style.backgroundColor = new Color(0f, 0f, 0f, 0.6f); // dim overlay
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        var p = new VisualElement();
        p.style.backgroundColor = new Color(0.08f, 0.12f, 0.22f, 0.97f);
        p.style.paddingTop    = 24;
        p.style.paddingBottom = 24;
        p.style.paddingLeft   = 28;
        p.style.paddingRight  = 28;
        p.style.borderTopLeftRadius     = 12;
        p.style.borderTopRightRadius    = 12;
        p.style.borderBottomLeftRadius  = 12;
        p.style.borderBottomRightRadius = 12;
        p.style.width = 320;
        p.style.borderTopWidth    = 2;
        p.style.borderBottomWidth = 2;
        p.style.borderLeftWidth   = 2;
        p.style.borderRightWidth  = 2;
        p.style.borderTopColor    = new Color(0.3f, 0.7f, 1f);
        p.style.borderBottomColor = new Color(0.3f, 0.7f, 1f);
        p.style.borderLeftColor   = new Color(0.3f, 0.7f, 1f);
        p.style.borderRightColor  = new Color(0.3f, 0.7f, 1f);
        root.Add(p);

        var icon = new Label("🧩  PUZZLE PIECE");
        icon.style.fontSize = 20;
        icon.style.unityTextAlign = TextAnchor.MiddleCenter;
        icon.style.marginBottom = 8;
        icon.style.color = new Color(0.9f, 0.85f, 0.3f);
        p.Add(icon);

        var title = new Label("Puzzle Piece Found!");
        title.style.fontSize = 16;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.color = Color.white;
        title.style.unityTextAlign = TextAnchor.MiddleCenter;
        title.style.marginBottom = 6;
        p.Add(title);

        var sub = new Label("You collected a piece of the campus map.");
        sub.style.fontSize = 13;
        sub.style.color = new Color(0.75f, 0.85f, 1f);
        sub.style.whiteSpace = WhiteSpace.Normal;
        sub.style.unityTextAlign = TextAnchor.MiddleCenter;
        sub.style.marginBottom = 16;
        p.Add(sub);

        var okBtn = MakeBtn("OK  —  Watch out for the goose!", () =>
        {
            UnityEngine.Debug.Log("[Puzzle] OK clicked! Triggering aggro and completing puzzle.");
            GameManager.Instance.TriggerGooseAggro(15f);
            completed = true;
            GameManager.Instance.PuzzleCompleted(puzzleID);
            CloseUI();
        });
        okBtn.style.backgroundColor = new Color(0.15f, 0.45f, 0.75f);
        okBtn.style.fontSize = 14;
        okBtn.style.paddingTop    = 10;
        okBtn.style.paddingBottom = 10;
        p.Add(okBtn);
    }

    IEnumerator ShowPuzzlePieceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowPuzzlePiece();
    }

    IEnumerator CorrectThenClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnCorrect();
    }

    IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseUI();
    }

    void CloseUI()
    {
        if (root != null)
            root.style.display = DisplayStyle.None;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void Reset()
    {
        CloseUI();
        completed = false;
        lastPuzzleShown = -1;
    }

    // ================================================================
    //  UI HELPERS
    // ================================================================

    VisualElement MakePanel()
    {
        var p = new VisualElement();
        p.style.backgroundColor = new Color(0.10f, 0.10f, 0.18f, 0.95f);
        p.style.paddingTop    = 18;
        p.style.paddingBottom = 18;
        p.style.paddingLeft   = 22;
        p.style.paddingRight  = 22;
        p.style.borderTopLeftRadius     = 10;
        p.style.borderTopRightRadius    = 10;
        p.style.borderBottomLeftRadius  = 10;
        p.style.borderBottomRightRadius = 10;
        p.style.width = 360;
        p.style.maxWidth = Length.Percent(85);
        p.style.maxHeight = Length.Percent(80);
        p.style.overflow = Overflow.Hidden;
        return p;
    }

    VisualElement MakeColumn()
    {
        var col = new VisualElement();
        col.style.width = Length.Percent(47);
        return col;
    }

    void AddLabel(VisualElement parent, string text, int fontSize, FontStyle style, Color color, float marginTop, float marginBottom)
    {
        var lbl = new Label(text);
        lbl.style.fontSize = fontSize;
        lbl.style.unityFontStyleAndWeight = style;
        lbl.style.color = color;
        lbl.style.whiteSpace = WhiteSpace.Normal;
        lbl.style.marginTop = marginTop;
        lbl.style.marginBottom = marginBottom;
        parent.Add(lbl);
    }

    Button MakeBtn(string text, System.Action callback)
    {
        var btn = new Button(callback);
        btn.text = text;
        btn.style.marginBottom = 5;
        btn.style.paddingTop    = 6;
        btn.style.paddingBottom = 6;
        btn.style.fontSize = 12;
        btn.style.color = Color.white;
        SetBtnColor(btn, ColorDefault);
        btn.style.borderTopLeftRadius     = 6;
        btn.style.borderTopRightRadius    = 6;
        btn.style.borderBottomLeftRadius  = 6;
        btn.style.borderBottomRightRadius = 6;
        btn.style.borderTopWidth    = 0;
        btn.style.borderBottomWidth = 0;
        btn.style.borderLeftWidth   = 0;
        btn.style.borderRightWidth  = 0;
        return btn;
    }

    void StyleAsSecondary(Button btn)
    {
        btn.style.backgroundColor = new Color(0.35f, 0.12f, 0.12f);
        btn.style.color = new Color(1f, 0.6f, 0.6f);
    }

    void SetBtnColor(Button btn, Color color)
    {
        btn.style.backgroundColor = color;
    }
}
