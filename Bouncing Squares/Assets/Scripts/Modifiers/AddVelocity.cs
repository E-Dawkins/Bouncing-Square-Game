using UnityEngine;

public class AddVelocity : IModifier
{
    public Vector2 velocity = Vector2.zero;

    public AddVelocity(BouncingSquare owner) : base(owner)
    {
        displayName = "Initial Velocity";
        hint = "The starting velocity of the square.";
    }

    public override void CustomStart()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb?.AddForce(velocity);
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddVec2("Velocity", velocity, (v) => { velocity = v; });
    }
}
