using DG.Tweening;
using Lucanhtai.Observer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Lucanhtai.Coloring
{
    public class ColoringEffect : MonoBehaviour
    {
        List<Image> listItem;
        private float fireworkSpeed = 1f;
        private float spreadRadius = 1f;
        [SerializeField] private AudioClip sfxfirework;
        void Start()
        {
            listItem = GetComponentsInChildren<Image>().ToList();
        }
        public void EffectPaint(Vector3 transformSpawn)
        {
            ResetItem();
            SoundChannel soundData = new SoundChannel(SoundChannel.PLAY_SOUND, sfxfirework, null, 1, false);
            ObserverManager.TriggerEvent<SoundChannel>(soundData);
            transform.position = transformSpawn;
            transform.localScale = Vector3.one;
            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].transform.DOKill();
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle;
                Vector3 targetPosition = transformSpawn + new Vector3(randomCircle.x, randomCircle.y, 0) * spreadRadius;

                listItem[i].transform.DOMove(targetPosition, fireworkSpeed).SetEase(Ease.OutQuad);
                listItem[i].transform.DOScale(Vector3.zero, fireworkSpeed / 2).SetEase(Ease.InQuad).SetDelay(fireworkSpeed / 2);
            }
        }
        private void ResetItem()
        {
            transform.localScale = Vector3.zero;
            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].transform.localScale = Vector3.one;
                listItem[i].transform.position = Vector3.zero;
            }
        }

    }
}