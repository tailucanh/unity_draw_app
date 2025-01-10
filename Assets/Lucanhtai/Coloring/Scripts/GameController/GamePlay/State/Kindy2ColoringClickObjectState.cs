using Cysharp.Threading.Tasks;
using Lucanhtai.Observer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringClickObjectState : FSMState
    {
        private const string ITEM_COLOR_SMALL_NAME = "ButtonColorSmall(Clone)";
        private const string ITEM_COLOR_BIG_NAME = "ButtonColorBig(Clone)";
        private const string PEN = "Pen";
        private const string WAX_PEN = "WaxPen";
        private const string ERASER = "Eraser";
        private const string BUTTON_CLOSE = "ButtonClose";
        private const string BUTTON_CHECK_DONE = "ButtonDone";
        //private const string BUTTON_EXIT = "ButtonExit";

        private Kindy2ColoringClickObjectStateObjectDependecy clickObjectDependecy;
        private Kindy2ColoringHandlesData dataDependency;
        private CancellationTokenSource cts;
        private List<BaseButton> listColors;
        private const float PERCENT_COMPLETE = 50f;

        public override void OnEnter(object data)
        {
            base.OnEnter(data);
            Kindy2ColoringClickStateDataEvent clickStateDataEvent = (Kindy2ColoringClickStateDataEvent)data;
            DoWork(clickStateDataEvent);
        }
        public override void SetUp(object data)
        {
            clickObjectDependecy = (Kindy2ColoringClickObjectStateObjectDependecy)data;
        }

        private void DoWork(Kindy2ColoringClickStateDataEvent clickStateDataEvent)
        {
            dataDependency = new Kindy2ColoringHandlesData();
            dataDependency.InitData(clickObjectDependecy.DataColorConfig);

            cts = new CancellationTokenSource();
            string[] objectClick = clickStateDataEvent.NameObject.Split("_");
            clickObjectDependecy.GuidingHand.ResetGuiding();
            listColors = clickObjectDependecy.LayoutColor.GetComponentsInChildren<BaseButton>().ToList();
            if (objectClick[0].Equals(ITEM_COLOR_SMALL_NAME) || objectClick[0].Equals(ITEM_COLOR_BIG_NAME))
            {
                OnclickButtonColor(clickStateDataEvent.NameObject, listColors, clickObjectDependecy.PaintController);
            }
            else if (objectClick[0].Equals(BUTTON_CLOSE))
            {
                dataDependency.DeleteMusicBackground();
                dataDependency.SaveStatusPlayGame(clickObjectDependecy.ClickConfig.idImage);
                if (clickObjectDependecy.PaintController.IsDrawing)
                {
                    clickObjectDependecy.PaintController.SavePixelsToFile(() =>
                    {
                        TriggerFinishClick(Kindy2ColoringState.GAME_FINISH_STATE);
                        GameObject.Destroy(clickObjectDependecy.GamePlay, .1f);
                    });
                }
                else
                {
                    TriggerFinishClick(Kindy2ColoringState.GAME_FINISH_STATE);
                    GameObject.Destroy(clickObjectDependecy.GamePlay, .1f);
                }
            }
            else if (objectClick[0].Equals(BUTTON_CHECK_DONE /*BUTTON_EXIT*/))
            {
                clickObjectDependecy.DataFinish.timeEnd = Time.realtimeSinceStartup;
                if (PlayerPrefs.GetFloat(clickObjectDependecy.DataFinish.key_pref + clickObjectDependecy.ClickConfig.idImage.ToString()) > 0)
                {
                    float getTime = PlayerPrefs.GetFloat(clickObjectDependecy.DataFinish.key_pref + clickObjectDependecy.ClickConfig.idImage.ToString());
                    clickObjectDependecy.DataFinish.timeSpent = (clickObjectDependecy.DataFinish.timeEnd - clickObjectDependecy.DataFinish.timeStart) + getTime;
                }
                else
                {
                    clickObjectDependecy.DataFinish.timeSpent = (clickObjectDependecy.DataFinish.timeEnd - clickObjectDependecy.DataFinish.timeStart);
                }
                PlayerPrefs.SetFloat(clickObjectDependecy.DataFinish.key_pref + clickObjectDependecy.ClickConfig.idImage.ToString(), clickObjectDependecy.DataFinish.timeSpent);
                if (clickObjectDependecy.DataFinish.isSendEvent)
                {
                    clickObjectDependecy.DataFinish.timeEnd = Time.realtimeSinceStartup;
                    clickObjectDependecy.DataFinish.score = 1;
                    if (PlayerPrefs.GetFloat(clickObjectDependecy.DataFinish.key_pref + clickObjectDependecy.ClickConfig.idImage.ToString()) > 0)
                    {
                        float getTime = PlayerPrefs.GetFloat(clickObjectDependecy.DataFinish.key_pref + clickObjectDependecy.ClickConfig.idImage.ToString());
                        clickObjectDependecy.DataFinish.timeSpent = (clickObjectDependecy.DataFinish.timeEnd - clickObjectDependecy.DataFinish.timeStart) + getTime;
                    }
                    else
                    {
                        clickObjectDependecy.DataFinish.timeSpent = (clickObjectDependecy.DataFinish.timeEnd - clickObjectDependecy.DataFinish.timeStart);
                    }

                    clickObjectDependecy.DataFinish.isSendEvent = false;
                }
                dataDependency.DeleteMusicBackground();
                dataDependency.SaveStatusPlayGame(clickObjectDependecy.ClickConfig.idImage);
                TriggerFinishClick(Kindy2ColoringState.OUTRO_STATE);

            }
            else if (objectClick[0].Equals(PEN) || objectClick[0].Equals(WAX_PEN) || objectClick[0].Equals(ERASER))
            {
                OnclickButtonPaint(clickStateDataEvent.NameObject, listColors, clickObjectDependecy.PaintController);
            }
        }
        private async void OnclickButtonColor(string nameObject, List<BaseButton> listColors, Kindy2ColoringPaintController paintController)
        {
            SoundChannel soundData;
            bool tscAudioClick = false;
            Action ActionDoneAudioClick = () =>
            {
                tscAudioClick = true;
            };

            ColoringImageColor itemClick = dataDependency.GetButtonByName(listColors, nameObject).GetComponent<ColoringImageColor>();
            int indexColor = dataDependency.GetIndexByName(nameObject);
            dataDependency.SelectedItemButton(itemClick, indexColor);
            dataDependency.EnableListButton(listColors, false);
            Color colorClick = clickObjectDependecy.DataColorConfig.colorButtons[indexColor];

            foreach (var item in clickObjectDependecy.ListDrawingTools)
            {
                if (item.name != ERASER)
                {
                    ColoringDrawingPen itemPen = item.GetComponent<ColoringDrawingPen>();
                    dataDependency.ChangeColorPen(itemPen, colorClick);
                }
            }
            paintController.SetPaintColor(colorClick);
            paintController.ReadCurrentCustomBrush();
            try
            {
                foreach (var item in listColors)
                {
                    if (item.name != itemClick.name)
                    {
                        ColoringImageColor itemColor = item.GetComponent<ColoringImageColor>();
                        dataDependency.UnSelectedItemButton(itemColor);
                    }
                }
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, clickObjectDependecy.ClickConfig.sfxClick, ActionDoneAudioClick, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
                await UniTask.WaitUntil(() => tscAudioClick, cancellationToken: cts.Token);
                dataDependency.EnableListButton(listColors, true);
                TriggerFinishClick(Kindy2ColoringState.PLAY_STATE);
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Timeout");
            }


        }

        private async void OnclickButtonPaint(string nameObject, List<BaseButton> listColors, Kindy2ColoringPaintController paintController)
        {
            SoundChannel soundData;
            bool tscAudioClick = false;
            Action ActionDoneAudioClick = () =>
            {
                tscAudioClick = true;
            };
            ColoringDrawingPen itemClick = dataDependency.GetButtonByName(clickObjectDependecy.ListDrawingTools, nameObject).GetComponent<ColoringDrawingPen>();

            dataDependency.SelectedItemPaint(itemClick);
            dataDependency.EnableListButton(listColors, false);
            dataDependency.EnableListButton(clickObjectDependecy.ListDrawingTools, false);

            foreach (var item in clickObjectDependecy.ListDrawingTools)
            {
                if (item.name != itemClick.name)
                {
                    ColoringDrawingPen itemPen = item.GetComponent<ColoringDrawingPen>();
                    dataDependency.UnSelectedItemPaint(itemPen);
                }
            }
            switch (itemClick.name)
            {
                case PEN:
                    paintController.SetDrawMode(DrawMode.Default);
                    paintController.SetPaintColor(dataDependency.GetCurrentColorSelected(listColors));
                    break;
                case WAX_PEN:
                    paintController.SetPaintColor(dataDependency.GetCurrentColorSelected(listColors));
                    paintController.SetDrawMode(DrawMode.CustomBrush);
                    paintController.ReadCurrentCustomBrush();
                    break;
                case ERASER:
                    paintController.SetDrawMode(DrawMode.Eraser);
                    break;
            }
            try
            {
                soundData = new SoundChannel(SoundChannel.PLAY_SOUND, clickObjectDependecy.ClickConfig.sfxClick, ActionDoneAudioClick, 1, false);
                ObserverManager.TriggerEvent<SoundChannel>(soundData);
                await UniTask.WaitUntil(() => tscAudioClick, cancellationToken: cts.Token);
                dataDependency.EnableListButton(listColors, true);
                dataDependency.EnableListButton(clickObjectDependecy.ListDrawingTools, true);
                TriggerFinishClick(Kindy2ColoringState.PLAY_STATE);
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Timeout");
            }
        }

        private void TriggerFinishClick(Kindy2ColoringState state)
        {
            Kindy2ColoringEvent kindy2ColoringEvent = new Kindy2ColoringEvent(state.ToString(), null);
            ObserverManager.TriggerEvent<Kindy2ColoringEvent>(kindy2ColoringEvent);
        }
        public override void OnExit()
        {
            dataDependency.EnableListButton(listColors, true);
            dataDependency.EnableListButton(clickObjectDependecy.ListDrawingTools, true);
            cts?.Cancel();
        }
        public override void OnDestroy()
        {

            cts?.Cancel();
            cts?.Dispose();
        }
    }


    public class Kindy2ColoringClickStateDataEvent
    {
        public string NameObject { get; set; }
    }

    public class Kindy2ColoringClickObjectStateObjectDependecy
    {
        public Kindy2ColoringClickConfig ClickConfig { get; set; }
        public Kindy2ColoringDataColor DataColorConfig { get; set; }
        public Kindy2ColoringDataFinish DataFinish { get; set; }
        public Kindy2ColoringPaintController PaintController { get; set; }
        public GameObject GamePlay { get; set; }
        public GridLayoutGroup LayoutColor { get; set; }
        public List<BaseButton> ListDrawingTools { get; set; }
        public ColoringGuiding GuidingHand { get; set; }
        public GameObject IntroGalleryRoom { get; set; }
        public GameObject OutroGalleryRoom { get; set; }

    }
}