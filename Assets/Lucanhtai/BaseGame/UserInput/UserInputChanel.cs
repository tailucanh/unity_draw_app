using Lucanhtai.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UserInputChanel : EventListener<UserInputChanel>
{
    public string EventName;
    public string ObjectName;
    public void OnMMEvent(UserInputChanel eventType)
    {
        
    }

    public UserInputChanel(string nameEvent, string objectName)
    {
        EventName = nameEvent;
        ObjectName = objectName;      
    }

    public const string BUTTON_CLICK = "BUTTON_CLICK";

}
