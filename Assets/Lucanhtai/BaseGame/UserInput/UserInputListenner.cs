using Lucanhtai.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputListenner : MonoBehaviour, EventListener<UserInputChanel>
{
    public virtual void OnMMEvent(UserInputChanel eventType)
    {
       
    }

    private void OnEnable()
    {
        this.ObserverStartListening<UserInputChanel>();
    }

    private void OnDisable()
    {
        this.ObserverStopListening<UserInputChanel>();
    }
}
