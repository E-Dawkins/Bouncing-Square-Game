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
        data.otherSquare.health -= value;
        owningSquare.health += value;
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Amount", value, (i) => { value = i; });
    }
}
