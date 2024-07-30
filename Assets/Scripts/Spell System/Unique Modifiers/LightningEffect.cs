using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningEffect : ElementEffectModifier
{
    public override void ModifyEffect(SpellEffect effect)
    {
        effect.controlType = SpellEffect.ControlType.Cardinal;        
    }
}
