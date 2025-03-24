using UnityEngine;

public class HealthSteal : IModifier
{
    public int value = 1;

    public HealthSteal()
    {
        displayName = "Vampirism";
        hint = "Steal health when squares collide.";
    }

    public override void HandleCollision(CollisionData data)
    {
        Health otherHealthComponent = data.otherSquare?.GetComponent<Health>();
        if (otherHealthComponent) otherHealthComponent.value -= value;

        Health healthComponent = gameObject.GetComponent<Health>();
        if (healthComponent) healthComponent.value += value;
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Amount", value, (i) => { value = i; });
    }
}
