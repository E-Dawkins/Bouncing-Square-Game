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

    public int health = 5;
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
        uiBuilder.AddInt("Health", health, (i) => 
        { 
            health = i;
            healthSlider.maxValue = health;
        });

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
    }
}
