using Spine.Unity;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringDependency : Dependency
    {
        [SerializeField] private Kindy2ColoringScriptableObject kindy2ColoringScriptable;
        [SerializeField] private Kindy2ColoringPaintController paintController;
        private GameObject layoutGamePlay;
        private GameObject gamePlayDrawing;
        private GameObject introGalleryRoom;
        private GameObject outroGalleryRoom;
        [SerializeField] private GameObject gamePlayObject;
        [SerializeField] private GameObject buttonColor;
        [SerializeField] private GridLayoutGroup layoutColor;
        [SerializeField] private List<BaseButton> listDrawingTools;
        [SerializeField] private ColoringGuiding guidingHand;

        private SkeletonGraphic ellieGallerySkeleton;
        private SkeletonGraphic ellieGalleryOutroSkeleton;

        private BaseButton buttonClose;
        private BaseImage imageWashOut;
        [SerializeField] private Image uIBlockSpamInit;
        [SerializeField] private BaseButton buttonCheckDone;
        [SerializeField] private GameObject buttonCheckNormal;

        public void InitDependency(GameObject introGalleryRoom,
            GameObject outroGalleryRoom, GameObject layoutGamePlay, GameObject gamePlayDrawing, BaseButton buttonClose, BaseImage imageWashOut)
        {
            this.layoutGamePlay = layoutGamePlay;
            this.gamePlayDrawing = gamePlayDrawing;
            this.introGalleryRoom = introGalleryRoom;
            this.outroGalleryRoom = outroGalleryRoom;
            this.buttonClose = buttonClose;
            this.imageWashOut = imageWashOut;
        }

        public void InitEllieSkeleton(SkeletonGraphic ellieGallerySkeleton, SkeletonGraphic ellieGalleryOutroSkeleton)
        {
            this.ellieGallerySkeleton = ellieGallerySkeleton;
            this.ellieGalleryOutroSkeleton = ellieGalleryOutroSkeleton;
        }

        public override T GetStateData<T>()
        {
            T data;

            Type listType = typeof(T);
            if (listType == typeof(Kindy2ColoringInitStateObjectDependecy))
            {
                Kindy2ColoringInitStateObjectDependecy initStateData = new Kindy2ColoringInitStateObjectDependecy();
                initStateData.LocalizationConfig = kindy2ColoringScriptable.localizationConfig;
                initStateData.DataColorConfig = kindy2ColoringScriptable.dataColorConfig;
                initStateData.InitDataConfig = kindy2ColoringScriptable.initDataConfig;
                initStateData.ClickConfig = kindy2ColoringScriptable.clickDataConfig;
                initStateData.PaintController = paintController;
                initStateData.IntroGalleryRoom = introGalleryRoom;
                initStateData.ImageWashOut = imageWashOut;
                initStateData.ButtonColor = buttonColor;
                initStateData.LayoutColor = layoutColor;
                initStateData.ButtonClose = buttonClose;
                initStateData.GamePlayDrawing = gamePlayDrawing;
                initStateData.ListDrawingTools = listDrawingTools;
                initStateData.UIBlockSpamInit = uIBlockSpamInit;
                initStateData.ButtonCheckNormal = buttonCheckNormal;
                initStateData.ButtonCheckDone = buttonCheckDone;
     

                data = ConvertToType<T>(initStateData);
            }
            else if (listType == typeof(Kindy2ColoringIntroStateObjectDependecy))
            {
                Kindy2ColoringIntroStateObjectDependecy introStateData = new Kindy2ColoringIntroStateObjectDependecy();
                introStateData.PaintController = paintController;
                introStateData.InitDataConfig = kindy2ColoringScriptable.initDataConfig;
                introStateData.IntroConfig = kindy2ColoringScriptable.introDataConfig;
                introStateData.IntroGalleryRoom = introGalleryRoom;
                introStateData.ButtonClose = buttonClose;
                introStateData.ImageWashOut = imageWashOut;
                introStateData.GamePlayDrawing = gamePlayDrawing;
                introStateData.UIBlockSpamInit = uIBlockSpamInit;
                introStateData.EllieGallerySkeleton = ellieGallerySkeleton;


                data = ConvertToType<T>(introStateData);
            }
            else if (listType == typeof(Kindy2ColoringClickObjectStateObjectDependecy))
            {
                Kindy2ColoringClickObjectStateObjectDependecy clickStateData = new Kindy2ColoringClickObjectStateObjectDependecy();
                clickStateData.ClickConfig = kindy2ColoringScriptable.clickDataConfig;
                clickStateData.DataColorConfig = kindy2ColoringScriptable.dataColorConfig;
                clickStateData.DataFinish = kindy2ColoringScriptable.dataFinishGame;
                clickStateData.PaintController = paintController;
                clickStateData.GamePlay = layoutGamePlay;
                clickStateData.LayoutColor = layoutColor;
                clickStateData.ListDrawingTools = listDrawingTools;
                clickStateData.GuidingHand = guidingHand;
                clickStateData.IntroGalleryRoom = introGalleryRoom;
                clickStateData.OutroGalleryRoom = outroGalleryRoom;

                data = ConvertToType<T>(clickStateData);
            }
            else if (listType == typeof(Kindy2ColoringPlayStateObjectDependecy))
            {
                Kindy2ColoringPlayStateObjectDependecy playStateData = new Kindy2ColoringPlayStateObjectDependecy();
                playStateData.GuidingConfig = kindy2ColoringScriptable.guidingDataConfig;
                playStateData.ClickConfig = kindy2ColoringScriptable.clickDataConfig;
                playStateData.DataFinish = kindy2ColoringScriptable.dataFinishGame;
                playStateData.GuidingHand = guidingHand;
                playStateData.PaintController = paintController;
                playStateData.UIBlockSpamInit = uIBlockSpamInit;
                playStateData.ImageWashOut = imageWashOut;
                playStateData.ButtonClose = buttonClose;
                playStateData.ButtonCheckDone = buttonCheckDone;
                playStateData.ButtonCheckNormal = buttonCheckNormal;
                playStateData.AudioBackground = kindy2ColoringScriptable.initDataConfig.audioBackground;

                data = ConvertToType<T>(playStateData);
            }
            else if (listType == typeof(Kindy2ColoringPauseStateObjectDependecy))
            {
                Kindy2ColoringPauseStateObjectDependecy pauseStateData = new Kindy2ColoringPauseStateObjectDependecy();
                pauseStateData.IntroGalleryRoom = introGalleryRoom;
                pauseStateData.EllieGallerySkeleton = ellieGallerySkeleton;

                data = ConvertToType<T>(pauseStateData);
            }
            else if (listType == typeof(Kindy2ColoringOutroStateObjectDependecy))
            {
                Kindy2ColoringOutroStateObjectDependecy outroStateData = new Kindy2ColoringOutroStateObjectDependecy();
                outroStateData.DataFinish = kindy2ColoringScriptable.dataFinishGame;
                outroStateData.OutroConfig = kindy2ColoringScriptable.outroConfig;
                outroStateData.ClickConfig = kindy2ColoringScriptable.clickDataConfig;
                outroStateData.InitDataConfig = kindy2ColoringScriptable.initDataConfig;
                outroStateData.OutroGalleryRoom = outroGalleryRoom;
                outroStateData.GamePlayObject = gamePlayObject;
                outroStateData.GamePlay = layoutGamePlay;
                outroStateData.EllieOutroSkeleton = ellieGalleryOutroSkeleton;
                outroStateData.ImageWashOut = imageWashOut;
                outroStateData.PaintController = paintController;
                data = ConvertToType<T>(outroStateData);
            }
            else
            {
                data = ConvertToType<T>(null);
            }
            return data;
        }
    }
}