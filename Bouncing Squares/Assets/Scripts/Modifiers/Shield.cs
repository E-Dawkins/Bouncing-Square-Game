using UnityEngine;

public class Shield : IModifier
{
    public float cooldown = 3;
    private float timer = 0;

    private GameObject visuals = null;
    private GameObject visualInst = null;

    private void Awake()
    {
        visuals = Resources.Load<GameObject>("ShieldPrefab");
    }

    public Shield(BouncingSquare owner) : base(owner)
    {
        displayName = "Rechargeable Shield";
        hint = "Regenerate a damage-blocking shield every X seconds.";
    }

    private void Update()
    {
        if (owningSquare.isBlocking) // shield is already up, early return
            return;
        else if (visualInst != null) // not blocking, remove visuals
            Destroy(visualInst);

        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            owningSquare.BlockNextDamage();
            visualInst = Instantiate(visuals, owningSquare.transform);
            timer = 0;
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddFloat("Cooldown", cooldown, (f) => { cooldown = f; }, new Vector2(0.1f, float.PositiveInfinity));
    }
}
