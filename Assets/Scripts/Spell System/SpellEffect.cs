using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FunkyCode;
using UnityEngine;
public enum TargetingControlType
{
    Guided, // Should use the TargetClosestToCursor algorithm
    FreeAim, // Should use the FreeAimTargeting algorithm which allows player to place the attack anywhere within light radius
    Cardinal, // Should use the CardinalTargeting algorithm which allows player to place the attack in 4 directions
    Self, // No targeting, spell is cast on the player
}

public class SpellEffect : MonoBehaviour
{


[Header("Visual FX")]
    [SerializeField] GameObject projectileParticlesParent;
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
    public TargetingControlType targetingType = TargetingControlType.Guided;

[Header("Targeting")]
    public StageEntity targetEntity;
    public Vector3 targetPosition;
    [SerializeField] float speed = 5f;
    [SerializeField] bool _reachedDestination = false;
    public bool ReachedDestination => _reachedDestination;

[Header("Debugging")]
    [SerializeField] bool testDeployGuided = false;

    void Awake()
    {
        ApplySettings();
    }

    void Start()
    {

    }

    void Update()
    {
        if(testDeployGuided)
        {
            testDeployGuided = false;
            DeploySpellGuided(targetEntity);
        }
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

    void TriggerImpact()
    {
        projectileParticlesParent.SetActive(false);
        bulletSpriteRenderer.enabled = false;
        impactParticles.gameObject.SetActive(true);
        TweenLightSize(0, 0.5f);
        StartCoroutine(WaitAndDestroy(impactParticles.main.duration));
    }

    public void TweenLightSize(float newSize, float tweenSpeed)
    {
        DOTween.To(() => pointLight.size, x => pointLight.size = x, newSize, tweenSpeed);
    }


    IEnumerator WaitAndDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void DeploySpellGuided(StageEntity targetEntity)
    {
        DetachFromParent();
        _reachedDestination = false;

        this.targetEntity = targetEntity;
        StartCoroutine(MoveToLocationWithConstantSpeed(targetEntity.SpriteCenterPoint.position, speed));
        StartCoroutine(WaitUntilDestinationReached(() => 
        {
            TriggerImpact();
            ApplyDamage(targetEntity);
        }));
    }

    void ApplyDamage(StageEntity target)
    {
        target.HurtEntity(damagePayload);
    }

    IEnumerator MoveToLocationWithConstantSpeed(Vector3 targetPosition, float speed)
    {
        while(Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        _reachedDestination = true;
    }

    IEnumerator WaitUntilDestinationReached(Action callback = null)
    {
        yield return new WaitUntil(() => _reachedDestination);
        callback?.Invoke();
    }

    public void DeploySpellFreeAim(Vector3 targetPosition)
    {
        DetachFromParent();
        _reachedDestination = false;

        StartCoroutine(MoveToLocationWithConstantSpeed(targetPosition, speed));
    }

    public void DetachFromParent()
    {
        transform.parent = null;
    }

}
