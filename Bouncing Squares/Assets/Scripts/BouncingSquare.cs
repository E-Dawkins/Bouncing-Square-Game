using UnityEngine;
using System.Collections.Generic;

public class BouncingSquare : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer outlineRenderer;

    public List<IModifier> modifiers { get; private set; } = new List<IModifier>();
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

    public void PositionChangedCallback(bool isX, string data)
    {
        Vector3 newPos = transform.position;

        if (isX)
        {
            newPos.x = float.Parse(data);
        }
        else
        {
            newPos.y = float.Parse(data);
        }

        transform.position = newPos;
    }
}
