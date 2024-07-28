using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWorldSpellMixerUI : MonoBehaviour
{
    [SerializeField] PlayerSpellManager playerSpellManager;
    [SerializeField] List<WorldElementSlot> elementSlots;

    void Start()
    {
        InitElementSlots();

        if(!playerSpellManager)
        {
            Debug.LogError("PlayerSpellManager not set in PlayerWorldSpellMixerUI");
            return;
        }
        playerSpellManager.OnSpellMixChanged += UpdateElementSlots;
    }

    void InitElementSlots()
    {
        elementSlots.Clear();
        elementSlots.AddRange(GetComponentsInChildren<WorldElementSlot>());
    }

    void UpdateElementSlots()
    {
        for (int i = 0; i < elementSlots.Count; i++)
        {
            if(i < playerSpellManager.CurrentSpellMix.Mix.Count)
            {
                elementSlots[i].SetIcon(playerSpellManager.CurrentSpellMix.Mix[i].ElementIcon);
            }else
            {
                elementSlots[i].SetIcon(null);
            }
        }
    }

}
