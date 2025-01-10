using Spine.Unity;
using System;
using UnityEngine;


namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringUISceneDependency : Dependency
    {
        [SerializeField] private GameObject gamePlay;
        [SerializeField] private GameObject prefabGameShort;
        [SerializeField] private GameObject prefabGameLong;
        [SerializeField] private GameObject introGalleryRoom;
        [SerializeField] private GameObject outroGalleryRoom;
        [SerializeField] Kindy2ColoringScriptableObject kindy2ColoringScriptable;

        [SerializeField] SkeletonGraphic ellieGallerySkeleton;
        [SerializeField] SkeletonGraphic ellieGalleryOutroSkeleton;

        [SerializeField] BaseButton buttonClose;
        [SerializeField] BaseImage imageWashOut;

        public override T GetStateData<T>()
        {
            T data;

            Type listType = typeof(T);
            if (listType == typeof(Kindy2UISpawnSceneStateObjectDependecy))
            {
                Kindy2UISpawnSceneStateObjectDependecy initSceneStateData = new Kindy2UISpawnSceneStateObjectDependecy();
                initSceneStateData.GamePlay = gamePlay;
                initSceneStateData.PrefabGameLong = prefabGameLong;
                initSceneStateData.PrefabGameShort = prefabGameShort;
                initSceneStateData.IntroGalleryRoom = introGalleryRoom;
                initSceneStateData.OutroGalleryRoom = outroGalleryRoom;
                initSceneStateData.DataFinsh = kindy2ColoringScriptable.dataFinishGame;
                initSceneStateData.InitDataConfig = kindy2ColoringScriptable.initDataConfig;
                initSceneStateData.EllieGallerySkeleton = ellieGallerySkeleton;
                initSceneStateData.EllieGalleryOutroSkeleton = ellieGalleryOutroSkeleton;
                initSceneStateData.ButtonClose = buttonClose;
                initSceneStateData.ImageWashOut = imageWashOut;

                data = ConvertToType<T>(initSceneStateData);
            }

            else
            {
                data = ConvertToType<T>(null);
            }
            return data;
        }

    }
}