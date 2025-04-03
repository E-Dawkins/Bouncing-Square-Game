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
    private SpriteRenderer[] shieldRenderers = new SpriteRenderer[4];
    private CollisionData currentCollisionData = new CollisionData();
    private int blockingDirection = -1;

    public bool isBlocking { get; private set; } = false;
    public int health { get; private set; } = 5;
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

        Transform shieldParent = transform.Find("Shields");
        shieldRenderers[0] = shieldParent?.GetChild(0)?.GetComponent<SpriteRenderer>();
        shieldRenderers[1] = shieldParent?.GetChild(1)?.GetComponent<SpriteRenderer>();
        shieldRenderers[2] = shieldParent?.GetChild(2)?.GetComponent<SpriteRenderer>();
        shieldRenderers[3] = shieldParent?.GetChild(3)?.GetComponent<SpriteRenderer>();

        SetShieldRenderers(false);

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

            currentCollisionData.otherSquare = collision.gameObject?.GetComponent<BouncingSquare>();
            currentCollisionData.directionToOtherSquare = (collision.transform.position - transform.position).normalized;

            // handle collision in modifiers
            foreach (IModifier modifier in modifiers)
            {
                modifier.HandleCollision(currentCollisionData);
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
        uiBuilder.AddVec2("Position", transform.position, (v) => { transform.position = v; });
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

    public void Reset()
    {
        // loop over all child components, if it is an IModifier, remove it :)
        foreach (IModifier comp in GetComponents<IModifier>())
        {
            Destroy(comp);
        }

        // stop rigidbody movement
        rb.angularVelocity = 0;
        rb.linearVelocity = Vector2.zero;

        // reset health slider
        healthSlider.value = healthSlider.maxValue;
        health = (int)healthSlider.value;

        // reset shield vars
        SetShieldRenderers(false);
        isBlocking = false;
        blockingDirection = -1;

        // reset size
        transform.localScale = Vector3.one;
    }

    public void Damage(int amount)
    {
        int directionToOtherSquare = GetDirectionFromVector(currentCollisionData.directionToOtherSquare);

        if (isBlocking && directionToOtherSquare == blockingDirection)
        {
            isBlocking = false;
            SetShieldRenderers(false);
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
        SetShieldRenderers(true, direction);
        blockingDirection = direction;
    }

    private void SetShieldRenderers(bool isActive, int direction = -1) 
    {
        // a set direction should be modified, not all directions
        if (direction != -1)
        {
            shieldRenderers[direction].enabled = isActive;
            return;
        }

        // this modifies all directions
        shieldRenderers[0].enabled = isActive;
        shieldRenderers[1].enabled = isActive;
        shieldRenderers[2].enabled = isActive;
        shieldRenderers[3].enabled = isActive;
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
