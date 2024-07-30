using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DamagePayload
{
    public int damage;
    public List<StatusEffect> statusEffects;
    public GameObject attacker;
}
