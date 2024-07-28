using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerSpellManager))]
public class SpellMixerInput : MonoBehaviour
{
    [SerializeField] InputActionAsset spellMixerControls;
    InputAction selectElementAction;
    Dictionary<string, int> keyToIndex = new();

    PlayerSpellManager playerSpellManager;

    void Awake()
    {
        InitializeIndexDictionary();
    }

    void Start()
    {
        playerSpellManager = GetComponent<PlayerSpellManager>();
    }

    void InitializeIndexDictionary()
    {
        keyToIndex.Add("Q", 0);
        keyToIndex.Add("W", 1);
        keyToIndex.Add("E", 2);
        keyToIndex.Add("A", 3);
        keyToIndex.Add("S", 4);
        keyToIndex.Add("D", 5);

        InputActionMap playerActionMap = spellMixerControls.FindActionMap("Player");

        if(playerActionMap == null)
        {
            Debug.LogError("Player action map not found in: " + spellMixerControls.name);
            return;
        }

        selectElementAction = playerActionMap.FindAction("SelectElement");

        if(selectElementAction == null)
        {
            Debug.LogError("SelectElement action not found in: " + playerActionMap.name);
            return;
        }

        selectElementAction.Enable();
        selectElementAction.performed += SelectElement;
    }

    void SelectElement(InputAction.CallbackContext context)
    {
        // Get the key that was pressed
        string key = context.control.displayName.ToUpper();

        if (keyToIndex.TryGetValue(key, out int index))
        {
            // Use the index to select the element from the list
            //Debug.Log("Key pressed: " + key + " - Index: " + index);
            playerSpellManager.AddElementToMix(index);

        }
        else
        {
            Debug.LogWarning("Key not mapped: " + key);
        }
    }


}
