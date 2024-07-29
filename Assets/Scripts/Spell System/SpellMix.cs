using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellMix
{
    public int MaxElements = 3;

    [SerializeField] List<SpellElementSO> mix = new(3);
    public IReadOnlyList<SpellElementSO> Mix => mix;

    public void ClearElements()
    {
        mix.Clear();
    }

    public void AddElement(SpellElementSO element)
    {
        if (mix.Count < MaxElements)
        {
            mix.Add(element);
        }else
        {
            //Debug.LogWarning("Cannot add more than 3 elements to the mix");
        }
    }

    public void RemoveRange(int index, int count)
    {
        mix.RemoveRange(index, count);
    }


}
