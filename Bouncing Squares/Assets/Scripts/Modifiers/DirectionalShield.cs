using UnityEngine;
using System.Collections.Generic;

public class DirectionalShield : IModifier
{
    public float cooldown = 3;
    public int selectedDirection = 0;
    private float timer = 0;

    private GameObject visuals = null;
    private GameObject visualInst = null;

    private void Awake()
    {
        visuals = Resources.Load<GameObject>("DirectionalShieldPrefab");
    }

    public DirectionalShield(BouncingSquare owner) : base(owner)
    {
        displayName = "Directional Shield";
        hint = "Regenerate a directional damage-blocking shield every X seconds.";
    }

    private void Update()
    {
        if (owningSquare.IsBlocking()) // shield is already up, early return
            return;
        else if (visualInst != null) // not blocking, remove visuals
            Destroy(visualInst);

        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            owningSquare.BlockNextDamage(selectedDirection);
            visualInst = Instantiate(visuals, owningSquare.transform);
            visualInst.transform.Rotate(new Vector3(0, 0, GetAngleFromDirection()));
            timer = 0;
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddFloat("Cooldown", cooldown, (f) => { cooldown = f; }, new Vector2(0.1f, float.PositiveInfinity));
        uiBuilder.AddDropdown("Direction", new List<string>(){ "Left", "Right", "Top", "Bottom" }, selectedDirection, (i) => { selectedDirection = i; });
    }

    private int GetAngleFromDirection()
    {
        switch (selectedDirection)
        {
            case 1: return 180;
            case 2: return -90;
            case 3: return 90;
            default: return 0;
        }
    }
}
