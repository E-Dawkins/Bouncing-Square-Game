using UnityEngine;

public class PingPongSize : IModifier
{
    public Vector2 minSize = Vector2.one;
    public Vector2 maxSize = Vector2.one * 2;
    public float minToMaxDuration = 1;
    public float maxToMinDuration = 1;

    private bool isGrowing = true;
    private float timer = 0;

    public PingPongSize(BouncingSquare owner) : base(owner)
    {
        displayName = "Ping-Pong Size";
        hint = "Bounce between min and max size.";
    }

    public override void CustomStart() 
    {
        // for some reason isGrowing gets reset to false before the first Update tick
        isGrowing = true;
    }

    private void Update()
    {
        float t = isGrowing 
            ? (timer / minToMaxDuration) 
            : (timer / maxToMinDuration);

        Vector3 calculatedScale = isGrowing 
            ? Vector2.Lerp(minSize, maxSize, t)
            : Vector2.Lerp(maxSize, minSize, t);

        owningSquare.transform.localScale = calculatedScale;

        if (t >= 1)
        {
            isGrowing = !isGrowing;
            timer = 0;
        }

        timer += Time.deltaTime; // do this at the end, so we always start at min size due to lerp logic
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddVec2("Min Size", minSize, (v) => { minSize = v; }, new Vector2(0.2f, 0.2f), new Vector2(10, 10));
        uiBuilder.AddVec2("Max Size", maxSize, (v) => { maxSize = v; }, new Vector2(1, 1), new Vector2(10, 10));
        uiBuilder.AddFloat("Grow Duration", minToMaxDuration, (f) => { minToMaxDuration = f; }, new Vector2(0.1f, float.PositiveInfinity));
        uiBuilder.AddFloat("Shrink Duration", maxToMinDuration, (f) => { maxToMinDuration = f; }, new Vector2(0.1f, float.PositiveInfinity));
    }
}
