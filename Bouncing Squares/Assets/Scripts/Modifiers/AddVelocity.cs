using UnityEngine;

public class AddVelocity : IModifier
{
    public Vector2 velocity = Vector2.zero;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb?.AddForce(velocity);
    }
}
