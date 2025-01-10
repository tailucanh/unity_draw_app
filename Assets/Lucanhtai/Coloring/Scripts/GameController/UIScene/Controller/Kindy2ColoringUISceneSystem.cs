using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringUISceneSystem : FSMSystem
    {
        private FSMState kindy2UISpawnSceneState;

        private void Awake()
        {
            kindy2UISpawnSceneState = new Kindy2ColoringUISpawnSceneState();
        }
        public override void GotoState(string eventName, object data)
        {
            Kindy2ColoringState coloringState = (Kindy2ColoringState)Enum.Parse(typeof(Kindy2ColoringState), eventName);
            switch (coloringState)
            {
                case Kindy2ColoringState.INIT_SCENE_STATE:
                    GotoState(kindy2UISpawnSceneState, data);
                    break;
            }

        }
        public override void SetupStateData<T>(T data)
        {
            if (data is Dependency dependency)
            {
                Kindy2UISpawnSceneStateObjectDependecy initSceneData = dependency.GetStateData<Kindy2UISpawnSceneStateObjectDependecy>();
                kindy2UISpawnSceneState.SetUp(initSceneData);

            }

        }
    }
}