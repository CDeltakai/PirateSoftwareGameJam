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
    public DamagePayload damagePayload;
    public ControlType controlType;

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

    public void AddUniqueParticle(GameObject particleEffect)
    {
        GameObject uniqueEffect = Instantiate(particleEffect, uniqueEffectTransform);
        uniqueEffect.transform.localPosition = Vector3.zero;
    }

}
