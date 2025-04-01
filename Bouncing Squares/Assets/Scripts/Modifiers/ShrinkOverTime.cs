using UnityEngine;

public class ShrinkOverTime : IModifier
{
    public Vector2 ratePerAxis = new Vector2(0.1f, 0.1f);
    public Vector2 minSize = new Vector2(0.2f, 0.2f);

    public ShrinkOverTime(BouncingSquare owner) : base(owner)
    {
        displayName = "Shrink Over Time";
        hint = "Shrink in size over time.";
    }

    private void Update()
    {
        Vector3 calculatedScale = owningSquare.transform.localScale - (new Vector3(ratePerAxis.x, ratePerAxis.y, 1) * Time.deltaTime);
        owningSquare.transform.localScale = Vector3.Max(calculatedScale, minSize);
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddVec2("Rate Per Axis", ratePerAxis, (v) => { ratePerAxis = v; }, new Vector2(0.1f, 0.1f));
        uiBuilder.AddVec2("Min Size", minSize, (v) => { minSize = v; }, new Vector2(0.2f, 0.2f), new Vector2(10, 10));
    }
}
