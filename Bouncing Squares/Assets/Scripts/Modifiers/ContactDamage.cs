using UnityEngine;

public class ContactDamage : IModifier
{
    public int value = 1;

    public ContactDamage()
    {
        displayName = "Contact Damage";
        hint = "Inflicted damage when squares collide.";
    }

    public override void HandleCollision(CollisionData data)
    {
        Health healthComponent = data.otherSquare?.GetComponent<Health>();
        if (healthComponent)
        {
            healthComponent.value -= value;
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Damage", value, (i) => { value = i; });
    }
}
