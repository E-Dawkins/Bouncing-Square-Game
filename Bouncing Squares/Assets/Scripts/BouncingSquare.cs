using UnityEngine;
using System.Collections.Generic;
using System;

public class BouncingSquare : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer outlineRenderer;
    private Rigidbody2D rb;
    private Vector2 lastVelocity = Vector2.zero;

    public List<IModifier> modifiers { get; private set; } = new List<IModifier>();
    public Color hoveredColor = Color.yellow;
    public Color selectedColor = Color.green;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        outlineRenderer = transform.Find("Outline")?.gameObject.GetComponent<SpriteRenderer>();
        outlineRenderer.enabled = false;

        // default modifiers
        modifiers.Add(new Health());
        modifiers.Add(new ContactDamage());
        modifiers.Add(new AddVelocity());
    }

    private void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Square"))
        {
            // resolve any unwanted movement
            // i.e. after a bounce if one square would be frozen, apply negative previous velocity
            if (rb.linearVelocity.x == 0) rb.linearVelocity = new Vector2(-lastVelocity.x, rb.linearVelocity.y);
            if (rb.linearVelocity.y == 0) rb.linearVelocity = new Vector2(rb.linearVelocity.x, -lastVelocity.y);

            CollisionData collisionData = new CollisionData();
            collisionData.otherSquare = collision.gameObject?.GetComponent<BouncingSquare>();
            collisionData.directionToOtherSquare = (collision.transform.position - transform.position).normalized;

            // handle collision in modifiers
            foreach (IModifier modifier in modifiers)
            {
                modifier.HandleCollision(collisionData);
            }
        }
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
