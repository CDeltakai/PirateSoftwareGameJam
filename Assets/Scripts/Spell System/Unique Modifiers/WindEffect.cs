using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : ElementEffectModifier
{
    public override void ModifyEffect(SpellEffect effect)
    {
        effect.controlType = SpellEffect.ControlType.FreeAim;
    }
}
