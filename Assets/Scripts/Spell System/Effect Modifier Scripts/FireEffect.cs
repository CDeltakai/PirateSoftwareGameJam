using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffect : ElementEffectModifier
{
    [SerializeField] int damageMod = 1;

    public override void ModifyEffect(SpellEffect effect)
    {
        effect.damagePayload.damage += damageMod;
    }
}
