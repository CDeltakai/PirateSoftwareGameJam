using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell Element", menuName = "Spell Element")]
public class SpellElementSO : ScriptableObject
{
    [SerializeField] string _elementName;
    public string ElementName => _elementName;

    [SerializeField] Sprite _elementIcon;
    public Sprite ElementIcon => _elementIcon;

    [SerializeField] Color _elementColor;
    public Color ElementColor => _elementColor;

    [SerializeField] bool _unique;
    public bool Unique => _unique;

    [SerializeField] GameObject _particleEffect;
    public GameObject ParticleEffect => _particleEffect;

    [SerializeField] List<GameObject> _elementEffectModifiers;
    public List<GameObject> ElementEffectModifiers => _elementEffectModifiers;

    void OnValidate()
    {
        VerifyElementEffectModifiers();
    }


    //Make sure that the all the elementEffectModifiers contain the ElementEffectModifier component on its root object.
    //If not, throw a warning.
    void VerifyElementEffectModifiers()
    {
        if(_elementEffectModifiers == null || _elementEffectModifiers.Count == 0)
        {
            return;
        }
        for (int i = 0; i < _elementEffectModifiers.Count; i++)
        {
            if(_elementEffectModifiers[i] == null)
            {
                continue;
            }

            if (!_elementEffectModifiers[i].GetComponent<ElementEffectModifier>())
            {
                Debug.LogWarning("ElementEffectModifier component not found on " + _elementEffectModifiers[i].name);
            }
        }
    }

}
