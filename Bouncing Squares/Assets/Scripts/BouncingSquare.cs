using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class BouncingSquare : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer outlineRenderer;
    private Rigidbody2D rb;
    private Vector2 lastVelocity = Vector2.zero;
    private Slider healthSlider;
    private int blockingDirection = -1;

    public bool isBlocking { get; private set; } = false;
    public int health { get; private set; } = 5;
    public Vector2 position2d { get; private set; } = Vector2.zero;
    public List<IModifier> modifiers { get; private set; } = new List<IModifier>();
    public Color hoveredColor = Color.yellow;
    public Color selectedColor = Color.green;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        healthSlider = GetComponentInChildren<Slider>();

        if (healthSlider)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }

        outlineRenderer = transform.Find("Outline")?.gameObject.GetComponent<SpriteRenderer>();
        outlineRenderer.enabled = false;

        position2d = transform.position;

        // default modifiers
        modifiers.Add(new ContactDamage(this));
        modifiers.Add(new AddVelocity(this));
    }

    private void Update()
    {
        if (healthSlider)
        {
            if (health > healthSlider.maxValue)
                health = (int)healthSlider.maxValue;

            healthSlider.value = health;
        }
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

            CollisionData data = new CollisionData();
            data.otherSquare = collision.gameObject?.GetComponent<BouncingSquare>();
            data.directionToOtherSquare = (collision.transform.position - transform.position).normalized;

            // handle collision in modifiers
            foreach (IModifier modifier in modifiers)
            {
                modifier.HandleCollision(data);
            }
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (isSelected) outlineRenderer.color = selectedColor;
        else outlineRenderer.enabled = false;
    }

    public void SetGhost(bool isGhost)
    {
        // Switch collision state
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            collider.excludeLayers = isGhost
                ? 1 << LayerMask.NameToLayer("Square")
                : 0;
        }

        // Some simple visual feedback, made the sprite renderer more 'ghost' like
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer)
        {
            renderer.color *= (isGhost ? 0.5f : 2);
        }
    }

    public void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddText(name);
        uiBuilder.AddVec2("Position", position2d, (v) => 
        {
            transform.position = v;
            position2d = v;
        });
        uiBuilder.AddInt("Health", health, (i) => 
        { 
            health = i;
            healthSlider.maxValue = health;
        }, new Vector2(1, float.PositiveInfinity));

        foreach (IModifier modifier in modifiers)
        {
            uiBuilder.AddModifierLabel(modifier, this);
            modifier.CreateUI(uiBuilder);
        }

        uiBuilder.AddModifierButton("Add Modifier", this, (s) => 
        {
            // add the corresponding modifier to our modifier list
            foreach (Type type in ModifierAttribs.Modifiers)
            {
                IModifier modifierInst = (Activator.CreateInstance(type, new object[] { this }) as IModifier);

                if (modifierInst.displayName == s)
                {
                    modifiers.Add(modifierInst);

                    uiBuilder.AddModifierLabel(modifierInst, this);
                    modifierInst.CreateUI(uiBuilder);
                }
            }
        });
    }

    public void ApplyModifiers()
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            IModifier modifier = modifiers[i];

            Type type = modifier.GetType();
            IModifier copy = gameObject.AddComponent(type) as IModifier;
            foreach (var field in type.GetFields())
            {
                field.SetValue(copy, field.GetValue(modifier)); // Copy each field's value
            }

            copy.CustomStart();

            modifiers[i] = copy; // replace old reference with new one
        }
    }

    public void CopyPropsFrom(BouncingSquare otherSquare)
    {
        transform.position = otherSquare.position2d;
        position2d = otherSquare.position2d;

        health = (int)otherSquare.healthSlider.maxValue;
        healthSlider.maxValue = otherSquare.healthSlider.maxValue;

        modifiers = otherSquare.modifiers;
        foreach (var modifier in modifiers)
        {
            modifier.owningSquare = this;
        }
    }

    public void Damage(int amount, Vector2 originPosition)
    {
        int directionToOtherSquare = GetDirectionFromVector((originPosition - position2d).normalized);

        // we should block this damage, and either full shield is up or we are blocking in the damage direction
        if (isBlocking && (blockingDirection == -1 || directionToOtherSquare == blockingDirection))
        {
            isBlocking = false;
            return;
        }

        health -= amount;
    }

    public void Heal(int amount)
    {
        health += amount;
    }

    public void BlockNextDamage(int direction = -1)
    {
        isBlocking = true;
        blockingDirection = direction;
    }

    private int GetDirectionFromVector(Vector2 direction) // left, right, top, bottom => 0, 1, 2, 3
    {
        float highestDot = float.MinValue;
        int lowestDirection = -1;

        Vector2[] directions = new Vector2[4] { Vector2.left, Vector2.right, Vector2.up, Vector2.down};

        for (int i = 0; i < 4; i++)
        {
            float currentDot = Vector2.Dot(direction, directions[i]);
            if (currentDot > highestDot)
            {
                highestDot = currentDot;
                lowestDirection = i;
            }
        }

        return lowestDirection;
    }
}
