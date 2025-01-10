using Lucanhtai.Observer;
using System;
using UnityEngine;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringNavigator : Navigator
    {
        private float aspectRatio = 0;
        public override (string, object) GetData(Adapter adapter, string eventName, object eventData)
        {
            Kindy2ColoringState coloringStateReturn = Kindy2ColoringState.INIT_SCENE_STATE;
            object ObjectDataReturn = null;
            Kindy2ColoringState coloringState = (Kindy2ColoringState)Enum.Parse(typeof(Kindy2ColoringState), eventName);
            //LogMe.LogError("Lucanhtai: " + coloringState.ToString());
            aspectRatio = (float)Screen.width / Screen.height;

            switch (coloringState)
            {
                case Kindy2ColoringState.INIT_SCENE_STATE:
                    coloringStateReturn = Kindy2ColoringState.INIT_SCENE_STATE;
                    Kindy2UISpawnScenePlayData uIScenePlayData = adapter.GetData<Kindy2UISpawnScenePlayData>(turn);
                    uIScenePlayData.AspectRatio = aspectRatio;
                    ObjectDataReturn = uIScenePlayData;
                    break;

                case Kindy2ColoringState.INIT_DATA_STATE:
                    coloringStateReturn = Kindy2ColoringState.INIT_DATA_STATE;
                    Kindy2ColoringInitStateData initStateData = adapter.GetData<Kindy2ColoringInitStateData>(turn);
                    ObjectDataReturn = initStateData;
                    break;

                case Kindy2ColoringState.INTRO_STATE:
                    coloringStateReturn = Kindy2ColoringState.INTRO_STATE;
                    Kindy2ColoringIntroStateData introStateData = new Kindy2ColoringIntroStateData();
                    introStateData.AspectRatio = aspectRatio;
                    ObjectDataReturn = introStateData;
                    break;

                case Kindy2ColoringState.CLICK_OBJECT_STATE:
                    coloringStateReturn = Kindy2ColoringState.CLICK_OBJECT_STATE;
                    Kindy2ColoringClickStateDataEvent clickStateDataEvent = (Kindy2ColoringClickStateDataEvent)eventData;
                    ObjectDataReturn = clickStateDataEvent;
                    break;

                case Kindy2ColoringState.PLAY_STATE:
                    coloringStateReturn = Kindy2ColoringState.PLAY_STATE;
                    //Kindy2ColoringPlayStateEventData eventDataPlay = (Kindy2ColoringPlayStateEventData)eventData;
                    break;
                case Kindy2ColoringState.PAUSE_STATE:
                    coloringStateReturn = Kindy2ColoringState.PAUSE_STATE;
                    break;

                case Kindy2ColoringState.OUTRO_STATE:
                    coloringStateReturn = Kindy2ColoringState.OUTRO_STATE;
                    Kindy2ColoringOutroStateData dataOutro = adapter.GetData<Kindy2ColoringOutroStateData>(turn);
                    dataOutro.AspectRatio = aspectRatio;
                    ObjectDataReturn = dataOutro;
                    break;

                case Kindy2ColoringState.GAME_FINISH_STATE:
                  
                    break;
                default:
                    ObjectDataReturn = null;
                    break;
            }

            return (coloringStateReturn.ToString(), ObjectDataReturn);
        }
        private void OnDestroy()
        {
            aspectRatio = 0;
        }

    }
}