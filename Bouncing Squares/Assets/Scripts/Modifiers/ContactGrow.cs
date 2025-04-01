using UnityEngine;

public class ContactGrow : IModifier
{
    float rate = 1.1f;

    public ContactGrow(BouncingSquare owner) : base(owner)
    {
        displayName = "Contact Growth";
        hint = "Grow in size when squares collide.";
    }

    public override void HandleCollision(CollisionData data) 
    {
        owningSquare.transform.localScale *= rate;
    }

    public override void CreateUI(SquareUIBuilder uiBuilder) 
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddFloat("Rate", rate, (f) => { rate = f; }, new Vector2(1.1f, float.PositiveInfinity));
    }
}
