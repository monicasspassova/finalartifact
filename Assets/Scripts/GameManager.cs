using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Goose goose;
    public FPSController player;
    public UIDocument winUI;
    public UIDocument loseUI;
    public UIDocument aggroTimerUI;
    public List<PuzzleZoneController> puzzles = new List<PuzzleZoneController>();

    private int puzzlesCompleted = 0;
    private const int totalPuzzles = 3;
    private HashSet<int> completedPuzzles = new HashSet<int>();

    private float aggroTotal;
    private bool aggroActive;
    private VisualElement aggroFill;
    private Label aggroLabel;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Reset();

        winUI.rootVisualElement.style.display = DisplayStyle.None;
        loseUI.rootVisualElement.style.display = DisplayStyle.None;

        Button loseCloseBtn = loseUI.rootVisualElement.Q<Button>();
        loseCloseBtn.clicked += () => CloseUI(loseUI);

        Button winCloseBtn = winUI.rootVisualElement.Q<Button>();
        winCloseBtn.clicked += () => CloseUI(winUI);

        BuildAggroBar();
    }

    void BuildAggroBar()
    {
        UnityEngine.Debug.Log("[GameManager] BuildAggroBar called. aggroTimerUI=" + aggroTimerUI);
        if (aggroTimerUI == null) return;
        var root = aggroTimerUI.rootVisualElement;
        root.Clear();
        root.style.display = DisplayStyle.None;

        var container = new VisualElement();
        container.style.alignItems = Align.Center;
        container.style.width = 420;
        root.Add(container);

        aggroLabel = new Label("GOOSE INCOMING!");
        aggroLabel.style.fontSize = 13;
        aggroLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        aggroLabel.style.color = new Color(1f, 0.35f, 0.35f);
        aggroLabel.style.marginBottom = 3;
        container.Add(aggroLabel);

        var track = new VisualElement();
        track.style.width = 420;
        track.style.height = 14;
        track.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.7f);
        track.style.borderTopLeftRadius = 7;
        track.style.borderTopRightRadius = 7;
        track.style.borderBottomLeftRadius = 7;
        track.style.borderBottomRightRadius = 7;
        track.style.paddingTop = 2;
        track.style.paddingBottom = 2;
        track.style.paddingLeft = 2;
        track.style.paddingRight = 2;
        container.Add(track);

        aggroFill = new VisualElement();
        aggroFill.style.height = Length.Percent(100);
        aggroFill.style.width = Length.Percent(100);
        aggroFill.style.backgroundColor = new Color(0.85f, 0.2f, 0.2f);
        aggroFill.style.borderTopLeftRadius = 5;
        aggroFill.style.borderTopRightRadius = 5;
        aggroFill.style.borderBottomLeftRadius = 5;
        aggroFill.style.borderBottomRightRadius = 5;
        track.Add(aggroFill);
    }

    void Reset()
    {
        winUI.rootVisualElement.style.display = DisplayStyle.None;
        loseUI.rootVisualElement.style.display = DisplayStyle.None;

        aggroActive = false;
        if (aggroTimerUI != null)
            aggroTimerUI.rootVisualElement.style.display = DisplayStyle.None;

        puzzlesCompleted = 0;
        completedPuzzles.Clear();

        goose.Reset();
        player.Reset();

        foreach (PuzzleZoneController puzzle in puzzles)
        {
            if (puzzle != null)
            {
                puzzle.Reset();
            }
        }

    }

    void Update()
    {
        if (goose.playerDead)
        {
            LoseGame();
        }

        if (aggroActive)
        {
            // Read timer directly from goose - single source of truth
            bool gooseAggro = goose != null && goose.isAggro;
            float remaining = goose != null ? goose.AggroTimeRemaining : 0f;
            float pct = aggroTotal > 0 ? Mathf.Clamp01(remaining / aggroTotal) * 100f : 0f;
            if (aggroFill != null) aggroFill.style.width = Length.Percent(pct);
            if (aggroLabel != null) aggroLabel.text = $"GOOSE INCOMING!  {Mathf.CeilToInt(remaining)}s";
            if (!gooseAggro)
            {
                aggroActive = false;
                if (aggroTimerUI != null)
                    aggroTimerUI.rootVisualElement.style.display = DisplayStyle.None;
            }
        }
    }

    public void PuzzleCompleted(int puzzleID)
    {
        if (completedPuzzles.Contains(puzzleID)) return;
        completedPuzzles.Add(puzzleID);

        puzzlesCompleted++;

        // if (puzzlesCompleted == totalPuzzles)
        // {
        //     WinGame();
        // }
    }

    public void TriggerGooseAggro(float duration = 30f)
    {
        UnityEngine.Debug.Log("[GameManager] TriggerGooseAggro called. goose=" + goose + " aggroTimerUI=" + aggroTimerUI);
        goose.AggroForDuration(duration);
        aggroTotal = duration;
        aggroActive = true;
        if (aggroTimerUI != null)
        {
            aggroTimerUI.rootVisualElement.style.display = DisplayStyle.Flex;
            if (aggroFill != null) aggroFill.style.width = Length.Percent(100);
        }
        else
        {
            UnityEngine.Debug.LogWarning("[GameManager] aggroTimerUI is NOT assigned in the Inspector!");
        }
    }

    public void WinGame()
    {
        winUI.rootVisualElement.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void LoseGame()
    {
        loseUI.rootVisualElement.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

    }

    public void CloseUI(UIDocument ui)
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;

        Reset();


    }

    public bool AllPuzzlesComplete()
    {
        return puzzlesCompleted >= totalPuzzles;
    }

    public int PuzzlesCompleted()
    {
        return puzzlesCompleted;
    }
}

