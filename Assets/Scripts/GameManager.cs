using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Goose goose;
    public FPSController player;
    public UIDocument winUI;
    public UIDocument loseUI;
    public List<PuzzleTrigger> puzzles = new List<PuzzleTrigger>();

    private int puzzlesCompleted = 0;
    private const int totalPuzzles = 3;
    private HashSet<int> completedPuzzles = new HashSet<int>();


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        winUI.rootVisualElement.style.display = DisplayStyle.None;
        loseUI.rootVisualElement.style.display = DisplayStyle.None;

        Button loseCloseBtn = loseUI.rootVisualElement.Q<Button>();
        loseCloseBtn.clicked += () => CloseUI(loseUI);

        Button winCloseBtn = winUI.rootVisualElement.Q<Button>();
        winCloseBtn.clicked += () => CloseUI(winUI);

    }

    void Reset()
    {
        winUI.rootVisualElement.style.display = DisplayStyle.None;
        loseUI.rootVisualElement.style.display = DisplayStyle.None;

        puzzlesCompleted = 0;
        completedPuzzles.Clear();

        goose.Reset();
        player.Reset();

        foreach (PuzzleTrigger puzzle in puzzles)
        {
            puzzle.Reset();
        }

    }

    void Update()
    {
        if (goose.playerDead)
        {
            LoseGame();
        }
        

    }

    public void PuzzleCompleted(int puzzleID)
    {

        if (completedPuzzles.Contains(puzzleID)) return;
        completedPuzzles.Add(puzzleID);

        puzzlesCompleted++;
        goose.PuzzleSolved();

        if (puzzlesCompleted == totalPuzzles)
        {
            WinGame();
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
}

