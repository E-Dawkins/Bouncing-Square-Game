using UnityEngine;

public class ContactShrink : IModifier
{
    public float rate = 0.9f;

    public ContactShrink(BouncingSquare owner) : base(owner)
    {
        displayName = "Contact Shrink";
        hint = "Shrink in size when squares collide.";
    }

    public override void HandleCollision(CollisionData data)
    {
        owningSquare.transform.localScale *= rate;
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddFloat("Rate", rate, (f) => { rate = f; }, new Vector2(0.1f, 0.9f));
    }
}
