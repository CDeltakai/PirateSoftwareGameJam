using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPreviewController : MonoBehaviour
{
    [SerializeField] PlayerSpellManager playerSpellManager;
    [SerializeField] GameObject baseSpellPrefab;

    [SerializeField] SpellEffect _spellEffectInstance;
    public SpellEffect SpellEffectInstance => _spellEffectInstance;


    void Start()
    {
        playerSpellManager.OnSpellMixChanged += UpdateSpellPreview;
    }

    void GenerateSpellPreview()
    {
        GameObject spellPreview = Instantiate(baseSpellPrefab, transform);   
        spellPreview.transform.localPosition = Vector3.zero;
        _spellEffectInstance = spellPreview.GetComponent<SpellEffect>();
    }

    void CancelSpellPreview()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateSpellPreview()
    {
        if(playerSpellManager.CurrentSpellMix.Mix.Count == 0)
        {
            CancelSpellPreview();
        }
        else
        if(!_spellEffectInstance && playerSpellManager.CurrentSpellMix.Mix.Count > 0)
        {
            GenerateSpellPreview();
        }

        if(_spellEffectInstance)
        {
            for (int i = 0; i < playerSpellManager.CurrentSpellMix.Mix.Count; i++)
            {
                if(!_spellEffectInstance.primaryElement)
                {
                    _spellEffectInstance.primaryElement = playerSpellManager.CurrentSpellMix.Mix[0];
                    _spellEffectInstance.ApplyPrimaryElement();
                }

                //If the element is not already applied to the spell effect, apply it.
                if(i == 0 && !_spellEffectInstance.primaryElement)
                {
                    _spellEffectInstance.primaryElement = playerSpellManager.CurrentSpellMix.Mix[i];
                    _spellEffectInstance.ApplyPrimaryElement();
                }
                else if(i == 1 && !_spellEffectInstance.secondaryElement)
                {
                    _spellEffectInstance.secondaryElement = playerSpellManager.CurrentSpellMix.Mix[i];
                    _spellEffectInstance.ApplySecondaryElement();
                }
                else if(i == 2 && !_spellEffectInstance.tertiaryElement)
                {
                    _spellEffectInstance.tertiaryElement = playerSpellManager.CurrentSpellMix.Mix[i];
                    _spellEffectInstance.ApplyTertiaryElement();
                }
            }
        }

    }


}
