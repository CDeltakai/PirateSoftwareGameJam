using System.Collections;
using System.Collections.Generic;
using FunkyCode;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(PlayerController))]
public class PlayerLightManager : MonoBehaviour
{
    [SerializeField] Light2D playerLight;
    [SerializeField] CircleCollider2D lightCollider;

    [SerializeField] float _minSize = 2f;
    [SerializeField] float _maxSize = 8f;

    [SerializeField] float _lightTweenSpeed = 0.25f;

    PlayerController playerController;

[Header("Debugging")]
    [SerializeField] bool forceUpdate = false;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.OnHPModified += UpdateLightSize;
        UpdateLightSize();
    }

    // Update is called once per frame
    void Update()
    {
        if(forceUpdate)
        {
            forceUpdate = false;
            UpdateLightSize();
        }

        lightCollider.radius = playerLight.size * 0.95f;
    }

/// <summary>
/// Tweens the size of the player light from its current value to the new size over the specified time.
/// </summary>
/// <param name="newSize"></param>
/// <param name="tweenSpeed"></param>
    public void TweenLightSize(float newSize, float tweenSpeed)
    {
        DOTween.To(() => playerLight.size, x => playerLight.size = x, newSize, tweenSpeed);
    }

    void UpdateLightSize()
    {
        float currentHP = playerController.CurrentHP;
        float maxHP = playerController.MaxHP;
        float healthPercentage = currentHP / maxHP;

        float newSize = Mathf.Lerp(_minSize, _maxSize, healthPercentage);
        TweenLightSize(newSize, _lightTweenSpeed);
    }

}
