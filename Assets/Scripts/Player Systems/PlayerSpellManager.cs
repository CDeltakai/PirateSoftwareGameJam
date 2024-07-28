using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellManager : MonoBehaviour
{
    [SerializeField] int maxElements = 3;


    [SerializeField] SpellEffect currentSpellEffect;
    [SerializeField] SpellMix currentSpellMix = new();

    [Serializable]
    public class ElementIndex
    {
        public int index;
        public SpellElementSO element;
    }

    [SerializeField] List<ElementIndex> elementIndexList;

    void Awake()
    {
        currentSpellMix.MaxElements = maxElements;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // This method will be called by Unity when the script is loaded or a value is changed in the inspector
    private void OnValidate()
    {
        if (currentSpellMix.Mix.Count > maxElements)
        {
            currentSpellMix.RemoveRange(currentSpellMix.MaxElements, currentSpellMix.Mix.Count - maxElements);
            Debug.LogWarning("Exceeded max number of elements. Removing extra elements.");
        }
    }

    public void MixSpell(SpellMix spellMix)
    {
        SpellEffect spellEffect = new SpellEffect();
        //TODO: Implement spell mixing logic

    }

}
