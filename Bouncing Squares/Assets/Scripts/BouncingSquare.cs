using UnityEngine;
using System.Collections.Generic;
using System;

public class BouncingSquare : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sprite;
    [SerializeField] private Health healthComp;
    [SerializeField] private Outline outlineComp;

    // Square essentials
    public List<IModifier> modifiers { get; private set; } = new List<IModifier>();
    private Vector2 startingPosition = Vector2.zero;
    private Vector2 lastVelocity = Vector2.zero;
    private int? blockingDirection = null; // -1, 0, 1, 2, 3 => all, L, R, T, B

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();

        // default modifiers
        modifiers.Add(new ContactDamage(this));
        modifiers.Add(new AddVelocity(this));
    }

    private void FixedUpdate() => lastVelocity = rb.linearVelocity;

    private void OnMouseEnter() => outlineComp.OnHover();

    private void OnMouseExit() => outlineComp.OnUnHover();

    public void SetSelected(bool selected) => outlineComp.OnSelect(selected);

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

    public void SetCollision(bool state) => col.excludeLayers = (state ? 1 << LayerMask.NameToLayer("Square") : 0);

    public void SetOpacity(float opacity) => sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, opacity);

    public void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddText(name);
        uiBuilder.AddVec2("Position", transform.position, (v) => { transform.position = v; });
        uiBuilder.AddInt("Health", healthComp.GetValue(), (i) => { healthComp.SetMaxValue(i); }, new Vector2(1, float.PositiveInfinity));

        // add existing modifiers to the ui
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
        startingPosition = transform.position;

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
        transform.position = otherSquare.startingPosition;
        startingPosition = otherSquare.startingPosition;

        healthComp.SetMaxValue(otherSquare.healthComp.GetMaxValue());

        modifiers = otherSquare.modifiers;
        foreach (var modifier in modifiers)
        {
            modifier.owningSquare = this;
        }
    }

    public void Damage(int amount, Vector2 originPosition)
    {
        // block all directions or blocking specific direction
        if (blockingDirection == -1 ||
            blockingDirection == GetDirectionFromVector((originPosition - (Vector2)transform.position).normalized))
        {
            blockingDirection = null;
            return;
        }

        healthComp.SetValue(healthComp.GetValue() - amount);
    }

    public void Heal(int amount) => healthComp.SetValue(healthComp.GetValue() + amount);

    public bool IsAlive() => healthComp.GetValue() > 0;

    public bool IsBlocking() => blockingDirection != null;

    public void BlockNextDamage(int direction = -1) => blockingDirection = direction;

    private int GetDirectionFromVector(Vector2 direction) // left, right, top, bottom => 0, 1, 2, 3
    {
        Vector2[] directions = new Vector2[4] { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

        float highestDot = float.MinValue;
        int lowestDirection = -1;


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
