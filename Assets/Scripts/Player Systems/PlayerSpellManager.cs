using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpellManager : MonoBehaviour
{
    public event Action OnSpellMixChanged;

    [SerializeField] int maxElements = 3;

    [SerializeField] GameObject baseSpellPrefab;

    [SerializeField] SpellEffect currentSpellEffect;
    [SerializeField] SpellMix _currentSpellMix = new();
    public SpellMix CurrentSpellMix => _currentSpellMix;

    [SerializeField] List<SpellElementSO> elementList;

    void Awake()
    {
        _currentSpellMix.MaxElements = maxElements;
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
        if (_currentSpellMix.Mix.Count > maxElements)
        {
            _currentSpellMix.RemoveRange(_currentSpellMix.MaxElements, _currentSpellMix.Mix.Count - maxElements);
            Debug.LogWarning("Exceeded max number of elements. Removing extra elements.");
        }
    }

    public void MixSpell(SpellMix spellMix)
    {
        SpellEffect spellEffect = new SpellEffect();
        //TODO: Implement spell mixing logic

    }

    public void CastSpell()
    {
        if(!currentSpellEffect)
        {
            Debug.LogWarning("No spell effect set");
            return;
        }
    }

    public void AddElementToMix(SpellElementSO element)
    {
        //Check if element is unique and the mix already has a unique element
        if(element.Unique && _currentSpellMix.Mix.Any(e => e.Unique == true))
        {
            //Debug.LogWarning("Element: " + element.ElementName + " is unique and already in mix");
            return;
        }

        _currentSpellMix.AddElement(element);

        OnSpellMixChanged?.Invoke();
    }

    public void ClearSpellMix()
    {
        _currentSpellMix.ClearElements();
        OnSpellMixChanged?.Invoke();
    }

    /// <summary>
    /// Adds an element to the mix based on the index of the element in the elementIndexList
    /// </summary>
    /// <param name="index"></param>
    public void AddElementToMix(int index)
    {
        if(index < 0 || index >= elementList.Count)
        {
            Debug.LogWarning("Index out of range");
            return;
        }

        if(!elementList[index])
        {
            Debug.LogWarning("Element is null");
            return;
        }

        AddElementToMix(elementList[index]);
    }

}
