using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GooseController goose;
    public UIDocument winUI;

    private int puzzlesCompleted = 0;
    private const int totalPuzzles = 3;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        winUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void PuzzleCompleted(int puzzleID)
    {
        puzzlesCompleted++;
        goose.StartChasing();

        if (puzzlesCompleted >= totalPuzzles)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        winUI.rootVisualElement.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }
}
