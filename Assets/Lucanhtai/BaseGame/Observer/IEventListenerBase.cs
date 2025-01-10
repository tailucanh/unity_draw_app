using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucanhtai.Observer
{
    public interface IEventListenerBase 
    {
        
    }

    /// <summary>
    /// A public interface you'll need to implement for each type of event you want to listen to.
    /// </summary>
    public interface EventListener<T> : IEventListenerBase
    {
        void OnMMEvent(T eventType);
    }
}
