using System.Collections;
using System.Collections.Generic;
using FunkyCode;
using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    public enum ControlType
    {
        Guided, // Should use the TargetClosestToCursor algorithm
        FreeAim, // Should use the FreeAimTargeting algorithm which allows player to place the attack anywhere within light radius
        Cardinal, // Should use the CardinalTargeting algorithm which allows player to place the attack in 4 directions
        Self, // No targeting, spell is cast on the player
    }


[Header("Visual FX")]
    [SerializeField] ParticleSystem primaryParticles;
    [SerializeField] ParticleSystem secondaryParticles;
    [SerializeField] ParticleSystem tertiaryParticles;
    [SerializeField] ParticleSystem impactParticles;

    [SerializeField] Transform uniqueEffectTransform;

    [SerializeField] Light2D pointLight;
    [SerializeField] SpriteRenderer bulletSpriteRenderer;
    [SerializeField] TrailRenderer bulletTrail;

[Header("Visual FX Settings")]
    public Color primaryParticlesColor;
    public Color secondaryParticlesColor;
    public Color tertiaryParticlesColor;
    public Color impactParticlesColor;

    public Color pointLightColor;
    public Color bulletSpriteColor;
    public Color bulletTrailColor;

[Header("Combat Attributes")]
    [SerializeField] GameObject effectModifiersParent;

    public SpellElementSO primaryElement;
    public SpellElementSO secondaryElement;
    public SpellElementSO tertiaryElement;

    public DamagePayload damagePayload;
    public ControlType controlType = ControlType.Guided;

    void Awake()
    {
        ApplySettings();
    }

    void Start()
    {

    }


    public void ApplySettings()
    {
        var primaryParticlesMain = primaryParticles.main;
        primaryParticlesMain.startColor = primaryParticlesColor;

        var secondaryParticlesMain = secondaryParticles.main;
        secondaryParticlesMain.startColor = secondaryParticlesColor;

        var tertiaryParticlesMain = tertiaryParticles.main;
        tertiaryParticlesMain.startColor = tertiaryParticlesColor;

        var impactParticlesMain = impactParticles.main;
        impactParticlesMain.startColor = impactParticlesColor;

        pointLight.color = pointLightColor;
        bulletSpriteRenderer.color = bulletSpriteColor;
        bulletTrail.startColor = bulletTrailColor;
    }


    public void ApplyPrimaryElement()
    {
        ApplyElementEffect(primaryElement);

        if(primaryElement.Unique)
        {
            AddUniqueParticle(primaryElement.ParticleEffect);
        }else
        {
            var primaryParticlesMain = primaryParticles.main;
            primaryParticlesMain.startColor = primaryElement.ElementColor;
            primaryParticles.gameObject.SetActive(true);
        }

    }

    public void ApplySecondaryElement()
    {
        ApplyElementEffect(secondaryElement);

        if(secondaryElement.Unique)
        {
            AddUniqueParticle(secondaryElement.ParticleEffect);
        }else
        {
            var secondaryParticlesMain = secondaryParticles.main;
            secondaryParticlesMain.startColor = secondaryElement.ElementColor;
            secondaryParticles.gameObject.SetActive(true);
        }
    }

    public void ApplyTertiaryElement()
    {
        ApplyElementEffect(tertiaryElement);

        if(tertiaryElement.Unique)
        {
            AddUniqueParticle(tertiaryElement.ParticleEffect);
        }else
        {
            var tertiaryParticlesMain = tertiaryParticles.main;
            tertiaryParticlesMain.startColor = tertiaryElement.ElementColor;
            tertiaryParticles.gameObject.SetActive(true);
        }
    }

    void ApplyElementEffect(SpellElementSO element)
    {
        if (element == null)
        {
            return;
        }

        foreach (GameObject effectModifier in element.ElementEffectModifiers)
        {
            ElementEffectModifier modifier = effectModifier.GetComponent<ElementEffectModifier>();
            modifier.ModifyEffect(this);
        }
    }

    public void AddUniqueParticle(GameObject particleEffect)
    {
        GameObject uniqueEffect = Instantiate(particleEffect, uniqueEffectTransform);
        uniqueEffect.transform.localPosition = Vector3.zero;
    }

}
