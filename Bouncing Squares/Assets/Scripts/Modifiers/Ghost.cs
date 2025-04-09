using UnityEngine;

public class Ghost : IModifier
{
    public float interval = 5;
    public float duration = 3;

    private bool isGhost = false;
    private float timer = 0;

    public Ghost(BouncingSquare owner) : base(owner)
    {
        displayName = "Ghost";
        hint = "Periodically become 'ghostly' for X seconds. While 'ghostly' we don't collide with other squares.";
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= (isGhost ? duration : interval))
        {
            isGhost = !isGhost;
            owningSquare.SetCollision(!isGhost);
            owningSquare.SetOpacity(isGhost ? 0.5f : 1);
            timer = 0;
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddFloat("Interval", interval, (f) => { interval = f; }, new Vector2(0.5f, float.PositiveInfinity));
        uiBuilder.AddFloat("Duration", duration, (f) => { duration = f; }, new Vector2(0.5f, float.PositiveInfinity));
    }
}
