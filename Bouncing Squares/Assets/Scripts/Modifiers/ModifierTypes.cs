
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
        typeof(Shield),
        typeof(DirectionalShield)
    };
}
