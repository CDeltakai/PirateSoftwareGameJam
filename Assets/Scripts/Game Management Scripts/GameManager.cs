using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance{get; private set;}
    public PlayerController PlayerRef{get; private set;}


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            PlayerRef = FindObjectOfType<PlayerController>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
