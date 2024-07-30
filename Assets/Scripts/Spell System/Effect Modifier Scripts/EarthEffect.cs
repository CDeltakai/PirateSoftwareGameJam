using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthEffect : ElementEffectModifier
{
    [SerializeField] StatusEffect statusEffect;

    public override void ModifyEffect(SpellEffect effect)
    {
        effect.damagePayload.statusEffects.Add(statusEffect);
    }
}
