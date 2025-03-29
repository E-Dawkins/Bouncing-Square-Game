using UnityEngine;

public class HealthSteal : IModifier
{
    public int value = 1;

    public HealthSteal(BouncingSquare owner) : base(owner)
    {
        displayName = "Vampirism";
        hint = "Steal health when squares collide.";
    }

    public override void HandleCollision(CollisionData data)
    {
        data.otherSquare.Damage(value);
        owningSquare.Heal(value);
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Amount", value, (i) => { value = i; }, new Vector2(1, float.PositiveInfinity));
    }
}
