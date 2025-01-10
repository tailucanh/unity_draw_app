using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringSystem : FSMSystem
    {
        private FSMState kindy2ColoringInitState;
        private FSMState kindy2ColoringIntroState;
        private FSMState kindy2ColoringPlayState;
        private FSMState kindy2ColoringClickState;
        private FSMState kindy2ColoringPauseState;
        private FSMState kindy2ColoringOutroState;

        private void Awake()
        {
            kindy2ColoringInitState = new Kindy2ColoringInitState();
            kindy2ColoringIntroState = new Kindy2ColoringIntroState();
            kindy2ColoringPlayState = new Kindy2ColoringPlayState();
            kindy2ColoringClickState = new Kindy2ColoringClickObjectState();
            kindy2ColoringPauseState = new Kindy2ColoringPauseState();
            kindy2ColoringOutroState = new Kindy2ColoringOutroState();
        }
        public override void GotoState(string eventName, object data)
        {
            Kindy2ColoringState coloringState = (Kindy2ColoringState)Enum.Parse(typeof(Kindy2ColoringState), eventName);

            switch (coloringState)
            {
                case Kindy2ColoringState.INIT_DATA_STATE:
                    GotoState(kindy2ColoringInitState, data);
                    break;
                case Kindy2ColoringState.INTRO_STATE:
                    GotoState(kindy2ColoringIntroState, data);
                    break;
                case Kindy2ColoringState.PLAY_STATE:
                    GotoState(kindy2ColoringPlayState, data);
                    break;
                case Kindy2ColoringState.CLICK_OBJECT_STATE:
                    GotoState(kindy2ColoringClickState, data);
                    break;
                case Kindy2ColoringState.PAUSE_STATE:
                    GotoState(kindy2ColoringPauseState, data);
                    break;
                case Kindy2ColoringState.OUTRO_STATE:
                    GotoState(kindy2ColoringOutroState, data);
                    break;
            }

        }
        public override void SetupStateData<T>(T data)
        {
            if (data is Dependency dependency)
            {

                Kindy2ColoringInitStateObjectDependecy initStateData = dependency.GetStateData<Kindy2ColoringInitStateObjectDependecy>();
                kindy2ColoringInitState.SetUp(initStateData);

                Kindy2ColoringIntroStateObjectDependecy introStateData = dependency.GetStateData<Kindy2ColoringIntroStateObjectDependecy>();
                kindy2ColoringIntroState.SetUp(introStateData);

                Kindy2ColoringPlayStateObjectDependecy playStateData = dependency.GetStateData<Kindy2ColoringPlayStateObjectDependecy>();
                kindy2ColoringPlayState.SetUp(playStateData);

                Kindy2ColoringPauseStateObjectDependecy pauseStateData = dependency.GetStateData<Kindy2ColoringPauseStateObjectDependecy>();
                kindy2ColoringPauseState.SetUp(pauseStateData);

                Kindy2ColoringClickObjectStateObjectDependecy clickStateData = dependency.GetStateData<Kindy2ColoringClickObjectStateObjectDependecy>();
                kindy2ColoringClickState.SetUp(clickStateData);

                Kindy2ColoringOutroStateObjectDependecy outroStateData = dependency.GetStateData<Kindy2ColoringOutroStateObjectDependecy>();
                kindy2ColoringOutroState.SetUp(outroStateData);
            }

        }
    }
}