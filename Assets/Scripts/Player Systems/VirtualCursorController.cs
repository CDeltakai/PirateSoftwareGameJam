using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class VirtualCursorController : MonoBehaviour
{
    public bool CursorActive = false;

    [SerializeField] GameObject _cursorPrefab;
    [SerializeField] GameObject _cursorInstance;
    [SerializeField] GameObject _targetingReticlePrefab;
    [SerializeField] GameObject _targetingReticleInstance;

    [SerializeField] CircleCollider2D _boundingRadius; // if set, the cursor will be confined to this radius
    [SerializeField] float _cursorSpeed = 5f;
    [SerializeField] Vector2 _cursorVelocity;



[Header("Targeting Algorithms")]
    [SerializeField] TargetingControlType _targetingType;
    [SerializeField] TargetClosestToCursor _targetClosestToCursor;
    [SerializeField] FreeAimTargeting _freeAimTargeting;

    void Start()
    {
        InitializeCursor();
        InitializeTargetingReticle();
    }

    void Update()
    {
        MoveCursor(_cursorVelocity);
    }

    public void InitializeCursor()
    {
        if(_cursorInstance == null)
        {
            _cursorInstance = Instantiate(_cursorPrefab, transform.position, Quaternion.identity);
        }

        CursorActive = false;
        _cursorInstance.SetActive(CursorActive);
    }

    public void InitializeTargetingReticle()
    {
        if(_targetingReticleInstance == null)
        {
            _targetingReticleInstance = Instantiate(_targetingReticlePrefab, transform.position, Quaternion.identity);
        }

        _targetingReticleInstance.SetActive(false);

        // Initialize the TargetClosestToCursor script
        if(_targetClosestToCursor)
        {
            _targetClosestToCursor.virtualCursor = _cursorInstance.transform;
            _targetClosestToCursor.reticleObject = _targetingReticleInstance;
            _targetClosestToCursor.useVirtualCursor = true;

            _targetClosestToCursor.searchRadius = _cursorInstance.GetComponent<CircleCollider2D>();
        }

        if(_freeAimTargeting)
        {
            _freeAimTargeting.virtualCursor = _cursorInstance.transform;
            _freeAimTargeting.reticleSprite = _targetingReticleInstance;
            _freeAimTargeting.useVirtualCursor = true;
        }

    }

    /// <summary>
    /// Toggles the cursor on and off. NOTE: Make sure that player movement is disabled when the cursor is active in order to prevent the player
    /// from moving around whilst trying to control the cursor as they are bound to the same controls.
    /// </summary>
    public void ToggleCursor()
    {
        CursorActive = !CursorActive;
        _cursorInstance.SetActive(CursorActive);
    }

    public void SetCursor(bool active)
    {

        CursorActive = active;
        _cursorInstance.transform.position = transform.position;
        _cursorInstance.SetActive(CursorActive);

        _targetingReticleInstance.transform.position = transform.position; 
        _targetingReticleInstance.SetActive(CursorActive);
    }

    public object GetTarget()
    {
        return _targetingType switch
        {
            TargetingControlType.Guided => _targetClosestToCursor.Target,
            TargetingControlType.FreeAim => _freeAimTargeting.TargetPosition,
            _ => null,
        };
    }

    public void SetTargetingType(TargetingControlType controlType)
    {
        switch(controlType)
        {
            case TargetingControlType.Guided:
                _targetingType = controlType;
                _targetClosestToCursor.enabled = true;
                _freeAimTargeting.enabled = false;
                _targetingReticleInstance.SetActive(true);
                break;
            case TargetingControlType.FreeAim:
                _targetingType = controlType;
                _targetClosestToCursor.enabled = false;
                _freeAimTargeting.enabled = true;
                _targetingReticleInstance.SetActive(true);
                break;
        }
    }

    public void MoveCursor(Vector2 direction)
    {
        if(!CursorActive){return;}

        Vector3 moveVector = new(direction.x, direction.y, 0);
        Vector3 newPosition = _cursorInstance.transform.position + _cursorSpeed * Time.deltaTime * moveVector;

        if (_boundingRadius != null)
        {
            Vector3 center = _boundingRadius.transform.position;
            float radius = _boundingRadius.radius;

            Vector3 clampedPosition = center + Vector3.ClampMagnitude(newPosition - center, radius);
            _cursorInstance.transform.position = clampedPosition;
        }
        else
        {
            _cursorInstance.transform.position = newPosition;
        }
    }

    public void ChangeCursorDirection(CallbackContext context)
    {
        if(!CursorActive){return;}

        _cursorVelocity = context.ReadValue<Vector2>();
    }

}
