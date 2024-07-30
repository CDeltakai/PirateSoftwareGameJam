using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPreviewController : MonoBehaviour
{
    [SerializeField] PlayerSpellManager playerSpellManager;
    [SerializeField] GameObject baseSpellPrefab;

    [SerializeField] SpellEffect spellEffectInstance;


    void Start()
    {
        playerSpellManager.OnSpellMixChanged += UpdateSpellPreview;
    }

    void GenerateSpellPreview()
    {
        GameObject spellPreview = Instantiate(baseSpellPrefab, transform);   
        spellPreview.transform.localPosition = Vector3.zero;
        spellEffectInstance = spellPreview.GetComponent<SpellEffect>();
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
        if(!spellEffectInstance && playerSpellManager.CurrentSpellMix.Mix.Count > 0)
        {
            GenerateSpellPreview();
        }

        if(spellEffectInstance)
        {
            for (int i = 0; i < playerSpellManager.CurrentSpellMix.Mix.Count; i++)
            {
                if(!spellEffectInstance.primaryElement)
                {
                    spellEffectInstance.primaryElement = playerSpellManager.CurrentSpellMix.Mix[0];
                    spellEffectInstance.ApplyPrimaryElement();
                }

                //If the element is not already applied to the spell effect, apply it.
                if(i == 0 && !spellEffectInstance.primaryElement)
                {
                    spellEffectInstance.primaryElement = playerSpellManager.CurrentSpellMix.Mix[i];
                    spellEffectInstance.ApplyPrimaryElement();
                }
                else if(i == 1 && !spellEffectInstance.secondaryElement)
                {
                    spellEffectInstance.secondaryElement = playerSpellManager.CurrentSpellMix.Mix[i];
                    spellEffectInstance.ApplySecondaryElement();
                }
                else if(i == 2 && !spellEffectInstance.tertiaryElement)
                {
                    spellEffectInstance.tertiaryElement = playerSpellManager.CurrentSpellMix.Mix[i];
                    spellEffectInstance.ApplyTertiaryElement();
                }
            }
        }

    }


}
