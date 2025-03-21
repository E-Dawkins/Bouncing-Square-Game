
using System;
using System.Collections.Generic;
using UnityEngine;

public class ModifierAttribs
{
    public enum ModifierTypes
    {
        NONE = 0,
        ADD_VELOCITY
    }

    public Dictionary<ModifierTypes, Type> Modifiers;

    public ModifierAttribs()
    {
        Modifiers = new Dictionary<ModifierTypes, Type>
        {
            { ModifierTypes.NONE, typeof(MonoBehaviour) },
            { ModifierTypes.ADD_VELOCITY, typeof(AddVelocity) },
        };
    }
}
