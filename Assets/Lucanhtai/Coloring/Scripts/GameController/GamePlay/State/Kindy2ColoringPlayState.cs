using Lucanhtai.Observer;
using UnityEngine;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringPlayState : FSMState
    {
        private Kindy2ColoringPlayStateObjectDependecy playObjectDependecy;
        public override void OnEnter(object data)
        {
            base.OnEnter(data);
            DoWork();
        }

        public override void SetUp(object data)
        {
            playObjectDependecy = (Kindy2ColoringPlayStateObjectDependecy)data;
        }

        private void DoWork()
        {
            if (playObjectDependecy.PaintController.IsDrawing)
            {
                playObjectDependecy.ButtonCheckDone.gameObject.SetActive(true);
                playObjectDependecy.ButtonCheckNormal.gameObject.SetActive(false);
            }
            if (playObjectDependecy.UIBlockSpamInit.raycastTarget)
            {
                playObjectDependecy.UIBlockSpamInit.raycastTarget = false;
                playObjectDependecy.ButtonClose.Enable(true);
                playObjectDependecy.ImageWashOut.SetColor(new Color32(0, 0, 0, 0));
                SoundChannel soundData = new SoundChannel(SoundChannel.PLAY_SOUND_NEW_OBJECT, playObjectDependecy.AudioBackground, null, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
            }

            playObjectDependecy.GuidingHand.InitData(playObjectDependecy.GuidingConfig);
            playObjectDependecy.GuidingHand.StartGuiding();
        }

        public override void OnExit()
        {
        }
        public override void OnDestroy()
        {
            playObjectDependecy.DataFinish.timeStart = 0;
            playObjectDependecy.DataFinish.timeEnd = 0;
            playObjectDependecy.DataFinish.score = 0;
            playObjectDependecy.DataFinish.isSendEvent = true;
            playObjectDependecy.ButtonCheckDone.gameObject.SetActive(false);
            playObjectDependecy.ButtonCheckNormal.gameObject.SetActive(true);
        }
    }

    public class Kindy2ColoringPlayStateObjectDependecy
    {
        public Kindy2ColoringGuidingConfig GuidingConfig { get; set; }
        public Kindy2ColoringClickConfig ClickConfig { get; set; }
        public Kindy2ColoringPaintController PaintController { get; set; }
        public Kindy2ColoringDataFinish DataFinish { get; set; }
        public ColoringGuiding GuidingHand { get; set; }
        public BaseButton ButtonCheckDone { get; set; }
        public GameObject ButtonCheckNormal { get; set; }
        public Image UIBlockSpamInit { get; set; }
        public BaseImage ImageWashOut { get; set; }
        public BaseButton ButtonClose { get; set; }
        public AudioClip AudioBackground { get; set; }
    }
}