using Cysharp.Threading.Tasks;
using Lucanhtai.Observer;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringPauseState : FSMState
    {
        private Kindy2ColoringPauseStateObjectDependecy pauseObjectDependecy;
        private CancellationTokenSource cts;

        public override void OnEnter(object data)
        {
            base.OnEnter(data);
            DoWork();
        }

        public override void SetUp(object data)
        {
            pauseObjectDependecy = (Kindy2ColoringPauseStateObjectDependecy)data;
        }

        private void DoWork()
        {
            cts = new CancellationTokenSource();
            try
            {
                if (IsScaleUI(pauseObjectDependecy.IntroGalleryRoom))
                {
                    pauseObjectDependecy.EllieGallerySkeleton.AnimationState.TimeScale = 0f;
                }
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Timeout");
            }
        }

        private bool IsScaleUI(GameObject gameObject)
        {
            return gameObject.transform.localScale == Vector3.one;
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

    public class Kindy2ColoringPauseStateObjectDependecy
    {
        public GameObject IntroGalleryRoom { get; set; }
        public List<string> AnimationEllie { get; set; }
        public SkeletonGraphic EllieGallerySkeleton { get; set; }
    }
}