using Cysharp.Threading.Tasks;
using Lucanhtai.Observer;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringInitState : FSMState
    {
        private Kindy2ColoringInitStateObjectDependecy initObjectDependecy;
        private Kindy2ColoringHandlesData coloringHandlesData;
        private string nameProfile;
        private string[] parametter;
        private CancellationTokenSource cts;
        //private Texture2D textureToSave;
        public override void OnEnter(object data)
        {
            base.OnEnter(data);
            Kindy2ColoringInitStateData initStateData = (Kindy2ColoringInitStateData)data;
            DoWork(initStateData);
        }

        public override void SetUp(object data)
        {
            initObjectDependecy = (Kindy2ColoringInitStateObjectDependecy)data;
        }

        private async void DoWork(Kindy2ColoringInitStateData initStateData)
        {
            initObjectDependecy.ButtonClose.Enable(false);
            coloringHandlesData = new Kindy2ColoringHandlesData();
            cts = new CancellationTokenSource();
            coloringHandlesData.InitData(initObjectDependecy.DataColorConfig);
            initObjectDependecy.ClickConfig.idImage = initStateData.DataPlay.idImage;
            List<Color32> colors = initObjectDependecy.DataColorConfig.colorButtons;
       
            int timeDelayOut = initObjectDependecy.InitDataConfig.timeDelayOut;
            int timeDelayIn = initObjectDependecy.InitDataConfig.timeDelayIn;

            for (int i = 0; i < colors.Count; i++)
            {
                ColoringImageColor item = SpawnButton(i, colors[i]).GetComponent<ColoringImageColor>();

                if (i == 0)
                {
                    coloringHandlesData.SelectedItemButton(item, i);
                }
            }
            initObjectDependecy.UIBlockSpamInit.raycastTarget = true;
            ColoringDrawingPen firstPen = initObjectDependecy.ListDrawingTools[0].GetComponent<ColoringDrawingPen>();
            coloringHandlesData.SelectedItemPaint(firstPen);
           
            if (coloringHandlesData.CheckImageStatus(initObjectDependecy.ClickConfig.idImage))
            {
                initObjectDependecy.ImageWashOut.SetColor(new Color32(0, 0, 0, 255));

                initObjectDependecy.PaintController.SetMaskImage(initStateData.DataPlay.dataImage.texture, initStateData.DataPlay.idImage);
                initObjectDependecy.PaintController.InitializeEverything();

                initObjectDependecy.IntroGalleryRoom.transform.localScale = Vector3.zero;
                initObjectDependecy.GamePlayDrawing.transform.localScale = Vector3.one;
                if (coloringHandlesData.CheckImageDataSave(initObjectDependecy.ClickConfig.idImage))
                {
                    initObjectDependecy.ButtonCheckDone.gameObject.SetActive(true);
                    initObjectDependecy.ButtonCheckNormal.gameObject.SetActive(false);
                }
                await UniTask.Delay(1500, cancellationToken: cts.Token);
                initObjectDependecy.ImageWashOut.SetColor(new Color32(0, 0, 0, 0), (timeDelayIn / 1000f));
                initObjectDependecy.ButtonClose.Enable(true);
                SoundChannel soundData = new SoundChannel(SoundChannel.PLAY_SOUND_NEW_OBJECT, initObjectDependecy.InitDataConfig.audioBackground, null, 1, true);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
                initObjectDependecy.UIBlockSpamInit.raycastTarget = false;
                TriggerFinishSetData(Kindy2ColoringState.PLAY_STATE);
            }
            else
            {
                /*initObjectDependecy.ButtonClose.Enable(true);
                initObjectDependecy.GamePlayDrawing.transform.localScale = Vector3.one;
                initObjectDependecy.IntroGalleryRoom.transform.localScale = Vector3.zero;
                initObjectDependecy.IntroDrawingRoom.transform.localScale = Vector3.zero;
                TriggerFinishSetData(Kindy2ColoringState.PLAY_STATE);*/

                initObjectDependecy.PaintController.SetMaskImage(initStateData.DataPlay.dataImage.texture, initStateData.DataPlay.idImage);
                initObjectDependecy.PaintController.InitializeEverything();

                initObjectDependecy.GamePlayDrawing.transform.localScale = Vector3.zero;
                TriggerFinishSetData(Kindy2ColoringState.INTRO_STATE);
            }


        }

        private GameObject SpawnButton(int index, Color32 color)
        {
            GameObject item = Object.Instantiate(initObjectDependecy.ButtonColor, initObjectDependecy.LayoutColor.transform, false);
            item.name = item.name + "_" + index;
            item.GetComponent<BaseImage>().SetColor(color);
            return item;
        }

        private void TriggerFinishSetData(Kindy2ColoringState eventName)
        {
            Kindy2ColoringEvent kindy2ColoringEvent = new Kindy2ColoringEvent(eventName.ToString(), null);
            ObserverManager.TriggerEvent<Kindy2ColoringEvent>(kindy2ColoringEvent);
        }

        public override void OnExit()
        {
            cts?.Cancel();
        }
        public override void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }

    public class Kindy2ColoringInitStateData
    {
        public Kindy2ColoringConversationData DataPlay { get; set; }

    }

    public class Kindy2ColoringInitStateObjectDependecy
    {
        public Kindy2ColoringInitDataConfig InitDataConfig { get; set; }
        public Kindy2ColoringClickConfig ClickConfig { get; set; }
        public Kindy2ColoringLocalizationConfig LocalizationConfig { get; set; }
        public Kindy2ColoringDataColor DataColorConfig { get; set; }
        public Kindy2ColoringPaintController PaintController { get; set; }
        public BaseImage ImageWashOut { get; set; }
        public GameObject IntroGalleryRoom { get; set; }
        public GameObject GamePlayDrawing { get; set; }
        public GameObject ButtonColor { get; set; }
        public BaseButton ButtonClose { get; set; }
        public GridLayoutGroup LayoutColor { get; set; }
        public List<BaseButton> ListDrawingTools { get; set; }
        public Image UIBlockSpamInit { get; set; }
        public BaseButton ButtonCheckDone { get; set; }
        public GameObject ButtonCheckNormal { get; set; }

    }
}