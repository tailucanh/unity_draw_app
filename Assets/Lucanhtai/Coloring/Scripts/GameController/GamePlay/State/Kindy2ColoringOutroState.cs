using Cysharp.Threading.Tasks;
using Lucanhtai.Observer;
using Spine;
using Spine.Unity;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringOutroState : FSMState
    {
        private Kindy2ColoringOutroStateObjectDependecy outroObjectDependecy;
        private CancellationTokenSource cts;
        private BaseImage[] images;
        private int countAnimation = 0;
        private Kindy2ColoringHandlesData dataDependency;
        public override void OnEnter(object data)
        {
            base.OnEnter(data);
            Kindy2ColoringOutroStateData outroStateData = (Kindy2ColoringOutroStateData)data;
            DoWork(outroStateData);
        }

        public override void SetUp(object data)
        {
            outroObjectDependecy = (Kindy2ColoringOutroStateObjectDependecy)data;
        }

        private async void DoWork(Kindy2ColoringOutroStateData outroStateData)
        {
            cts = new CancellationTokenSource();
            dataDependency = new Kindy2ColoringHandlesData();
            outroObjectDependecy.PaintController.SavePixelsToFile(() => { });
            dataDependency.DeleteMusicBackground();

            outroObjectDependecy.PaintController.IsDrawing = false;
            float aspectRatio = outroStateData.AspectRatio;
            float sceneResolution = outroObjectDependecy.InitDataConfig.SCENCE_RESOLUTION;
            Color32 colorWashOut = outroObjectDependecy.InitDataConfig.colorWashOut;
            Color32 colorWashIn = outroObjectDependecy.InitDataConfig.colorWashIn;
            int timeDelayOut = outroObjectDependecy.InitDataConfig.timeDelayOut;
            int timeDelayIn = outroObjectDependecy.InitDataConfig.timeDelayIn;

            images = outroObjectDependecy.OutroGalleryRoom.GetComponentsInChildren<BaseImage>();
            try
            {
                outroObjectDependecy.ImageWashOut.SetColor(colorWashOut, timeDelayOut / 1000f);
                await UniTask.Delay(timeDelayOut, cancellationToken: cts.Token);
                outroObjectDependecy.OutroGalleryRoom.transform.localScale = Vector3.one;
                outroObjectDependecy.GamePlayObject.transform.localScale = Vector3.zero;

                Texture2D textureToSet = outroObjectDependecy.PaintController.GetScreenshot();
                Sprite sprite = Sprite.Create(textureToSet, new Rect(0, 0, textureToSet.width, textureToSet.height), new Vector2(0.5f, 0.5f));
                images[0].GetComponent<Image>().sprite = sprite;
                images[1].GetComponent<Image>().sprite = outroStateData.DataPlay.dataImage;

                outroObjectDependecy.ImageWashOut.SetColor(colorWashIn, timeDelayIn / 800f);

                SoundChannel soundData = new SoundChannel(SoundChannel.PLAY_SOUND_NEW_OBJECT, outroObjectDependecy.OutroConfig.sfxsVictory, null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);

                outroObjectDependecy.EllieOutroSkeleton.AnimationState.Event += HandleEventEllie;
                SetAnimationEllie(aspectRatio, sceneResolution, colorWashOut, timeDelayOut);
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Timeout");
            }
        }

        private void SetAnimationEllie(float aspectRatio, float sceneResolution, Color32 colorWashOut, int timeDelayOut)
        {
            outroObjectDependecy.EllieOutroSkeleton.AnimationState.SetAnimation(0, outroObjectDependecy.InitDataConfig.ellieOutroGallery
                + (aspectRatio > sceneResolution ? "Iphone" : "Tablet"), false).Complete += async (trackEntry) =>
                {
                    SetAnimationEllie(aspectRatio, sceneResolution, colorWashOut, timeDelayOut);
                    countAnimation++;
                    if (countAnimation == 2)
                    {
                        try
                        {
                            outroObjectDependecy.ImageWashOut.SetColor(colorWashOut, timeDelayOut / 1000f);
                            await UniTask.Delay(timeDelayOut, cancellationToken: cts.Token);
                            TriggerFinishOutro();
                        }
                        catch (OperationCanceledException ex)
                        {
                            Debug.Log("Timeout");
                        }
                    }
                };
        }
        private void HandleEventEllie(TrackEntry trackEntry, Spine.Event e)
        {
            SoundChannel soundData;
            //LogMe.LogError("Lucanhtai event outro: " + e.Data.Name);
            if (e.Data.Name.Equals(outroObjectDependecy.InitDataConfig.EVENT_OUTRO))
            {
                if (countAnimation < 2)
                {
                    soundData = new SoundChannel(SoundChannel.PLAY_SOUND, outroObjectDependecy.OutroConfig.sfxsCheered, null, 1, false);
                    ObserverManager.TriggerEvent<SoundChannel>(soundData);
                }
            }
        }
        private void TriggerFinishOutro()
        {
            outroObjectDependecy.DataFinish.score = 0;
            outroObjectDependecy.DataFinish.isSendEvent = true;
            Kindy2ColoringEvent kindy2ColoringEvent = new Kindy2ColoringEvent(Kindy2ColoringState.GAME_FINISH_STATE.ToString(), null);
            ObserverManager.TriggerEvent<Kindy2ColoringEvent>(kindy2ColoringEvent);
            System.GC.Collect();
            GameObject.Destroy(outroObjectDependecy.GamePlay, .1f);
        }



        public override void OnExit()
        {
            countAnimation = 0;
            cts?.Cancel();
        }
        public override void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }

    public class Kindy2ColoringOutroStateData
    {
        public Kindy2ColoringConversationData DataPlay { get; set; }
        public float AspectRatio;
    }

    public class Kindy2ColoringOutroStateObjectDependecy
    {
        public Kindy2ColoringInitDataConfig InitDataConfig { get; set; }
        public Kindy2ColoringPaintController PaintController { get; set; }
        public Kindy2ColoringClickConfig ClickConfig { get; set; }
        public Kindy2ColoringOutroConfig OutroConfig { get; set; }
        public GameObject GamePlay { get; set; }
        public BaseImage ImageWashOut { get; set; }
        public Kindy2ColoringDataFinish DataFinish { get; set; }
        public GameObject OutroGalleryRoom { get; set; }
        public SkeletonGraphic EllieOutroSkeleton { get; set; }
        public GameObject GamePlayObject { get; set; }
    }
}