using UnityEngine;
using System.Collections.Generic;
using System;

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

        // default modifiers
        modifiers.Add(new AddVelocity());
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

    public void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddText(name);
        uiBuilder.AddVec2("Position", transform.position, (v) => { transform.position = v; });

        foreach (IModifier modifier in modifiers)
        {
            modifier.CreateUI(uiBuilder);
        }
    }

    public void ApplyModifiers()
    {
        foreach (IModifier modifier in modifiers)
        {
            Type type = modifier.GetType();
            IModifier copy = gameObject.AddComponent(type) as IModifier;
            foreach (var field in type.GetFields())
            {
                field.SetValue(copy, field.GetValue(modifier)); // Copy each field's value
            }

            copy.CustomStart();
        }
    }
}
