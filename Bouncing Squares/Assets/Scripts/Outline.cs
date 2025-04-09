using UnityEngine;

public class Outline : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outlineRenderer;
    [SerializeField] private Color hoveredColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.green;

    private bool isSelected = false;

    public void OnHover()
    {
        if (isSelected)
            return;

        outlineRenderer.color = hoveredColor;
    }

    public void OnUnHover()
    {
        if (isSelected) 
            return;

        outlineRenderer.color = Color.clear;
    }

    public void OnSelect(bool state)
    {
        isSelected = state;
        outlineRenderer.color = (state ? selectedColor : Color.clear);
    }
}
