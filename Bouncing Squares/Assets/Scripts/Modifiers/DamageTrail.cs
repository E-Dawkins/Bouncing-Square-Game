using UnityEngine;
using System.Collections.Generic;

public class DamageTrail : IModifier
{
    public float interval = 0.5f;
    public float duration = 3;
    public int effectDamage = 1;

    private GameObject damagePrefab = null;
    private float timer = 0;

    private HashSet<GameObject> damageInstances = null;

    public DamageTrail(BouncingSquare owner) : base(owner)
    {
        displayName = "Damage Trail";
        hint = "Spawn damaging effect that lasts X seconds.";
    }

    private void Awake()
    {
        damagePrefab = Resources.Load<GameObject>("DamagePrefab");
        damageInstances = new HashSet<GameObject>();
    }

    private void Update()
    {
        // remove any references to destroyed prefab instances
        damageInstances.RemoveWhere((x) => x == null);

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0;

            GameObject damageInst = Instantiate(damagePrefab, owningSquare.transform.position, Quaternion.identity);
            Destroy(damageInst, duration);

            TriggerDamage triggerDamage = damageInst.GetComponent<TriggerDamage>();
            triggerDamage.ignoredSquare = owningSquare;
            triggerDamage.damage = effectDamage;

            damageInstances.Add(damageInst);
        }
    }

    private void OnDestroy()
    {
        // destroy all prefab instances when we stop the sim.
        foreach (GameObject inst in damageInstances)
        {
            Destroy(inst);
        }
    }

    public override void CreateUI(SquareUIBuilder uiBuilder)
    {
        uiBuilder.AddFloat("Interval", interval, (f) => { interval = f; }, new Vector2(0.5f, float.PositiveInfinity));
        uiBuilder.AddFloat("Duration", duration, (f) => { duration = f; }, new Vector2(0.5f, float.PositiveInfinity));
        uiBuilder.AddInt("Effect Damage", effectDamage, (i) => { effectDamage = i; }, new Vector2(1, float.PositiveInfinity));
    }
}
