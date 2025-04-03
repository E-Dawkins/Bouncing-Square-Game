using UnityEngine;

public class ContactGrow : IModifier
{
    public float amount = 0.5f;
    public Vector2 maxSize = new Vector2(3, 3);

    public ContactGrow(BouncingSquare owner) : base(owner)
    {
        displayName = "Contact Growth";
        hint = "Grow in size when squares collide.";
    }

    public override void HandleCollision(CollisionData data) 
    {
        Vector3 calculatedScale = owningSquare.transform.localScale + new Vector3(amount, amount, 1);
        owningSquare.transform.localScale = Vector3.Min(calculatedScale, maxSize);
    }

    public override void CreateUI(SquareUIBuilder uiBuilder) 
    {
        uiBuilder.AddFloat("Amount", amount, (f) => { amount = f; }, new Vector2(0.1f, 1));
        uiBuilder.AddVec2("Max Size", maxSize, (v) => { maxSize = v; }, new Vector2(1.5f, 1.5f), new Vector2(10, 10));
    }
}
