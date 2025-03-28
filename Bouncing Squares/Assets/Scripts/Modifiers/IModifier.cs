using UnityEngine;

public struct CollisionData
{
    public BouncingSquare otherSquare;
    public Vector2 directionToOtherSquare;
}

public class IModifier : MonoBehaviour
{
    public string displayName = "IModifier";
    public string hint = "This modifier does nothing.";

    [HideInInspector]
    public BouncingSquare owningSquare;

    public IModifier(BouncingSquare owner)
    {
        owningSquare = owner;
    }

    public virtual void CustomStart() { }

    public virtual void HandleCollision(CollisionData data) { }

    public virtual void CreateUI(SquareUIBuilder uiBuilder) { }
}
