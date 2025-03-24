using UnityEngine;

public class Health : IModifier
{
    public int value = 5;

    public Health()
    {
        displayName = "Initial Health";
        hint = "The starting health of the square.";
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Health", value, (i) => { value = i; });
    }
}
