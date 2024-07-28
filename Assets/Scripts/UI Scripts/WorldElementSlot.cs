using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldElementSlot : MonoBehaviour
{
    [SerializeField] Image _elementIcon;
    public Image ElementIcon => _elementIcon;

    [SerializeField] Image _elementBackground;
    public Image ElementBackground => _elementBackground;

    public void SetIcon(Sprite icon)
    {
        _elementIcon.enabled = icon != null;
        _elementIcon.sprite = icon;
    }
    
    public void SetBackground(Sprite background)
    {
        _elementBackground.sprite = background;
    }
}
