using UnityEngine;

public struct CollisionData
{
    BouncingSquare otherSquare;
    Vector2 directionToOtherSquare;
}

public class IModifier : MonoBehaviour
{
    public string displayName = "IModifier";
    public string hint = "This modifier does nothing.";

    public void HandleCollision(CollisionData data) { }
}
