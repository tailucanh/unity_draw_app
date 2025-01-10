using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;



    public class GameHelper : Singleton<GameHelper>
    {
        private static string lessonId;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitOnLoad()
        {
            //Init();
        }

        public Tween DelayedCall(float delay, UnityAction action)
        {
            return DOVirtual.DelayedCall(delay, () => action?.Invoke());
        
            // return DOTween.Sequence()
            //     .AppendInterval(delay)
            //     .AppendCallback(() => action?.Invoke())
            //     .Play();
        }

        private GameObject ignoreTouch;
        

        public void EnableTouch()
        {
            if (ignoreTouch != null)
            {
                ignoreTouch.SetActive(false);
            }
        }
        
        public static void SetMultiTouchEnabled(bool v)
        {
            Input.multiTouchEnabled = v;
        }

      
        public string GetLessonId()
        {
            return lessonId;
        }

        public string SetLessonId(string id)
        {
            return lessonId = id;
        }
        
       
    }

