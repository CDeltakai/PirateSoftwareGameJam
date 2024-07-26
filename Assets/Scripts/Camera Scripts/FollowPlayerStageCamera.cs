using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FollowPlayerStageCamera : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform playerTransform;



    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if(playerTransform != null)
        {
            InitCamera(playerTransform);
        }
    }

    void InitCamera(Transform transform)
    {
        playerTransform = transform;

        virtualCamera.Follow = playerTransform;
    }


}
