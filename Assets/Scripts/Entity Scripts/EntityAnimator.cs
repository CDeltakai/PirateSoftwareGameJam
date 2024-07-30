using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    [SerializeField] AnimancerComponent animancer;
    [SerializeField] List<AnimationClip> animationClips;
    [SerializeField] AnimationClip idleClip;

    void Start()
    {
        animancer.Play(idleClip);
    }

    public AnimationClip GetAnimationClip(string clipName)
    {
        return animationClips.Find(clip => clip.name == clipName);
    }

    public void PlayAnimation(int index)
    {
        animancer.Play(animationClips[index]);
    }

}
