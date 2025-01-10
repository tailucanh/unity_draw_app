using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dependency : MonoBehaviour
{
    public abstract T GetStateData<T>();

    protected T ConvertToType<T>(object data)
    {
        return (T)Convert.ChangeType(data, typeof(T));
    }
}
