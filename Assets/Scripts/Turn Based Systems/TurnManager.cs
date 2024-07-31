using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void TurnActionHandler();
public class TurnAction
{
    public TurnActionHandler ActionToExecute { get; private set; }
    public int Priority { get; private set; }

    /// <summary>
    /// The method that will be called to check if the action can be executed
    /// </summary>
    public Func<bool> CanExecute { get; private set; }


    public TurnAction(int priority, TurnActionHandler action, Func<bool> canExecute)
    {
        Priority = priority;
        ActionToExecute = action;
        CanExecute = canExecute;
    }

    public void Invoke()
    {
        if(CanExecute())
        {
            ActionToExecute?.Invoke();
        }else
        {
            Debug.Log("Action cancelled because the conditions to execute it were not met. Method: " + ActionToExecute.Method.Name);
        }
    }
}

public class TurnManager : MonoBehaviour
{
    public event Action OnNextTurn;


    public static TurnManager Instance { get; private set; }
    public static bool TurnInProgress { get; private set; } = false;

    [SerializeField] int _turnCount = 0;
    public int TurnCount => _turnCount;

    //The queue of actions here is to take into account actions of the same priority
    //such that they are executed in the order they were added 
    SortedList<int, Queue<TurnAction>> turnQueue = new();
    [SerializeField] int turnQueueCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _turnCount = 0;   
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public void AddTurnAction(TurnAction action)
    {
        if (!turnQueue.ContainsKey(action.Priority))
        {
            turnQueue[action.Priority] = new Queue<TurnAction>();
        }

        turnQueue[action.Priority].Enqueue(action);
    }

    /// <summary>
    /// Player actions are always executed first before the rest of actions from other sources
    /// </summary>
    /// <param name="playerAction"></param>
    public void ExecutePlayerAction(TurnAction playerAction, Func<bool> condition = null)
    {
        playerAction.Invoke();
        if (condition != null)
        {
            StartCoroutine(WaitForCondition(condition));
        }
        else
        {
            ExecuteNextTurn();
        }
    }

    IEnumerator WaitForCondition(Func<bool> condition)
    {
        TurnInProgress = true;

        while (!condition())
        {
            yield return null;
        }

        ExecuteNextTurn();
        TurnInProgress = false;

    }

    IEnumerator DelayNextTurn(float delay)
    {
        TurnInProgress = true;
        yield return new WaitForSeconds(delay);
        ExecuteNextTurn();
        TurnInProgress = false;
    }

    public void ExecuteNextTurn()
    {

        while (turnQueue.Count > 0)
        {
            int highestPriority = turnQueue.Keys[0];
            Queue<TurnAction> actionQueue = turnQueue[highestPriority];

            if (actionQueue.Count > 0)
            {
                var action = actionQueue.Dequeue();
                action.Invoke();

                if (actionQueue.Count == 0)
                {
                    turnQueue.Remove(highestPriority);
                }
            }
            turnQueueCount = turnQueue.Count;
        }

        _turnCount++;
        
        OnNextTurn?.Invoke();
    }

}
