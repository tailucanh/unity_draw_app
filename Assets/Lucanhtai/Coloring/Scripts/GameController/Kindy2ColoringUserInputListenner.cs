using Lucanhtai.Observer;
using UnityEngine;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringUserInputListenner : UserInputListenner
    {
        /*private const string BUTTON_CLOSE = "ButtonClose";
        private const string BUTTON_CANCEL = "ButtonCancel";
        private const string BUTTON_CLOSE_UI = "CloseUIPopup";
        [SerializeField] private RectTransform uIPopup;
        [SerializeField] private GameObject introGalleryRoom;
        [SerializeField] private GameObject introDrawingRoom;
        [SerializeField] private GameObject outroGalleryRoom;*/
        public override void OnMMEvent(UserInputChanel eventType)
        {
            Kindy2ColoringEvent dataRequest;
            switch (eventType.EventName)
            {
                case UserInputChanel.BUTTON_CLICK:
                  /*  string[] objectItem = eventType.ObjectName.Split(" ");
                    if (eventType.ObjectName.Equals(BUTTON_CLOSE) && (IsScaleUI(introGalleryRoom) || IsScaleUI(introDrawingRoom)))
                    {
                        uIPopup.gameObject.SetActive(true);
                        dataRequest = new Kindy2ColoringEvent(Kindy2ColoringState.PAUSE_STATE.ToString(), null);
                    }
                    else if ((eventType.ObjectName.Equals(BUTTON_CANCEL) || eventType.ObjectName.Equals(BUTTON_CLOSE_UI)) && (IsScaleUI(introGalleryRoom) || IsScaleUI(introDrawingRoom)))
                    {
                        dataRequest = new Kindy2ColoringEvent(Kindy2ColoringState.INTRO_STATE.ToString(), null);
                    }

                    else if (eventType.ObjectName.Equals(BUTTON_CANCEL) && (!IsScaleUI(introGalleryRoom) || !IsScaleUI(introDrawingRoom)) && !IsScaleUI(outroGalleryRoom))
                    {
                        dataRequest = new Kindy2ColoringEvent(Kindy2ColoringState.PLAY_STATE.ToString(), null);
                    }
                    else
                    {
                        Kindy2ColoringClickStateDataEvent clickStateEventData = new();
                        clickStateEventData.NameObject = eventType.ObjectName;
                        dataRequest = new Kindy2ColoringEvent(Kindy2ColoringState.CLICK_OBJECT_STATE.ToString(), clickStateEventData);
                    }*/

                    Kindy2ColoringClickStateDataEvent clickStateEventData = new();
                    clickStateEventData.NameObject = eventType.ObjectName;
                    dataRequest = new Kindy2ColoringEvent(Kindy2ColoringState.CLICK_OBJECT_STATE.ToString(), clickStateEventData);
                    ObserverManager.TriggerEvent<Kindy2ColoringEvent>(dataRequest);

                    break;
            }
        }
        private bool IsScaleUI(GameObject gameObject)
        {
            return gameObject.transform.localScale == Vector3.one;
        }

    }
}
