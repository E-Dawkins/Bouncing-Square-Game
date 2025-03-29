using UnityEngine;

public class HealthRegen : IModifier
{
    public int amount = 1;
    public float interval = 1;

    private float timer = 0;

    public HealthRegen(BouncingSquare owner) : base(owner)
    {
        displayName = "Health Regen.";
        hint = "Regenerate health every X seconds.";
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0;

            owningSquare.Heal(amount);
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Amount", amount, (i) => { amount = i; }, new Vector2(1, float.PositiveInfinity));
        uiBuilder.AddFloat("Interval", interval, (f) => { interval = f; }, new Vector2(0.1f, float.PositiveInfinity));
    }
}
