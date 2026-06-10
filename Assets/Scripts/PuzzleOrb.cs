using UnityEngine;

public class PuzzleOrb : MonoBehaviour
{
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;

    private Vector3 startPos;
    private Light orbLight;
    private Renderer orbRenderer;
    private bool completed = false;

    void Start()
    {
        startPos = transform.position;
        orbLight = GetComponentInChildren<Light>();
        orbRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (completed) return;

        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    public void SetCompleted()
    {
        completed = true;

        // Turn off point light
        if (orbLight != null)
            orbLight.enabled = false;

        if (orbRenderer != null)
            orbRenderer.enabled = false;
    }

    public void ResetOrb()
    {
        completed = false;
        if (orbLight != null)
            orbLight.enabled = true;
        if (orbRenderer != null)
            orbRenderer.enabled = true;
    }
}
