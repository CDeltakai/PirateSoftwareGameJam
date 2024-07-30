using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpellManager : MonoBehaviour
{
    public event Action OnSpellMixChanged;

    [SerializeField] int maxElements = 3;

    [SerializeField] SpellPreviewController spellPreviewController;
    [SerializeField] VirtualCursorController virtualCursorController;
    [SerializeField] GameObject baseSpellPrefab;

    [SerializeField] SpellEffect currentSpellEffect;
    [SerializeField] SpellMix _currentSpellMix = new();
    public SpellMix CurrentSpellMix => _currentSpellMix;

    [SerializeField] List<SpellElementSO> elementList;

    [SerializeField] bool _castingInProgress = false;
    public bool CastingInProgress => _castingInProgress;

    void Awake()
    {
        _currentSpellMix.MaxElements = maxElements;
    }

    void Start()
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
        //TODO: Implement spell mixing logic

    }

    public bool ConfirmSpellMix()
    {
        if(_currentSpellMix.Mix.Count == 0)
        {
            return false;
        }

        currentSpellEffect = spellPreviewController.SpellEffectInstance;
        virtualCursorController.SetTargetingType(currentSpellEffect.targetingType);
        return true;
    }

    public bool CastSpell()
    {
        if(!currentSpellEffect)
        {
            Debug.LogWarning("No spell effect set");
            return false;
        }

        object target = virtualCursorController.GetTarget();
        if(target == null)
        {
            Debug.Log("No target set");
            return false;
        }

        //This is a really bad idea but I was out of time so it'll have to do for now
        switch (currentSpellEffect.targetingType)
        {
            case TargetingControlType.Guided:
                currentSpellEffect.DeploySpellGuided((StageEntity)virtualCursorController.GetTarget());
                break;
            case TargetingControlType.FreeAim:
                currentSpellEffect.DeploySpellFreeAim((Vector3)virtualCursorController.GetTarget());
                break;
            case TargetingControlType.Cardinal:
                break;
            default:
                break;
        }

        ClearSpellMix();
        return true;
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
