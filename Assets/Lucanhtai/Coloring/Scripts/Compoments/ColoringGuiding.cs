using DG.Tweening;
using Lucanhtai.Observer;
using System.Collections;
using UnityEngine;


namespace Lucanhtai.Coloring
{
    public class ColoringGuiding : MonoBehaviour
    {
        private Kindy2ColoringGuidingConfig guidingConfig;
        private bool isGuiding = false;
        private Coroutine coroutineGuiding;
        private Vector3 leftPoint;
        private Vector3 rightPoint;
        void Start()
        {
            leftPoint = new(transform.position.x - 1f, transform.position.y, transform.position.z);
            rightPoint = new(transform.position.x + 1f, transform.position.y, transform.position.z);
        }

        public void InitData(Kindy2ColoringGuidingConfig guidingConfig)
        {
            this.guidingConfig = guidingConfig;
        }
        public void StartGuiding()
        {
            isGuiding = true;
            coroutineGuiding = StartCoroutine(DoSomethingDelay());
        }
        private IEnumerator DoSomethingDelay()
        {
            yield return new WaitForSeconds(guidingConfig.secondDelay);
            while (isGuiding)
            {
                if (!isGuiding) break;
                yield return StartCoroutine(StartGuidingHand());
            }
        }
        private IEnumerator StartGuidingHand()
        {
            yield return transform.DOScale(1f, 0.5f).SetEase(Ease.Linear).WaitForCompletion();
            SoundChannel soundData = new SoundChannel(SoundChannel.PLAY_SOUND, guidingConfig.audioGuiding, null, 1, false);
            ObserverManager.TriggerEvent<SoundChannel>(soundData);
            yield return new WaitForSeconds(0.5f);
            yield return transform.DOMove(leftPoint, 1f).SetEase(Ease.Linear).WaitForCompletion();
            yield return transform.DOMove(rightPoint, 1f).SetEase(Ease.Linear).WaitForCompletion();

        }


        public void ResetGuiding()
        {
            StopAllCoroutines();
            if (coroutineGuiding != null)
            {
                StopCoroutine(coroutineGuiding);
            }
            SoundManager.Instance.StopMusic();
            transform.localScale = Vector3.zero;
            isGuiding = false;
            transform.DOKill();
        }

    }
}
