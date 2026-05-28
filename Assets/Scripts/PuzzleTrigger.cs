using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleTrigger : MonoBehaviour
{
    public UIDocument puzzleUI;
    public GameManager gm;
    public int puzzleID;
    private bool completed = false;
    private VisualElement root;

    void Start()
    {
        root = puzzleUI.rootVisualElement;
        root.style.display = DisplayStyle.None;

        Button closeBtn = root.Q<Button>();
        closeBtn.clicked += CloseUI;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !completed)
        {
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
        completed = true;
        gm.PuzzleCompleted(0);
        CloseUI();
    }
}