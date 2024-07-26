using System.Collections;
using System.Collections.Generic;
using FunkyCode;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(PlayerController))]
public class PlayerLightManager : MonoBehaviour
{
    [SerializeField] Light2D playerLight;

    [SerializeField] float _minSize = 3f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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


}
