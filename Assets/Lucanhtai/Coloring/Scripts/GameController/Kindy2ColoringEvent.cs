using Lucanhtai.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lucanhtai.Coloring
{
    public struct Kindy2ColoringEvent : EventListener<Kindy2ColoringEvent>
    {

        public string EventName;
        public object Data;

        public Kindy2ColoringEvent(string nameEvent, object data)
        {
            this.EventName = nameEvent;
            this.Data = data;
        }

        public void OnMMEvent(Kindy2ColoringEvent eventType)
        {
            throw new System.NotImplementedException();
        }
    }
}