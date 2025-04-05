using UnityEngine;

public class Lasers : IModifier
{
    public float interval = 5;
    public float duration = 3;
    public int damagePerTick = 1;
    public int ticksPerSecond = 2;

    private bool lasersActive = false;
    private float timer = 0;
    private float tickTimer = 0;

    public Lasers(BouncingSquare owner) : base(owner)
    {
        displayName = "Lasers";
        hint = "Periodically spawn damaging lasers in each cardinal direction.";
    }

    public override void CustomStart()
    {
        // init this here so the very first tick damages other squares
        tickTimer = (1 / (float)ticksPerSecond);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= (lasersActive ? duration : interval))
        {
            timer = 0;
            lasersActive = !lasersActive;
            owningSquare.SetLasersActive(lasersActive);
        }

        if (lasersActive)
        {
            tickTimer += Time.deltaTime;

            if (tickTimer >= (1 / (float)ticksPerSecond))
            {
                CheckForLaserHits();
                tickTimer = 0;

            }
        }
    }

    private void CheckForLaserHits()
    {
        int squareLayer = LayerMask.NameToLayer("Square");

        void handleSquareHits(Vector2 direction)
        {
            Vector2 center = owningSquare.position2d + direction * 10;
            Vector2 extent = new Vector2(0.5f, 0.5f) +
                new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y)) * 19.5f; // (1, 0) => (19.5, 0) + (0.5, 0.5) => (20, 0)
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, extent, 0);

            foreach (Collider2D c in colliders)
            {
                if (c.gameObject == gameObject)
                    continue;

                if (c.gameObject.layer == squareLayer)
                {
                    c.gameObject.GetComponent<BouncingSquare>()?.Damage(damagePerTick, owningSquare.position2d);
                }
            }
        }

        handleSquareHits(Vector2.up);
        handleSquareHits(Vector2.down);
        handleSquareHits(Vector2.left);
        handleSquareHits(Vector2.right);
    }

    public override void CreateUI(SquareUIBuilder uiBuilder) 
    {
        base.CreateUI(uiBuilder);
        uiBuilder.AddFloat("Interval", interval, (f) => { interval = f; }, new Vector2(0.5f, float.PositiveInfinity));
        uiBuilder.AddFloat("Duration", duration, (f) => { duration = f; }, new Vector2(0.5f, float.PositiveInfinity));
        uiBuilder.AddInt("Damage/Tick", damagePerTick, (i) => { damagePerTick = i; }, new Vector2(1, float.PositiveInfinity));
        uiBuilder.AddInt("Ticks/Second", ticksPerSecond, (i) => { ticksPerSecond = i; }, new Vector2(1, float.PositiveInfinity));
    }
}
