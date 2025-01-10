using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Adapter : MonoBehaviour
{
    public abstract T GetData<T>(int turn);
    public abstract void SetData<T>(T data); 
    public abstract int GetMaxTurn();

    protected T ConvertToType<T>(object data)
    {
        return (T)Convert.ChangeType(data, typeof(T));
    }
}
