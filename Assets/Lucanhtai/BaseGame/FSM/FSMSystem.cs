using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMSystem: MonoBehaviour
{
    protected FSMState currentState { set; get; }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
        OnSystemUpdate();
    }
    private void LateUpdate()
    {
        if (currentState != null)
        {
            currentState.OnLateUpdate();
        }
        OnSystemLateUpdate();
    }
    private void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.OnFixedUpdate();
        }
        OnSystemFixedUpdate();
    }

    private void OnDestroy()
    {
        if (currentState != null)
            currentState.OnDestroy();
    }

    private void OnApplicationPause(bool pause)
    {
        if (currentState != null)
            currentState.OnApplicationPause(pause);
    }

    public abstract void SetupStateData<T>(T data);
    

    public void GotoState(FSMState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter();
    }
    public void GotoState(FSMState newState, object data)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter(data);
    }

    public virtual void GotoState(string eventName, object data) { }

    public virtual void OnSystemUpdate()
    {

    }
    public virtual void OnSystemLateUpdate()
    {

    }
    public virtual void OnSystemFixedUpdate()
    {

    }

}
