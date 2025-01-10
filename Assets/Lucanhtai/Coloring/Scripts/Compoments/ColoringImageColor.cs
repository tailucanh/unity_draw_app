using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lucanhtai.Coloring
{
    public class ColoringImageColor : BaseImage
    {
        [SerializeField] Image box;
        [SerializeField] Image border;
        [SerializeField] Image currentColor;
        public override void SetColor(Color color) {
            currentColor.color = color;
        }
        public void SetImageColor(Color colorBox, Color colorBorder)
        {
            box.color = colorBox;
            border.color = colorBorder;
        }
        public Color GetCurrentColor()
        {
            return currentColor.color;
        }
        public override float SetColor(Color color, float time)
        {
            return 0;
        }
    }
}