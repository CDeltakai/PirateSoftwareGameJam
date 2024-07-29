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

    [SerializeField] float _cursorSpeed = 5f;

    [SerializeField] Vector2 _cursorVelocity;

[Header("Targeting Algorithms")]
    [SerializeField] TargetClosestToCursor _targetClosestToCursor;

    void Start()
    {
        InitializeCursor();
        InitializeTargetingReticle();
    }

    void Update()
    {
        MoveCursor(_cursorVelocity);
    }

    void OnValidate()
    {
        if(_cursorInstance)
        {
            _cursorInstance.SetActive(CursorActive);
        }
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
        if(!_targetClosestToCursor)
        {
            _targetClosestToCursor = GetComponent<TargetClosestToCursor>();
            _targetClosestToCursor.virtualCursor = _cursorInstance.transform;
            _targetClosestToCursor.reticleObject = _targetingReticleInstance;
            _targetClosestToCursor.useVirtualCursor = true;

            _targetClosestToCursor.searchRadius = _cursorInstance.GetComponent<CircleCollider2D>();
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

    public void MoveCursor(Vector2 direction)
    {
        if(!CursorActive){return;}

        Vector3 moveVector = new Vector3(direction.x, direction.y, 0);
        _cursorInstance.transform.position += moveVector * _cursorSpeed * Time.deltaTime;
    }

    public void ChangeCursorDirection(CallbackContext context)
    {
        if(!CursorActive){return;}

        _cursorVelocity = context.ReadValue<Vector2>();
    }

}
