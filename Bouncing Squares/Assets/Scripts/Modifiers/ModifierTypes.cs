
using System;
using System.Collections.Generic;

public class ModifierAttribs
{
    public static List<Type> Modifiers = new List<Type>
    {
        typeof(AddVelocity),
        typeof(HealthSteal),
        typeof(HealthRegen),
        typeof(ContactDamage),
        typeof(ContactShrink),
        typeof(ContactGrow),
        typeof(Shield),
        typeof(DirectionalShield),
        typeof(GrowOverTime),
        typeof(ShrinkOverTime),
        typeof(PingPongSize),
        typeof(Ghost)
    };
}
