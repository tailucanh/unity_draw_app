using Cysharp.Threading.Tasks;
using Lucanhtai.Observer;
using Spine;
using Spine.Unity;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringIntroState : FSMState
    {
        private Kindy2ColoringIntroStateObjectDependecy introObjectDependecy;
        private CancellationTokenSource cts;
        private Color32 colorWashIn;

        public override void OnEnter(object data)
        {
            base.OnEnter(data);
            Kindy2ColoringIntroStateData introStateData = (Kindy2ColoringIntroStateData)data;
            DoWork(introStateData);
        }

        public override void SetUp(object data)
        {
            introObjectDependecy = (Kindy2ColoringIntroStateObjectDependecy)data;
        }

        private async void DoWork(Kindy2ColoringIntroStateData introStateData)
        {
            cts = new CancellationTokenSource();
            float aspectRatio = introStateData.AspectRatio;
            float sceneResolution = introObjectDependecy.InitDataConfig.SCENCE_RESOLUTION;
            Color32 colorWashOut = introObjectDependecy.InitDataConfig.colorWashOut;
            colorWashIn = introObjectDependecy.InitDataConfig.colorWashIn;
            int timeDelayOut = introObjectDependecy.InitDataConfig.timeDelayOut;
            int timeDelayIn = introObjectDependecy.InitDataConfig.timeDelayIn;

            try
            {
                introObjectDependecy.EllieGallerySkeleton.AnimationState.TimeScale = 0f;
                await UniTask.Delay(1000, cancellationToken: cts.Token);

                introObjectDependecy.EllieGallerySkeleton.AnimationState.TimeScale = 1f;

                introObjectDependecy.EllieGallerySkeleton.AnimationState.Event += HandleEventEllie;
                introObjectDependecy.EllieGallerySkeleton.AnimationState.SetAnimation(0, introObjectDependecy.InitDataConfig.ellieIntroGallery
                + (aspectRatio > sceneResolution ? "Iphone" : "Tablet"), false).Complete += async (trackEntry) =>
                {

                    introObjectDependecy.ImageWashOut.SetColor(colorWashOut, (timeDelayOut / 1000f));
                    introObjectDependecy.ButtonClose.Enable(false);
                    await UniTask.Delay(timeDelayOut, cancellationToken: cts.Token);
                    introObjectDependecy.IntroGalleryRoom.transform.localScale = Vector3.zero;
                    introObjectDependecy.GamePlayDrawing.transform.localScale = Vector3.one;
                    introObjectDependecy.ImageWashOut.SetColor(colorWashIn, (timeDelayIn / 1000f));
                    introObjectDependecy.ButtonClose.Enable(true);
                    await UniTask.Delay(timeDelayIn, cancellationToken: cts.Token);
                    TriggerFinishIntro();
                };
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Timeout");
            }
        }
        private void HandleEventEllie(TrackEntry trackEntry, Spine.Event e)
        {
            SoundChannel soundData;
            //LogMe.LogError("Lucanhtai event 1: " + e.Data.Name);
            if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_FIRST_RUN))
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.sfxsRun[0], null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_BRAKE))
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.sfxsBrake, null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_SECOND_RUN))
            {
                introObjectDependecy.ImageWashOut.SetColor(colorWashIn);
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.sfxsRun[1], null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_FIRST_SPEECH_INTRO_GALLERY_1))
            {
                introObjectDependecy.ButtonClose.Enable(true);
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.audioFirstSpeechIntroGallery[0], null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_FIRST_SPEECH_INTRO_GALLERY_2))
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.audioFirstSpeechIntroGallery[1], null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_BLINK))
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.sfxsBlink, null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_SECOND_SPEECH_INTRO_GALLERY))
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.audioSecondSpeechIntroGallery, null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_PENTOSS_1))
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.sfxsPenToss[0], null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_PENTOSS_2))
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.sfxsPenToss[1], null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
            else if (e.Data.Name.Equals(introObjectDependecy.InitDataConfig.EVENT_SPEECH_INTRO_DRAWING))
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, introObjectDependecy.IntroConfig.audioSpeechIntroDrawing, null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }
        }

        private void TriggerFinishIntro()
        {
            introObjectDependecy.UIBlockSpamInit.raycastTarget = false;
            introObjectDependecy.ImageWashOut.SetColor(colorWashIn);
            SoundChannel soundData = new SoundChannel(SoundChannel.PLAY_SOUND_NEW_OBJECT, introObjectDependecy.InitDataConfig.audioBackground, null, 1, true);
            ObserverManager.TriggerEvent<SoundChannel>(soundData);
            Kindy2ColoringEvent kindy2ColoringEvent = new Kindy2ColoringEvent(Kindy2ColoringState.PLAY_STATE.ToString(), null);
            ObserverManager.TriggerEvent<Kindy2ColoringEvent>(kindy2ColoringEvent);
        }

        public override void OnExit()
        {

            SoundManager.Instance.PauseFx(true);
            cts?.Cancel();
        }
        public override void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }

    public class Kindy2ColoringIntroStateData
    {
        public float AspectRatio;
    }

    public class Kindy2ColoringIntroStateObjectDependecy
    {
        public Kindy2ColoringPaintController PaintController { get; set; }
        public Kindy2ColoringIntroConfig IntroConfig { get; set; }
        public Kindy2ColoringInitDataConfig InitDataConfig { get; set; }
        public GameObject IntroGalleryRoom { get; set; }
        public GameObject GamePlayDrawing { get; set; }
        public BaseImage ImageWashOut { get; set; }
        public BaseButton ButtonClose { get; set; }
        public SkeletonGraphic EllieGallerySkeleton { get; set; }
        public Image UIBlockSpamInit { get; set; }

    }
}