using System;

public abstract class FSMState
{
    public abstract void SetUp(object data);
   
    public virtual void OnEnter()
    {

    }
    public virtual void OnEnter(object data)
    {

    }

    public virtual void OnUpdate()
    {

    }
    public virtual void OnLateUpdate()
    {

    }
    public virtual void OnFixedUpdate()
    {

    }

    public virtual void OnExit()
    {

    }
    public virtual void OnDestroy()
    {

    }
    public virtual void OnApplicationPause(bool pause)
    {

    }
}
