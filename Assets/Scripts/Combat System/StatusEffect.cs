using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StatusEffect 
{
    public StatusType statusType;
    [Range(0, 10)] public float effectStrength;
}
