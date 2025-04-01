using UnityEngine;

public class GrowOverTime : IModifier
{
    public Vector2 ratePerAxis = new Vector2(0.1f, 0.1f);
    public Vector2 maxSize = new Vector2(3, 3);

    public GrowOverTime(BouncingSquare owner) : base(owner)
    {
        displayName = "Grow Over Time";
        hint = "Grow in size over time.";
    }

    private void Update()
    {
        Vector3 calculatedScale = owningSquare.transform.localScale + (new Vector3(ratePerAxis.x, ratePerAxis.y, 1) * Time.deltaTime);
        owningSquare.transform.localScale = Vector3.Min(calculatedScale, maxSize);
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddVec2("Rate Per Axis", ratePerAxis, (v) => { ratePerAxis = v; }, new Vector2(0.1f, 0.1f));
        uiBuilder.AddVec2("Max Size", maxSize, (v) => { maxSize = v; }, new Vector2(1.5f, 1.5f), new Vector2(10, 10));
    }
}
