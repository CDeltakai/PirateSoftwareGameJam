using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(StageEntity))]
public class StageEntityUIController : MonoBehaviour
{
    StageEntity stageEntity;

    [SerializeField] TextMeshPro HPText;


    void Start()
    {
        stageEntity = GetComponent<StageEntity>();
        stageEntity.OnHPModified += UpdateHPText;
        UpdateHPText();
    }

    void Update()
    {
        
    }

    void UpdateHPText()
    {
        if(!HPText){return;}

        HPText.text = stageEntity.CurrentHP.ToString();
    }


}
