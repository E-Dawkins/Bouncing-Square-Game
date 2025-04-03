using UnityEngine;

public class ContactShrink : IModifier
{
    public float amount = 0.1f;
    public Vector2 minSize = new Vector2(0.2f, 0.2f);

    public ContactShrink(BouncingSquare owner) : base(owner)
    {
        displayName = "Contact Shrink";
        hint = "Shrink in size when squares collide.";
    }

    public override void HandleCollision(CollisionData data)
    {
        Vector3 calculatedScale = owningSquare.transform.localScale - new Vector3(amount, amount, 1);
        owningSquare.transform.localScale = Vector3.Max(calculatedScale, minSize);
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddFloat("Amount", amount, (f) => { amount = f; }, new Vector2(0.1f, 1));
        uiBuilder.AddVec2("Min Size", minSize, (v) => { minSize = v; }, new Vector2(0.2f, 0.2f), new Vector2(10, 10));
    }
}
