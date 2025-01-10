using Lucanhtai.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class ColoringButtonDone : BaseButton, IPointerDownHandler, IPointerUpHandler
    {
        private Shadow _shadow;
        private Vector2 originShadowPos;

        private void Start()
        {
             _shadow = GetComponent<Shadow>();
            originShadowPos = _shadow.effectDistance;
        }
        public override void Enable(bool isEnable)
        {

            button.interactable = isEnable;
        }

        public override void OnClick() {}
        public void OnPointerDown(PointerEventData eventData)
        {
            _shadow.effectDistance = Vector2.zero;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            _shadow.effectDistance = originShadowPos;
            UserInputChanel buttonData = new UserInputChanel(UserInputChanel.BUTTON_CLICK, gameObject.name);
            ObserverManager.TriggerEvent<UserInputChanel>(buttonData);
        }
    }
}