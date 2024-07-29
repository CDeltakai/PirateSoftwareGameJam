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

}
