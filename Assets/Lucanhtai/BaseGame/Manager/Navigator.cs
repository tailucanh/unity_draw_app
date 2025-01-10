using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Navigator : MonoBehaviour
{
    protected int turn;
    public abstract (string, object) GetData(Adapter adapter, string eventName, object eventData);
}
