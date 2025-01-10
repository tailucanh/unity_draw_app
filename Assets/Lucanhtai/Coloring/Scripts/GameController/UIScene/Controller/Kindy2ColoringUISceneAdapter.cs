using System;
using UnityEngine;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringUISceneAdapter : Adapter
    {
       //[SerializeField] bool isMookData;
       // [SerializeField] Kindy2ColoringMookData mookData;
        private Kindy2ColoringPlayData kindy2ColoringPlayData;
        public override T GetData<T>(int turn)
        {
            T data;
            /* if (isMookData)
             {
                 kindy2ColoringPlayData = new Kindy2ColoringPlayData();
                 kindy2ColoringPlayData.dataPlay = new Kindy2ColoringConversationData();
                 kindy2ColoringPlayData.dataPlay = mookData.dataMook;
                 kindy2ColoringPlayData.cameraGame = Camera.main;
             }*/

            Type listType = typeof(T);
            if (listType == typeof(Kindy2UISpawnScenePlayData))
            {
                Kindy2UISpawnScenePlayData uIScenePlayData = new Kindy2UISpawnScenePlayData();
                uIScenePlayData.DataPlay = kindy2ColoringPlayData.dataPlay;
                uIScenePlayData.CameraGame = kindy2ColoringPlayData.cameraGame;
                data = ConvertToType<T>(uIScenePlayData);
            }
            else
            {
                data = ConvertToType<T>(null);
            }

            return data;
        }

        public override int GetMaxTurn()
        {
            return 1;
        }


        public override void SetData<T>(T data)
        {
            
        }
    }
    public class Kindy2ColoringPlayData
    {
        public Kindy2ColoringConversationData dataPlay;
        public Camera cameraGame;
    }
    //Data for play
    [Serializable]
    public class Kindy2ColoringConversationData
    {
        public int idImage;
        public int idImageLow;
        public Sprite dataImage;
        public Sprite dataImageLow;
    }

    public class Kindy2ColoringData
    {
        public int image_draw;
        public int title;
        public int image_draw_low;
    }
}