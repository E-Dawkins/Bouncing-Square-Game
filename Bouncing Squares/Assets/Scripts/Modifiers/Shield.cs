using UnityEngine;

public class Shield : IModifier
{
    public float cooldown = 3;
    private float timer = 0;

    public Shield(BouncingSquare owner) : base(owner)
    {
        displayName = "Rechargeable Shield";
        hint = "Regenerate a damage-blocking shield every X seconds.";
    }

    private void Update()
    {
        if (owningSquare.isBlocking) // shield is already up, early return
            return;

        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            Debug.Log("Shield UP");
            owningSquare.BlockNextDamage();
            timer = 0;
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddFloat("Cooldown", cooldown, (f) => { cooldown = f; }, new Vector2(0.1f, float.PositiveInfinity));
    }
}
