using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetClosestToCursor : MonoBehaviour
{
    [SerializeField] float searchInterval = 0.1f;
    public LayerMask targetLayer;
    public CircleCollider2D searchRadius;
    public CircleCollider2D boundingRadius; // If this is set, any target outside of this radius will be ignored
    public GameObject reticleObject;

    public Transform virtualCursor;
    public bool useVirtualCursor = true;

    [SerializeField] StageEntity target;
    public StageEntity Target { get { return target; } }

    private float searchTimer = 0f;

    Tween reticleTween;



    // Update is called once per frame
    void Update()
    {
        if(!searchRadius || !reticleObject)
        {
            return;
        }

        searchTimer += Time.unscaledDeltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;

            MoveColliderToCursor();

            target = FindClosestTarget();
            if(target != null)
            {
                reticleObject.SetActive(true);
                TweenReticleToPosition(target.SpriteCenterPoint.position);
            }else
            {
                reticleObject.SetActive(false);
            }
        }
    }

    void TweenReticleToPosition(Vector3 position)
    {
        //Check the distance between the reticle and the position and if it's too close, don't tween
        if(Vector3.Distance(reticleObject.transform.position, position) < 0.01f)
        {
            reticleObject.transform.position = position;
            return;
        }

        if (reticleTween.IsActive())
        {
            reticleTween.Kill();
        }

        reticleTween = reticleObject.transform.DOMove(position, 0.15f).SetEase(Ease.OutExpo).SetUpdate(true);
    }

    void MoveColliderToCursor()
    {
        Vector3 cursorPosition;
        if(useVirtualCursor && virtualCursor)
        {
            cursorPosition = virtualCursor.position;
            cursorPosition.z = 0;
            searchRadius.transform.position = cursorPosition;
            return;
        }

        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0;
        searchRadius.transform.position = cursorPosition;
    }


StageEntity FindClosestTarget()
{
    Collider2D[] colliders = Physics2D.OverlapCircleAll(searchRadius.transform.position, searchRadius.radius, targetLayer);
    if (colliders.Length == 0)
    {
        return null;
    }

    StageEntity closestTarget = null;
    float closestDistance = float.MaxValue;

    foreach (Collider2D collider in colliders)
    {
        if (!collider.CompareTag(TagNames.Enemy.ToString()))
        {
            continue;
        }
        if (!collider.TryGetComponent<StageEntity>(out var entity))
        {
            continue;
        }

        // Check if the entity is within the bounding radius
        float distanceToSearchRadius = Vector2.Distance(searchRadius.transform.position, entity.transform.position);
        float distanceToBoundingRadius = boundingRadius != null ? Vector2.Distance(boundingRadius.transform.position, entity.transform.position) : float.MaxValue;

        //Debug.Log($"Entity: {entity.name}, DistanceToSearchRadius: {distanceToSearchRadius}, DistanceToBoundingRadius: {distanceToBoundingRadius}, BoundingRadius: {boundingRadius?.radius ?? float.MaxValue}");

        if (distanceToSearchRadius < closestDistance && (boundingRadius == null || distanceToBoundingRadius <= boundingRadius.radius))
        {
            closestDistance = distanceToSearchRadius;
            closestTarget = entity;
        }
    }

    return closestTarget;
}
}
