using UnityEngine;

public class ContactDamage : IModifier
{
    public int value = 1;

    public ContactDamage(BouncingSquare owner) : base(owner)
    {
        displayName = "Contact Damage";
        hint = "Inflicted damage when squares collide.";
    }

    public override void HandleCollision(CollisionData data)
    {
        data.otherSquare.Damage(value);
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Damage", value, (i) => { value = i; }, new Vector2(1, float.PositiveInfinity));
    }
}
