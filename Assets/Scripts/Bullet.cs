using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] bool _reachedDestination = false;
    public bool ReachedDestination { get { return _reachedDestination; } }

    public Vector3 targetPosition;
    public float speed;

    void Start()
    {
        
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void MoveToLocationThenSelfDestruct(Vector3 targetPosition, float speed)
    {
        StartCoroutine(MoveToLocationWithConstantSpeed(targetPosition, speed));
        StartCoroutine(WaitUntilDestinationReached(() => 
        {
            Destroy(gameObject);
        }));
    }   

    IEnumerator MoveToLocationWithConstantSpeed(Vector3 targetPosition, float speed)
    {
        while(Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        _reachedDestination = true;
    }

    IEnumerator WaitUntilDestinationReached(Action callback = null)
    {
        float startTime = Time.time;
        while (!_reachedDestination)
        {
            if (Time.time - startTime > 2f)
            {
                break;
            }
            yield return null;
        }
        callback?.Invoke();
    }


}
