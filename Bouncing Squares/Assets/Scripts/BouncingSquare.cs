using UnityEngine;

public class BouncingSquare : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer outlineRenderer;

    public Color hoveredColor = Color.yellow;
    public Color selectedColor = Color.green;

    private void Awake()
    {
        outlineRenderer = transform.Find("Outline")?.gameObject.GetComponent<SpriteRenderer>();
        outlineRenderer.enabled = false;
    }

    private void OnMouseEnter()
    {
        outlineRenderer.color = hoveredColor;
        outlineRenderer.enabled = true;
    }

    private void OnMouseExit()
    {
        outlineRenderer.enabled = isSelected;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (isSelected) outlineRenderer.color = selectedColor;
        else outlineRenderer.enabled = false;
    }
}
