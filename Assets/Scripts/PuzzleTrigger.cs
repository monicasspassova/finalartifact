using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleTrigger : MonoBehaviour
{
    public UIDocument puzzleUI;
    public int puzzleID;
    public bool completed = false;

    private VisualElement root;

    void Start()
    {
        root = puzzleUI.rootVisualElement;

        //Button closeBtn = root.Q<Button>();
        //closeBtn.clicked += () => CompletePuzzle();

        Reset();

    }

    public void Reset()
    {
        root.style.display = DisplayStyle.None;
        completed = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !completed)
        {
            Button closeBtn = root.Q<Button>();
            closeBtn.clicked -= CompletePuzzle; // clear others
            closeBtn.clicked += CompletePuzzle; // add only this one

            root.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }

    public void CloseUI()
    {
        root.style.display = DisplayStyle.None;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

    }

    public void CompletePuzzle()
    {
        if (completed) return;

        completed = true;

        CloseUI();

        GameManager.Instance.PuzzleCompleted(puzzleID);
    }
}