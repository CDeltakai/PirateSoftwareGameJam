using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ElementEffectModifier : MonoBehaviour
{
    public SpellElementSO element;
    public abstract void ModifyEffect(SpellEffect effect);

}
