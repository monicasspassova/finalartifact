using System;
using System.ComponentModel;
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
        player = GetComponent<FPSController>();

        Button loseCloseBtn = loseUI.rootVisualElement.Q<Button>();
        loseCloseBtn.clicked += () => CloseUI(loseUI);

        Button winCloseBtn = winUI.rootVisualElement.Q<Button>();
        winCloseBtn.clicked += () => CloseUI(winUI);

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

    public void CloseUI(UIDocument ui)
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;

        goose.Reset();

        player.Reset();

    }
}

