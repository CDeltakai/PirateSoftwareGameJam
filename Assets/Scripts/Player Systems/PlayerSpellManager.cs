using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellManager : MonoBehaviour
{
    [SerializeField] SpellEffect currentSpellEffect;


    void Start()
    {
        
    }

    void Update()
    {
        
    }



    public SpellEffect MixSpell(SpellElement element1, SpellElement element2, SpellElement element3)
    {
        SpellEffect spellEffect = new SpellEffect();
        //TODO: Implement spell mixing logic

        return spellEffect;
    }

}
