using UnityEngine;

public class HealthRegen : IModifier
{
    public int amount = 1;
    public float interval = 1;

    private Health healthComponent;
    private float timer = 0;

    public HealthRegen()
    {
        displayName = "Health Regen.";
        hint = "Regenerate health every X seconds.";
    }

    private void Awake()
    {
        healthComponent = gameObject.GetComponent<Health>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0;

            if (healthComponent)
            {
                healthComponent.value += amount;
            }
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddInt("Amount", amount, (i) => { amount = i; });
        uiBuilder.AddFloat("Interval", interval, (f) => { interval = f; });
    }
}
