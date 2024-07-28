using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellMixerInput : MonoBehaviour
{
    [SerializeField] InputActionAsset spellMixerControls;
    InputAction selectElementAction;
    Dictionary<string, int> keyToIndex = new();

    void Awake()
    {
        InitializeIndexDictionary();
    }

    void Start()
    {
        
    }

    void InitializeIndexDictionary()
    {
        keyToIndex.Add("Q", 0);
        keyToIndex.Add("W", 1);
        keyToIndex.Add("E", 2);
        keyToIndex.Add("A", 3);
        keyToIndex.Add("S", 4);
        keyToIndex.Add("D", 5);

        var playerActionMap = spellMixerControls.FindActionMap("Player");
        selectElementAction = playerActionMap.FindAction("SelectElement");

        selectElementAction.Enable();

        selectElementAction.performed += SelectElement;
    }

    void SelectElement(InputAction.CallbackContext context)
    {
        // Get the key that was pressed
        var key = context.control.displayName.ToUpper();

        if (keyToIndex.TryGetValue(key, out int index))
        {
            // Use the index to select the element from the list
            //Debug.Log("Key pressed: " + key + " - Index: " + index);
            
        }
        else
        {
            Debug.LogWarning("Key not mapped: " + key);
        }
    }


}
