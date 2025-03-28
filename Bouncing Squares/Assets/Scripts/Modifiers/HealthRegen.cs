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

            owningSquare.health += amount;
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Amount", amount, (i) => { amount = i; });
        uiBuilder.AddFloat("Interval", interval, (f) => { interval = f; });
    }
}
