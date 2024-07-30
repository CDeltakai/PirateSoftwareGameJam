using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FreeAimTargeting : MonoBehaviour
{
    [SerializeField] float searchInterval = 0.1f;

    [SerializeField] GroundTileData _targetTile;
    [SerializeField] Vector3 _targetPosition;
    public GroundTileData TargetTile => _targetTile;
    public Vector3 TargetPosition => _targetPosition;

    [Header("Reticle Settings")]
    public GameObject reticleSprite;
    public Transform virtualCursor;
    public bool useVirtualCursor = false;


    StageManager stageManager;
    float searchTimer = 0f;
    Tween reticleTween;


    void Start()
    {
        stageManager = StageManager.Instance;        
    }


    // Update is called once per frame
    void Update()
    {
        if(!reticleSprite || !virtualCursor)
        {
            return;
        }

        if(!virtualCursor.gameObject.activeInHierarchy && useVirtualCursor)
        {
            return;
        }

        searchTimer += Time.unscaledDeltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            FindClosestGroundTileData();
            TweenReticleToPosition(_targetPosition);
        }
    }

    void FindClosestGroundTileData()
    {
        if (useVirtualCursor)
        {
            _targetTile = stageManager.FindClosestGroundTile(virtualCursor.position);
            _targetPosition = _targetTile.worldPosition;
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        _targetTile = stageManager.FindClosestGroundTile(mousePos);
        _targetPosition = _targetTile.worldPosition;
    }

    void TweenReticleToPosition(Vector3 position)
    {
        //Check the distance between the reticle and the position and if it's too close, don't tween
        if(Vector3.Distance(reticleSprite.transform.position, position) < 0.01f)
        {
            reticleSprite.transform.position = position;
            return;
        }

        if (reticleTween.IsActive())
        {
            reticleTween.Kill();
        }

        reticleTween = reticleSprite.transform.DOMove(position, 0.15f).SetEase(Ease.OutExpo).SetUpdate(true);
    }

}
