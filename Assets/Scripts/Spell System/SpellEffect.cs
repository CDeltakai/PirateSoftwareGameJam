using System.Collections;
using System.Collections.Generic;
using FunkyCode;
using UnityEngine;

public class SpellEffect : MonoBehaviour
{

[Header("References")]
    [SerializeField] ParticleSystem primaryParticles;
    [SerializeField] ParticleSystem secondaryParticles;
    [SerializeField] ParticleSystem tertiaryParticles;
    [SerializeField] ParticleSystem impactParticles;

    [SerializeField] Transform uniqueEffectTransform;

    [SerializeField] Light2D pointLight;
    [SerializeField] SpriteRenderer bulletSpriteRenderer;
    [SerializeField] TrailRenderer bulletTrail;

[Header("Settings")]
    public Color primaryParticlesColor;
    public Color secondaryParticlesColor;
    public Color tertiaryParticlesColor;
    public Color impactParticlesColor;

    public Color pointLightColor;
    public Color bulletSpriteColor;
    public Color bulletTrailColor;


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
