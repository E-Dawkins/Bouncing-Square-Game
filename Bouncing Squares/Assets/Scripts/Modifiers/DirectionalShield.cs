using UnityEngine;
using System.Collections.Generic;

public class DirectionalShield : IModifier
{
    public float cooldown = 3;
    public int selectedDirection = 0;
    private float timer = 0;

    public DirectionalShield(BouncingSquare owner) : base(owner)
    {
        displayName = "Directional Shield";
        hint = "Regenerate a directional damage-blocking shield every X seconds.";
    }

    private void Update()
    {
        if (owningSquare.isBlocking) // shield is already up, early return
            return;

        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            owningSquare.BlockNextDamage(selectedDirection);
            timer = 0;
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddFloat("Cooldown", cooldown, (f) => { cooldown = f; }, new Vector2(0.1f, float.PositiveInfinity));
        uiBuilder.AddDropdown("Direction", new List<string>(){ "Left", "Right", "Top", "Bottom" }, (i) => { selectedDirection = i; });
    }
}
