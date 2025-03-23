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

    public virtual void CustomStart() { }

    public virtual void HandleCollision(CollisionData data) { }

    public virtual void CreateUI(SquareUIBuilder uiBuilder) 
    {
        uiBuilder.AddText(displayName);
        uiBuilder.AddText(hint);
    }
}
