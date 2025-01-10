using Lucanhtai.Observer;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringUISpawnSceneState : FSMState
    {

        private Kindy2UISpawnSceneStateObjectDependecy initObjectDependecy;
        private float sizeCamera = 0;
        private Kindy2UISpawnScenePlayData dataPlay;

        public override void OnEnter(object data)
        {
            base.OnEnter(data);
            dataPlay = (Kindy2UISpawnScenePlayData)data;
            sizeCamera = dataPlay.CameraGame.orthographicSize;
            SetData(dataPlay);
        }

        public override void SetUp(object data)
        {
            initObjectDependecy = (Kindy2UISpawnSceneStateObjectDependecy)data;
        }

        private void SetData(Kindy2UISpawnScenePlayData dataPlay)
        {
            if (PlayerPrefs.GetFloat(initObjectDependecy.DataFinsh.key_pref + dataPlay.DataPlay.idImage.ToString()) > 0)
            {
                float getTime = PlayerPrefs.GetFloat(initObjectDependecy.DataFinsh.key_pref + dataPlay.DataPlay.idImage.ToString());
                initObjectDependecy.DataFinsh.timeSpent = getTime;
            }
            else
            {
                initObjectDependecy.DataFinsh.timeSpent = 0;
            }
            initObjectDependecy.DataFinsh.timeStart = 0;
            initObjectDependecy.DataFinsh.timeEnd = 0;
            initObjectDependecy.DataFinsh.timeStart = Time.realtimeSinceStartup;
            initObjectDependecy.DataFinsh.score = 0;
            initObjectDependecy.DataFinsh.isSendEvent = true;
            GameObject spawnPrefab;
            if (dataPlay.AspectRatio > initObjectDependecy.InitDataConfig.SCENCE_RESOLUTION)
            {
                SetElliePosition(15f);
                spawnPrefab = SpawnPrefab(initObjectDependecy.PrefabGameLong, dataPlay);
                if (spawnPrefab != null)
                    Object.Destroy(initObjectDependecy.GamePlay.GetComponent<GameManager>());
            }
            else
            {
                SetElliePosition(-55f);
                spawnPrefab =  SpawnPrefab(initObjectDependecy.PrefabGameShort, dataPlay);
                if (spawnPrefab != null)
                    Object.Destroy(initObjectDependecy.GamePlay.GetComponent<GameManager>());
            }

        }

        private void SetElliePosition(float posY)
        {
            GetRectByUI(initObjectDependecy.EllieGallerySkeleton).anchoredPosition = new(GetRectByUI(initObjectDependecy.EllieGallerySkeleton).anchoredPosition.x, posY);
            GetRectByUI(initObjectDependecy.EllieGalleryOutroSkeleton).anchoredPosition = new(GetRectByUI(initObjectDependecy.EllieGalleryOutroSkeleton).anchoredPosition.x, posY);
        }

        private RectTransform GetRectByUI(SkeletonGraphic elllie)
        {
            return elllie.GetComponent<RectTransform>();
        }

        private GameObject SpawnPrefab(GameObject gameObject, Kindy2UISpawnScenePlayData playData)
        {
            GameObject item = Object.Instantiate(gameObject, initObjectDependecy.GamePlay.transform, false);
            playData.CameraGame.orthographicSize = 5f;
            Canvas[] canvasUIs = item.GetComponentsInChildren<Canvas>();

            foreach (var canvasItem in canvasUIs)
            {
                canvasItem.worldCamera = playData.CameraGame;
            }
            item.GetComponent<Adapter>().SetData(playData.DataPlay);
            item.GetComponent<Kindy2ColoringDependency>().InitDependency(initObjectDependecy.IntroGalleryRoom, initObjectDependecy.OutroGalleryRoom,
                initObjectDependecy.GamePlay, item, initObjectDependecy.ButtonClose, initObjectDependecy.ImageWashOut);
           
           /* item.GetComponent<Kindy2ColoringDependency>().InitPopup(initObjectDependecy.UIPopup, initObjectDependecy.PopUpTitle,
                initObjectDependecy.PopUpDescription, initObjectDependecy.PopUpCancel, initObjectDependecy.PopUpExit);*/

            item.GetComponent<Kindy2ColoringDependency>().InitEllieSkeleton(initObjectDependecy.EllieGallerySkeleton, initObjectDependecy.EllieGalleryOutroSkeleton);

            return item;
        }


        public override void OnExit()
        {
        }
        public override void OnDestroy()
        {
            if (dataPlay.CameraGame == null) return;
                dataPlay.CameraGame.orthographicSize = sizeCamera;
        }
    }

    public class Kindy2UISpawnScenePlayData
    {
        public Kindy2ColoringConversationData DataPlay;
        public Camera CameraGame;
        public float AspectRatio;
    }



    public class Kindy2UISpawnSceneStateObjectDependecy
    {
        public Kindy2ColoringInitDataConfig InitDataConfig { get; set; }
        public GameObject GamePlay { get; set; }
        public GameObject PrefabGameLong { get; set; }
        public GameObject PrefabGameShort { get; set; }
        public GameObject IntroGalleryRoom { get; set; }
        public GameObject OutroGalleryRoom { get; set; }
        public Kindy2ColoringDataFinish DataFinsh { get; set; }
        public SkeletonGraphic EllieGallerySkeleton { get; set; }
        public SkeletonGraphic EllieGalleryOutroSkeleton { get; set; }
        public BaseButton ButtonClose { get; set; }
        public BaseImage ImageWashOut { get; set; }
    }
}