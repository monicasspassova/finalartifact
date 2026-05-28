using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Goose goose;
    public UIDocument winUI;
    public UIDocument loseUI;

    private int puzzlesCompleted = 0;
    private const int totalPuzzles = 3;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        winUI.rootVisualElement.style.display = DisplayStyle.None;
        loseUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    void Update()
    {

    }

    public void PuzzleCompleted(int puzzleID)
    {
        puzzlesCompleted++;
        goose.PuzzleSolved();

        if (puzzlesCompleted >= totalPuzzles)
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
}

