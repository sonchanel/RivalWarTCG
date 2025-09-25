using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GridHighlighter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color highlightColor = Color.yellow;
    private Color originalColor;
    private GridCell gridCell;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        gridCell = GetComponent<GridCell>();
    }
    private void OnMouseEnter() {
        if (gridCell == null || !gridCell.cellFull)
            spriteRenderer.color = highlightColor;
    }
    private void OnMouseExit() {
        spriteRenderer.color = originalColor;
    }

    // --- NEW: allow external control
    public void SetHighlight(bool active)
    {
        spriteRenderer.color = active ? highlightColor : originalColor;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
