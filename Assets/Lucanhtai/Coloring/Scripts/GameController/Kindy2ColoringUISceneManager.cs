using Lucanhtai.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringUISceneManager : GameManager, EventListener<Kindy2ColoringEvent>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            this.ObserverStartListening<Kindy2ColoringEvent>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.ObserverStopListening<Kindy2ColoringEvent>();
        }
        public override void SetData<T>(T data)
        {
            base.SetData(data);
            adapter.SetData(data);
        }

        protected override void Start()
        {
            base.Start();
            fSMSystem.SetupStateData(dependency);
            Kindy2ColoringEvent kindy2ColoringEvent = new Kindy2ColoringEvent(Kindy2ColoringState.INIT_SCENE_STATE.ToString(), null);
            ObserverManager.TriggerEvent<Kindy2ColoringEvent>(kindy2ColoringEvent);
        }
        public void OnMMEvent(Kindy2ColoringEvent eventType)
        {
            (string eventName, object data) navigatorData = navigator.GetData(adapter, eventType.EventName, eventType.Data);
            fSMSystem.GotoState(navigatorData.eventName, navigatorData.data);
        }
    }
}